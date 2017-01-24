using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;

namespace Nop.Plugin.Payments.Deposit.Extensions
{
    public static class CustomerExtensions
    {
        /// <summary>
        /// Adds money to customer deposit
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <param name="amount">Amount of money in base currency</param>
        public static void AddDeposit(this Customer customer, decimal amount)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var deposit = customer.GetAttribute<decimal>("Deposit", genericAttributeService);
            genericAttributeService.SaveAttribute(customer, "Deposit", deposit + amount);
        }

        /// <summary>
        /// Subtracts money from customer deposit
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <param name="amount">Amount of money in base currency</param>
        public static void SubtractDeposit(this Customer customer, decimal amount)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var deposit = customer.GetAttribute<decimal>("Deposit", genericAttributeService);
            genericAttributeService.SaveAttribute(customer, "Deposit", deposit - amount);
        }

        /// <summary>
        /// Returns actual customer deposit balance
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <returns>Deposit balance in base currency</returns>
        public static decimal GetDepositBalance(this Customer customer)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return customer.GetAttribute<decimal>("Deposit", genericAttributeService);
        }
    }
}