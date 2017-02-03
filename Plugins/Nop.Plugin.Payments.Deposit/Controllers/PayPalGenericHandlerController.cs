using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Deposit.Domain;
using Nop.Plugin.Payments.Deposit.Services;
using Nop.Plugin.Payments.PayPalStandard;
using Nop.Plugin.Payments.PayPalStandard.Controllers;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit.Controllers
{
    public class PayPalGenericHandlerController : BasePluginController
    {
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IDepositTransactionService _depositTransactionService;

        public PayPalGenericHandlerController(IWebHelper webHelper, IPaymentService paymentService,
            PaymentSettings paymentSettings, IDepositTransactionService depositTransactionService)
        {
            _webHelper = webHelper;
            _paymentService = paymentService;
            _paymentSettings = paymentSettings;
            _depositTransactionService = depositTransactionService;
        }

        public ActionResult PdtHandler(FormCollection form)
        {
            var payPal = DependencyResolver.Current.GetService<PaymentPayPalStandardController>();
            payPal.ControllerContext = this.ControllerContext;

            var tx = _webHelper.QueryString<string>("tx");
            Dictionary<string, string> values;
            string response;

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.PayPalStandard") as PayPalStandardPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("PayPal Standard module cannot be loaded");

            if (processor.GetPdtDetails(tx, out values, out response))
            {
                string orderNumber;
                values.TryGetValue("custom", out orderNumber);
                var orderNumberGuid = Guid.Empty.ToString();
                try
                {
                    orderNumberGuid = new Guid(orderNumber).ToString();
                }
                catch
                {
                }
                if (orderNumberGuid.Contains("-0001-0000-0000-000000000000"))
                {
                    return HandleDepositCharge(Convert.ToInt32(orderNumberGuid.Substring(0, 8), 16), values);
                }
            }
            return payPal.PDTHandler(null);
        }

        public ActionResult HandleDepositCharge(int transactionId, Dictionary<string, string> values)
        {
            var transaction = new DepositTransaction{Id = transactionId};
            string status;
            values.TryGetValue("payment_status", out status);
            switch (status)
            {
                case "Completed":
                case "Processed":
                    transaction.Status = PaymentStatus.Paid;
                    break;
                case "Denied":
                case "Expired":
                case "Failed":
                case "Voided":
                    transaction.Status = PaymentStatus.Voided;
                    break;
            }
            this._depositTransactionService.SetTransaction(transaction);
            return RedirectToRoute("Plugin.Payments.Deposit.ChargeComplete", new {transactionId});
        }
    }
}