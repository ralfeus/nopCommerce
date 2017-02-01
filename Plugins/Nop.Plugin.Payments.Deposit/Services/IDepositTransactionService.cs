using System.Collections.Generic;
using Nop.Plugin.Payments.Deposit.Domain;

namespace Nop.Plugin.Payments.Deposit.Services
{
    public interface IDepositTransactionService
    {
        int AddTransaction(DepositTransaction transaction);
        DepositTransaction GetTransactionById(int transactionId);
        IList<DepositTransaction> GetTransactions();
        void SetTransaction(DepositTransaction transaction);
        void VoidTransaction(int transactionId);
    }
}