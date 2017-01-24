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
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Payments.Deposit
{
    public class DepositPaymentProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly IWorkContext _workContext;

        public DepositPaymentProcessor(IWorkContext workContext) {
            this._workContext = workContext;
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
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.AccountNavigationDeposit", "Deposit");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.CustomerDeposit", "Customer deposit");
            this.AddOrUpdatePluginLocaleResource("Payment.Deposit.Charge", "Charge deposit");
            // Payment.Deposit.DepositAmount
            // "Payment.Deposit.ProceedPayment"
            // "Payment.Deposit.TransactionAmount"
            // Payment.Deposit.TransactionID
            // Payment.Deposit.PaymentStatus
            // Payment.Deposit.CreatedOn

            //TODO: Set up widget setting so it's immediately active
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Payment.Deposit.AccountNavigationDeposit");
            this.DeletePluginLocaleResource("Payment.Deposit.CustomerDeposit");
            this.DeletePluginLocaleResource("Payment.Deposit.Charge");

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
            this._workContext.CurrentCustomer.SubtractDeposit(processPaymentRequest.OrderTotal);
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