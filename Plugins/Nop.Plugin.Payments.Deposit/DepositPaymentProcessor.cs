using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Deposit.Controllers;
using Nop.Plugin.Payments.Deposit.Extensions;
using Nop.Services.Cms;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Payments.Deposit
{
    public class DepositPaymentProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;

        public DepositPaymentProcessor(IWorkContext workContext, ICurrencyService currencyService)
        {
            this._workContext = workContext;
            _currencyService = currencyService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
//            actionName = "Configure";
//            controllerName = "PaymentDeposit";
//            routeValues = new RouteValueDictionary
//            {
//                {"Namespaces", "Nop.Plugin.Payments.Deposit.Controllers"},
//                {"area", null}
//            };
            actionName = null;
            controllerName = null;
            routeValues = null;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.CustomerDeposit", "Your deposit");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.Charge", "Charge deposit");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.DepositAmount", "Deposit amount");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.ProceedPayment", "Proceed payment");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.TransactionAmount", "Transaction amount");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.TransactionCurrencyCode", "Currency");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.TransactionID", "ID");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.TransactionTime", "Time");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.PaymentStatus", "Status");
//            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.CreatedOn
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.PaymentPending",
                "Your payment is awaiting for administrator's approval");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.PaymentPaid", "Your payment is accepted");
            // {0} - source currency code
            // {1} - target currency code
            // {2} - deposit amount in source currency
            // {3} - deposit amount in target currency
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.ChangeCurrency", "Change deposit currency");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.ChangeCurrencyConfirm",
                "You are abount to change your deposit currency from {0} to {1}.<br />"+
                "Now you have {2} {0}.<br />"+
                "After conversion you will have {3} {1} on you deposit.<br />"+
                "Are you sure you want to perform this conversion?");
//            this.AddOrUpdatePluginLocaleResource("Admin.Common.Approve", "App"
//            this.AddOrUpdatePluginLocaleResource("Admin.Common.Reject"

            //TODO: Set up widget setting so it's immediately active
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Payment.Deposit.CustomerDeposit");
            this.DeletePluginLocaleResource("Payment.Deposit.Charge");
            this.DeletePluginLocaleResource("Payment.Deposit.DepositAmount");
            this.DeletePluginLocaleResource("Payment.Deposit.ProceedPayment");
            this.DeletePluginLocaleResource("Payment.Deposit.TransactionAmount");
            this.DeletePluginLocaleResource("Payment.Deposit.TransactionCurrencyCode");
            this.DeletePluginLocaleResource("Payment.Deposit.TransactionID");
            this.DeletePluginLocaleResource("Payment.Deposit.TransactionTime");
            this.DeletePluginLocaleResource("Payment.Deposit.PaymentStatus");
            this.DeletePluginLocaleResource("Payment.Deposit.PaymentPending");
            this.DeletePluginLocaleResource("Payment.Deposit.PaymentPaid");
            this.DeletePluginLocaleResource("Payment.Deposit.ChangeCurrency");
            this.DeletePluginLocaleResource("Payment.Deposit.ChangeCurrencyConfirm");

            base.Uninstall();
        }

        #region IAdminMenuPlugin implementation

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Payments.Deposit",
                Title = "Deposit",
                ControllerName = "AdminDeposit",
                ActionName = "List",
                Visible = true,
                Url = "admin/deposit",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if(pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        #endregion

        #region IPaymentMethod implementation

        public bool SupportCapture => false;
        public bool SupportPartiallyRefund => true;
        public bool SupportRefund => true;
        public bool SupportVoid => false;
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;
        public bool SkipPaymentInfo => false;

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var customerDepositCurrencyCode = this._workContext.CurrentCustomer.GetDepositCurrency();
            var customerDepositCurrency = this._currencyService.GetCurrencyByCode(customerDepositCurrencyCode);
            this._workContext.CurrentCustomer.SubtractDeposit(this._currencyService.ConvertFromPrimaryStoreCurrency(
                processPaymentRequest.OrderTotal, customerDepositCurrency));
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Paid
            };
            return result;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return this._workContext.CurrentCustomer.IsGuest();
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            throw new NotImplementedException();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public bool CanRePostProcessPayment(Order order)
        {
            throw new NotImplementedException();
        }

        public Type GetControllerType()
        {
            return typeof(PaymentDepositController);
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentDeposit";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Payments.Deposit.Controllers" },
                { "area", null }
            };
        }

        #endregion

        #region IWidgetPlugin implementation

        public IList<string> GetWidgetZones()
        {
            return new[] {"account_navigation_after"};
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            controllerName = "PublicDeposit";
            actionName = "PublicInfoNavigation";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Payments.Deposit.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        #endregion
    }
}