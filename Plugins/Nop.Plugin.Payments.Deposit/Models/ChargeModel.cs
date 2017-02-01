using System.Collections.Generic;
using Nop.Core.Domain.Payments;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class ChargeModel : BaseNopModel
    {
        public IList<CheckoutPaymentMethodModel.PaymentMethodModel> PaymentMethods { get; set; }
        public decimal ChargeAmount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public string Currency { get; set; }

        public ChargeModel()
        {
            this.PaymentMethods = new List<CheckoutPaymentMethodModel.PaymentMethodModel>();
        }
    }
}