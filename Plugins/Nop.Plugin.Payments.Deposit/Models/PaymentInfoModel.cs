using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
        [NopResourceDisplayName("Payment.Deposit.CurrentDepositAmount")]
        [AllowHtml]
        public string CurrentDepositAmount { get; set; }
        [NopResourceDisplayName("Payment.Deposit.FutureDepositAmount")]
        [AllowHtml]
        public string FutureDepositAmount { get; set; }
        [NopResourceDisplayName("Order.OrderTotal")]
        [AllowHtml]
        public string OrderTotal { get; set; }
    }
}