using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Ralfeus.OrderItemsList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Services
{
    public interface IOrderItemsService
    {
        /// <summary>
        /// Gets an order item extension by given order item ID
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <returns>Order item extension instance</returns>
        OrderItem2 GetOrderItemById(int orderItemId);
        
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
        IPagedList<OrderItem2> SearchOrderItems(int storeId = 0,
        int vendorId = 0, int customerId = 0, int productId = 0,
            OrderItemStatus? ois = null, string billingEmail = null,
            string orderItemPrivateComment = null, string orderItemPublicComment = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        void UpdateOrderItem(OrderItem2 orderItem);
    }
}
