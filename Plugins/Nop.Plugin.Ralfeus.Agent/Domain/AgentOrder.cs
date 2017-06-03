using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Ralfeus.Agent.Domain
{
    public class AgentOrder : BaseEntity
    {
        public string ItemUrl { get; set; }
        public string ImagePath { get; set; }
        public string ItemName { get; set; }
        public string SourceShopName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        public AgentOrderStatus Status { get; set; }
    }
}