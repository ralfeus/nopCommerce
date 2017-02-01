using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Deposit.Domain;
using Nop.Plugin.Payments.Deposit.Extensions;
using Nop.Plugin.Payments.Deposit.Models;
using Nop.Plugin.Payments.Deposit.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Deposit.Controllers
{
    public class PublicDepositController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentService _paymentService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ITaxService _taxService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly AddressSettings _addressSettings;
        private readonly IDepositTransactionService _depositTransactionService;
        private readonly ILogger _logger;

        public PublicDepositController(IWorkContext workContext, IPriceFormatter priceFormatter, ICurrencyService currencyService,
            IPaymentService paymentService, IStoreContext storeContext, ILocalizationService localizationService, IWebHelper webHelper,
            ITaxService taxService, IGenericAttributeService genericAttributeService, AddressSettings addressSettings,
            IDepositTransactionService depositTransactionService, ILogger logger)
        {
            this._workContext = workContext;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._paymentService = paymentService;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            _webHelper = webHelper;
            _taxService = taxService;
            _genericAttributeService = genericAttributeService;
            _addressSettings = addressSettings;
            _depositTransactionService = depositTransactionService;
            _logger = logger;
        }

        [ChildActionOnly]
        public ActionResult PublicInfoNavigation()
        {
            return View("~/Plugins/Payments.Deposit/Views/PublicDeposit/PublicInfoNavigation.cshtml");
        }

        public ActionResult PublicInfo()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            var currency = this._currencyService.GetCurrencyByCode(customer.GetDepositCurrency());
            var model = new PublicDepositModel
            {
                DepositAmount = this._priceFormatter.FormatPrice(customer.GetDepositBalance(), true, currency),
                DepositCurrencyCode = currency.CurrencyCode,
                ChangeCurrencyConfirmationMessage = this.GetChangeDepositCurrencyConfirmationMessageText(
                    _localizationService.GetResource("Payment.Deposit.ChangeCurrencyConfirm"),
                    currency,
                    this._workContext.WorkingCurrency,
                    customer.GetDepositBalance(),
                    this._currencyService.ConvertCurrency(
                        customer.GetDepositBalance(),
                        currency,
                        this._workContext.WorkingCurrency
                    ))
            };
            return View("~/Plugins/Payments.Deposit/Views/PublicDeposit/PublicInfo.cshtml", model);
        }

        [NonAction]
        private string GetChangeDepositCurrencyConfirmationMessageText(string template, Currency sourceCurrency,
            Currency targetCurrency, decimal sourceDepositAmount, decimal targetDepositAmount)
        {
            return string.Format(template,
                sourceCurrency.CurrencyCode,
                targetCurrency.CurrencyCode,
                this._priceFormatter.FormatPrice(sourceDepositAmount, true, sourceCurrency),
                this._priceFormatter.FormatPrice(targetDepositAmount, true, targetCurrency));
        }

        [NonAction]
        protected virtual ChargeModel PrepareChargeModel(int filterByCountryId)
        {
            var model = new ChargeModel
            {
                Currency = this._workContext.CurrentCustomer.GetDepositCurrency()
            };
            var cart = new List<ShoppingCartItem>();

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .Where(pm => !(pm is DepositPaymentProcessor))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                var rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                var rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!string.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }


        public ActionResult Charge()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //filter by country
            int filterByCountryId = 0;
            if (_addressSettings.CountryEnabled &&
                _workContext.CurrentCustomer.BillingAddress != null &&
                _workContext.CurrentCustomer.BillingAddress.Country != null)
            {
                filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
            }

            var paymentMethodModel = PrepareChargeModel(filterByCountryId);

            //customer have to choose a payment method
            return View("~/Plugins/Payments.Deposit/Views/PublicDeposit/Charge.cshtml", paymentMethodModel);
        }

        [HttpPost]
        public ActionResult Charge(ChargeModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered() || _workContext.CurrentCustomer.IsGuest())
                return new HttpUnauthorizedResult();
            if  (!ModelState.IsValid)
                return Charge();
                           try
            {
                //validation

                if (_workContext.CurrentCustomer.IsGuest())
                    return new HttpUnauthorizedResult();

                // check payment method
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(model.PaymentMethod);
                if (paymentMethod == null)
                    return RedirectToRoute("HomePage");

                // add deposit transaction
                var transaction = new DepositTransaction
                {
                    CustomerId = this._workContext.CurrentCustomer.Id,
                    PaymentMethodSystemName = model.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    TransactionAmount = model.ChargeAmount,
                    TransactionCurrencyCode = this._workContext.CurrentCustomer.GetDepositCurrency(),
                    TransactionTime = DateTime.Now
                };
                this._depositTransactionService.AddTransaction(transaction);


//                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
//                    return RedirectToRoute("HomePage");


                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = new Order
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        Customer = this._workContext.CurrentCustomer,
                        OrderTotal = model.ChargeAmount,
                        PaymentMethodSystemName = model.PaymentMethod,
                        PaymentStatus = PaymentStatus.Pending
                    }
                };

                _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content("Redirected");
                }
                model.Status = postProcessPaymentRequest.Order.PaymentStatus;
                return View("~/Plugins/Payments.Deposit/Views/PublicDeposit/ChargeProceed.cshtml", model);
            }
            catch (Exception exc)
            {
                this._logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Content(exc.Message);
            }
        }

        public ActionResult ChangeCurrency()
        {
            var customer = this._workContext.CurrentCustomer;
            var deposit = customer.GetDepositBalance();
            var sourceCurrency = this._currencyService.GetCurrencyByCode(customer.GetDepositCurrency());
            var targetCurrency = this._workContext.WorkingCurrency;
            customer.SubtractDeposit(deposit);
            customer.SetDepositCurrency(targetCurrency.CurrencyCode);
            customer.AddDeposit(this._currencyService.ConvertCurrency(deposit, sourceCurrency, targetCurrency));
            return RedirectToAction("PublicInfo");
        }
    }
}