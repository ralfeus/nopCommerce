using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Deposit.Domain;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class AdminDepositModel : BaseNopModel
    {
        public IList<PaymentStatus> AvailableStatuses => new List<PaymentStatus>
        {
            PaymentStatus.Pending,
            PaymentStatus.Paid
        };
        public IList<Customer> Customers { get; set; }
        public IList<DepositTransactionModel> Transactions { get; set; }
    }
}