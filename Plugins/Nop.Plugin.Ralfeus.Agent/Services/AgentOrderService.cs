using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Ralfeus.Agent.Domain;
using Nop.Plugin.Ralfeus.Agent.Infrastructure;
using Nop.Services.Orders;

namespace Nop.Plugin.Ralfeus.Agent.Services
{
    public class AgentOrderService : IAgentOrderService
    {
        private readonly IRepository<AgentOrder> _repository;
        private readonly IOrderService _orderService;
        private readonly AgentOrderStatus[] _agentOrderFinalStatuses = {AgentOrderStatus.Accepted, AgentOrderStatus.Cancelled, AgentOrderStatus.Complete, AgentOrderStatus.Rejected};

        public AgentOrderService(IRepository<AgentOrder> repository, IOrderService orderService)
        {
            _repository = repository;
            _orderService = orderService;
        }

        public IPagedList<AgentOrder> GetAgentOrders(Customer customer = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var filterCustomerId = customer == null ? 0 : customer.Id;
            return new PagedList<AgentOrder>(
                this._repository.Table
                    .Where(ao => filterCustomerId == 0 || ao.CustomerId == filterCustomerId)
                    .OrderByDescending(ao => ao.Id), pageIndex, pageSize);
        }

        public AgentOrder SetAgentOrder(AgentOrder agentOrder)
        {
            var existingAgentOrder = this._repository.Table.FirstOrDefault(ao => ao.Id == agentOrder.Id);
            if (existingAgentOrder != null)
            {
                if (agentOrder.Price != 0) existingAgentOrder.Price = agentOrder.Price;
                // change underlying order payment status if necessary
                if (existingAgentOrder.Status.In(this._agentOrderFinalStatuses) &&
                    !agentOrder.Status.In(this._agentOrderFinalStatuses))
                {
                    var underlyingOrder = this._orderService.GetOrderById(existingAgentOrder.OrderId);
                    if (underlyingOrder.PaymentStatus == PaymentStatus.Pending)
                    {
                        underlyingOrder.PaymentStatus = PaymentStatus.Paid;
                    }
                }
                // subtract previously accepted agent order price from understalying order total if Accepted is changed
                if (existingAgentOrder.Status == AgentOrderStatus.Accepted && agentOrder.Status != AgentOrderStatus.Accepted)
                {
                    var orderItem = this._orderService.GetOrderItemById(existingAgentOrder.OrderItemId);
                    //update order totals
                    var underlyingOrder = orderItem.Order;
                    underlyingOrder.OrderSubtotalExclTax -= orderItem.PriceExclTax;
                    underlyingOrder.OrderSubtotalInclTax -= orderItem.PriceInclTax;
                    underlyingOrder.OrderTotal -= orderItem.PriceExclTax;
                    // update order item price
                    orderItem.UnitPriceExclTax = 0;
                    orderItem.UnitPriceInclTax = 0;
                    orderItem.PriceExclTax = 0;
                    orderItem.PriceInclTax = 0;
                }

                existingAgentOrder.Status = agentOrder.Status;
                if (agentOrder.Status == AgentOrderStatus.Accepted)
                {
                    AcceptAgentOrder(existingAgentOrder);
                } else if (agentOrder.Status.In(this._agentOrderFinalStatuses))
                {
                    FinishAgentOrder(existingAgentOrder);
                }

                this._repository.Update(existingAgentOrder);
                return existingAgentOrder;
            }
            else
            {
                this._repository.Insert(agentOrder);
                return agentOrder;
            }
        }

        private void FinishAgentOrder(AgentOrder agentOrder)
        {
            var underlyingOrder = this._orderService.GetOrderById(agentOrder.OrderId);
            var orders = this._repository.Table
                .Where(ao => ao.OrderId == agentOrder.OrderId)
                .ToList();
            if (orders.All(ao => ao.Status.In(this._agentOrderFinalStatuses)) &&
                orders.Any(ao => ao.Status == AgentOrderStatus.Accepted))
            {
                underlyingOrder.PaymentStatus = PaymentStatus.Pending;
            }
            this._orderService.UpdateOrder(underlyingOrder);
        }

        private void AcceptAgentOrder(AgentOrder agentOrder)
        {
            var orderItem = this._orderService.GetOrderItemById(agentOrder.OrderItemId);
            // update order item price
            orderItem.UnitPriceExclTax = agentOrder.Price;
            orderItem.UnitPriceInclTax = orderItem.UnitPriceExclTax;
            orderItem.PriceExclTax = agentOrder.Price * agentOrder.Quantity;
            orderItem.PriceInclTax = orderItem.PriceExclTax;
            //update order totals
            var underlyingOrder = orderItem.Order;
            underlyingOrder.OrderSubtotalExclTax += orderItem.PriceExclTax;
            underlyingOrder.OrderSubtotalInclTax += orderItem.PriceInclTax;
            underlyingOrder.OrderTotal += orderItem.PriceExclTax;

            this.FinishAgentOrder(agentOrder);
       }

        public void AddAgentOrder(AgentOrder agentOrder)
        {
            this._repository.Insert(agentOrder);
        }

        public void AddAgentOrders(IEnumerable<AgentOrder> agentOrders)
        {
            this._repository.Insert(agentOrders);
        }
    }
}