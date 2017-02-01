using System.Data.Entity.ModelConfiguration;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.Deposit.Domain;

namespace Nop.Plugin.Payments.Deposit.Data
{
    public class DepositTransactionMap : NopEntityTypeConfiguration<DepositTransaction>
    {
        public DepositTransactionMap()
        {
            ToTable("DepositTransaction");
            HasKey(m => m.Id);
            Property(m => m.CustomerId);
            Property(m => m.PaymentMethodSystemName);
            Property(m => m.TransactionAmount);
            Property(m => m.TransactionCurrencyCode);
            Property(m => m.TransactionTime);
            Property(m => m.Status);
            Property(m => m.AddedBy);
            Property(m => m.NewBalance);
        }
    }
}