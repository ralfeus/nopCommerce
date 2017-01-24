using System;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Services.Customers;

namespace Nop.Plugin.Payments.Deposit.Domain
{
    public class DepositTransaction : BaseEntity
    {
        public virtual int CustomerId { get; set; }
        public virtual string PaymentMethodSystemName { get; set; }
        public virtual decimal TransactionAmount { get; set; }
        public virtual DateTime TransactionTime { get; set; }
        public virtual PaymentStatus Status { get; set; }
        public virtual int AddedBy { get; set; }
        public virtual decimal NewBalance { get; set; }
    }
}