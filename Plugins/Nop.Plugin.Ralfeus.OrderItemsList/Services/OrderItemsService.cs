using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Ralfeus.OrderItemsList.Domain;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        private readonly IRepository<OrderItem2> _orderItemRepository;
        private readonly IOrderService _orderService;

        public OrderItemsService(IRepository<OrderItem2> orderItemRepository, IOrderService orderService)
        {
            this._orderItemRepository = orderItemRepository;
            this._orderService = orderService;
        }

        public OrderItem2 GetOrderItemById(int orderItemId)
        {
            var query = _orderItemRepository.Table;
            var item = query.FirstOrDefault(oi => oi.OrderItemId == orderItemId);
            if (item == null)
            {
                item = new OrderItem2
                {
                    OrderItemId = orderItemId,
                    OrderItemStatus = OrderItemStatus.Pending
                };
                this._orderItemRepository.Insert(item);
            }
            return item;
        }

        /// <summary>
        /// Search order items
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="ois">Order item status; null to load all orders</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="orderItemPrivateComment">Search in order notes. Leave empty to load all records.</param>
        /// <param name="orderItemPublicComment">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>OrderItems</returns>
        public virtual IPagedList<OrderItem2> SearchOrderItems(int storeId = 0,
            int vendorId = 0, int customerId = 0, int productId = 0, 
            OrderItemStatus? ois = null, string billingEmail = null, 
            string orderItemPrivateComment = null, string orderItemPublicComment = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            int? orderItemStatusId = null;
            if (ois.HasValue)
                orderItemStatusId = (int)ois.Value;

            var query = _orderItemRepository.Table;
            if (storeId > 0)
                query = query.Where(oi => this._orderService.GetOrderItemById(oi.OrderItemId).Order.StoreId == storeId);
            if (vendorId > 0)
            {
                query = query.Where(oi => this._orderService.GetOrderItemById(oi.OrderItemId).Product.VendorId == vendorId);
            }
            if (customerId > 0)
                query = query.Where(oi => this._orderService.GetOrderItemById(oi.OrderItemId).Order.CustomerId == customerId);
            if (productId > 0)
            {
                query = query.Where(oi => this._orderService.GetOrderItemById(oi.OrderItemId).Product.Id == productId);
            }
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(
                    oi => this._orderService.GetOrderItemById(oi.OrderItemId).Order.BillingAddress != null 
                    && !String.IsNullOrEmpty(this._orderService.GetOrderItemById(oi.OrderItemId).Order.BillingAddress.Email) 
                    && this._orderService.GetOrderItemById(oi.OrderItemId).Order.BillingAddress.Email.Contains(billingEmail));
            //if (!String.IsNullOrEmpty(orderItemPrivateComment))
            //    query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderItemPrivateComment)));
            //if (!String.IsNullOrEmpty(orderItemPublicComment))
            //    query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderItemPublicComment)));
            query = query.Where(oi => this._orderService.GetOrderItemById(oi.OrderItemId).Order.Deleted);
            query = query.OrderByDescending(oi => this._orderService.GetOrderItemById(oi.OrderItemId).Order.CreatedOnUtc);


            //database layer paging
            return new PagedList<OrderItem2>(query, pageIndex, pageSize);
        }

        public void UpdateOrderItem(OrderItem2 orderItem)
        {
            this._orderItemRepository.Update(orderItem);
        }
    }
}
