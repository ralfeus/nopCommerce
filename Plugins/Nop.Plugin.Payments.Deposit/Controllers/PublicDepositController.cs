using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Deposit.Extensions;
using Nop.Plugin.Payments.Deposit.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
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

        public PublicDepositController(IWorkContext workContext, IPriceFormatter priceFormatter, ICurrencyService currencyService,
            IPaymentService paymentService, IStoreContext storeContext, ILocalizationService localizationService, IWebHelper webHelper,
            ITaxService taxService, IGenericAttributeService genericAttributeService, AddressSettings addressSettings)
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

            var currentDepositAmount = _workContext.CurrentCustomer.GetDepositBalance();
            var model = new PublicDepositModel
            {
                DepositAmount = this._priceFormatter.FormatPrice(
                    this._currencyService.ConvertFromPrimaryStoreCurrency(currentDepositAmount, this._workContext.WorkingCurrency))
            };
            return View("~/Plugins/Payments.Deposit/Views/PublicDeposit/PublicInfo.cshtml", model);
        }

        [NonAction]
        protected virtual ChargeModel PrepareChargeModel(int filterByCountryId)
        {
            var model = new ChargeModel();
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
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
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
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();
            if (!ModelState.IsValid)
            {
                return Charge();
            }
            return View();
        }
    }
}