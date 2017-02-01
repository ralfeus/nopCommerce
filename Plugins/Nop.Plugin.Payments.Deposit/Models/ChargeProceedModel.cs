using System.Collections.Generic;
using Nop.Core.Domain.Payments;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class ChargeProceedModel : BaseNopModel
    {
        public PaymentStatus Status { get; set; }
        public decimal NewBalance { get; set; }
    }
}