using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class ChargeModel : BaseNopModel
    {
        public IList<CheckoutPaymentMethodModel.PaymentMethodModel> PaymentMethods { get; set; }
        public decimal ChargeAmount { get; set; }
    }
}