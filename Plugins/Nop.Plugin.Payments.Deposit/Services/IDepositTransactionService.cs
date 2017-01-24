using System.Collections.Generic;
using Nop.Plugin.Payments.Deposit.Domain;

namespace Nop.Plugin.Payments.Deposit.Services
{
    public interface IDepositTransactionService
    {
        void AddTransaction(DepositTransaction transaction);
        void VoidTransaction(int transactionId);
        IList<DepositTransaction> GetTransactions();
    }
}