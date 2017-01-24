using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Schema;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Deposit.Extensions;
using Nop.Plugin.Payments.Deposit.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Deposit.Controllers
{
    public class PaymentDepositController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IStoreContext _storeContext;
        private readonly IPriceFormatter _priceFormatter;

        public PaymentDepositController(IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService,
            IStoreContext storeContext,
            IPriceFormatter priceFormatter)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._storeContext = storeContext;
            this._priceFormatter = priceFormatter;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var depositPaymentSettings = _settingService.LoadSetting<DepositPaymentSettings>();

            var model = new ConfigurationModel() {
                DescriptionText = depositPaymentSettings.DescriptionText
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DescriptionText = depositPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, languageId, false, false);
            });
            #region Stores
// It concerns diferent settings for different stores. Probably in the future I'll implement that
//            model.AdditionalFee = depositPaymentSettings.AdditionalFee;
//            model.AdditionalFeePercentage = depositPaymentSettings.AdditionalFeePercentage;
//            model.ShippableProductRequired = depositPaymentSettings.ShippableProductRequired;

//            model.ActiveStoreScopeConfiguration = storeScope;
//            if (storeScope > 0)
//            {
//                model.DescriptionText_OverrideForStore = _settingService.SettingExists(depositPaymentSettings, x => x.DescriptionText, storeScope);
//                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(depositPaymentSettings, x => x.AdditionalFee, storeScope);
//                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(depositPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
//                model.ShippableProductRequired_OverrideForStore = _settingService.SettingExists(depositPaymentSettings, x => x.ShippableProductRequired, storeScope);
//            }
            #endregion
            return View("~/Plugins/Payments.Deposit/Views/PaymentDeposit/Configure.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            return new List<string>();
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var depositPaymentSettings = _settingService.LoadSetting<DepositPaymentSettings>();
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var orderTotal = this._orderTotalCalculationService.GetShoppingCartTotal(cart);
            var orderShippingTotal = this._orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
            var total = orderTotal ?? 0 + orderShippingTotal ?? 0;
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            //var balance = this._workContext.CurrentCustomer.GetAttribute<decimal>("Deposit", genericAttributeService);
            var model = new PaymentInfoModel
            {
                DescriptionText = depositPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, _workContext.WorkingLanguage.Id),
                CurrentDepositAmount = this._priceFormatter.FormatPrice(
                    this._currencyService.ConvertFromPrimaryStoreCurrency(
                        this._workContext.CurrentCustomer.GetAttribute<decimal>("Deposit", genericAttributeService), this._workContext.WorkingCurrency)),
                FutureDepositAmount = this._priceFormatter.FormatPrice(
                    this._currencyService.ConvertFromPrimaryStoreCurrency(
                        this._workContext.CurrentCustomer.GetAttribute<decimal>("Deposit", genericAttributeService) - total, this._workContext.WorkingCurrency)),
                OrderTotal = this._priceFormatter.FormatPrice(total)
            };
            return View("~/Plugins/Payments.Deposit/Views/PaymentDeposit/PaymentInfo.cshtml", model);
        }
    }
}