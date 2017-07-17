using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Ralfeus.OrderItemsList.Services;
using Nop.Services.Orders;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Domain
{
    public class OrderItem2 : BaseEntity
    {
        private readonly OrderItem _orderItem;
        private readonly IOrderService _orderService;

        public int OrderItemId { get; set; }
        public OrderItemStatus OrderItemStatus
        {
            get => (OrderItemStatus)this.OrderItemStatusId;
            set => this.OrderItemStatusId = (int)value;
        }

        public int OrderItemStatusId { get; set; }

        public string PrivateComment { get; set; }
        public string PublicComment { get; set; }

        public virtual int OrderId
        {
            get => this._orderItem.OrderId; 
            set => this._orderItem.OrderId = value;
        }

        public virtual Order Order => this._orderService.GetOrderById(this.OrderId);

        public virtual Product Product {
            get => this._orderItem.Product;
            set => this._orderItem.Product = value;
        }
        //public string CustomerEmail { get { return this.Order.Customer.Email; } }
        
        
    }
}
