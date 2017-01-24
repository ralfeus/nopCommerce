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
        public void AddTransaction(DepositTransaction transaction)
        {
            this._repository.Insert(transaction);
            var customer = this._customerService.GetCustomerById(transaction.CustomerId);
            customer.AddDeposit(transaction.TransactionAmount);
        }

        public void DeleteTransaction(int transactionId)
        {
            var transaction = this._repository.GetById(transactionId);
            this._repository.Delete(transaction);
            var customer = this._customerService.GetCustomerById(transaction.CustomerId);
            customer.AddDeposit(transaction.TransactionAmount);
        }

        public IList<DepositTransaction> GetTransactions()
        {
            return this.GetTransactions(0);
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
            transaction.Status = PaymentStatus.Voided;
            this._repository.Update(transaction);
            var customer = this._customerService.GetCustomerById(transaction.CustomerId);
            customer.AddDeposit(transaction.TransactionAmount);
        }
    }
}