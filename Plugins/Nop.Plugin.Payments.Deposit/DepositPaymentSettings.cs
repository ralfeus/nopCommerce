using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Deposit
{
    public class DepositPaymentSettings : ISettings
    {
        /// <summary>
        /// Description of the payment method
        /// </summary>
        public string DescriptionText { get; set; }
    }
}