using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Deposit.Domain;
using Nop.Plugin.Payments.Deposit.Extensions;
using Nop.Services.Customers;

namespace Nop.Plugin.Payments.Deposit.Services
{
    public class DepositTransactionService : IDepositTransactionService
    {
        private readonly IRepository<DepositTransaction> _repository;
        private readonly ICustomerService _customerService;

        public DepositTransactionService(IRepository<DepositTransaction> repository, ICustomerService customerService)
        {
            this._repository = repository;
            _customerService = customerService;
        }
        public int AddTransaction(DepositTransaction transaction)
        {
            var customer = this._customerService.GetCustomerById(transaction.CustomerId);
            if (transaction.Status == PaymentStatus.Paid)
            {
                customer.AddDeposit(transaction.TransactionAmount);
                transaction.NewBalance = customer.GetDepositBalance();
            }
            this._repository.Insert(transaction);
            return transaction.Id;

        }

        public void DeleteTransaction(int transactionId)
        {
            var transaction = this._repository.GetById(transactionId);
            if (transaction.Status == PaymentStatus.Paid)
            {
                var customer = this._customerService.GetCustomerById(transaction.CustomerId);
                customer.SubtractDeposit(transaction.TransactionAmount);
            }
            this._repository.Delete(transaction);
        }

        public DepositTransaction GetTransactionById(int transactionId)
        {
            return this._repository.Table.FirstOrDefault(dt => dt.Id == transactionId);
        }

        public IList<DepositTransaction> GetTransactions()
        {
            return this.GetTransactions(0);
        }

        public void SetTransaction(DepositTransaction transaction)
        {
            var existingTransaction = this.GetTransactionById(transaction.Id);
            var oldStatus = existingTransaction.Status;
            // update existing transaction properties
            existingTransaction.Status = transaction.Status;

            if ((oldStatus != PaymentStatus.Paid) && (transaction.Status == PaymentStatus.Paid))
            {
                var customer = this._customerService.GetCustomerById(existingTransaction.CustomerId);
                customer.AddDeposit(existingTransaction.TransactionAmount);
                transaction.NewBalance = customer.GetDepositBalance();
            }
            this._repository.Update(existingTransaction);
       }

        public IList<DepositTransaction> GetTransactions(int customerId)
        {
            var query = this._repository.Table;
            if (customerId != 0)
            {
                query = query.Where(dt => dt.CustomerId == customerId);
            }
            return query.ToList();
        }

        public void VoidTransaction(int transactionId)
        {
            var transaction = this._repository.GetById(transactionId);
            if (transaction.Status == PaymentStatus.Paid)
            {
                var customer = this._customerService.GetCustomerById(transaction.CustomerId);
                customer.SubtractDeposit(transaction.TransactionAmount);
                transaction.NewBalance = customer.GetDepositBalance();
            }
            transaction.Status = PaymentStatus.Voided;
            this._repository.Update(transaction);
        }
    }
}