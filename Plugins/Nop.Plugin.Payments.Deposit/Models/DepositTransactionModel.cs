using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class DepositTransactionModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMethodName { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionTime { get; set; }
        public int StatusId { get; set; }
        public int AddedBy { get; set; }
        public string NewBalance { get; set; }
    }
}