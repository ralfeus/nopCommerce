using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Plugin.Ralfeus.OrderItemsList.Domain;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Models
{
    public partial class OrderModel
    {
        public partial class OrderItemModel : Admin.Models.Orders.OrderModel.OrderItemModel
        {
            [NopResourceDisplayName("Admin.Orders.Fields.CustomerEmail")]
            public string CustomerEmail { get; set; }
            
            [NopResourceDisplayName("Ralfeus.OrderItems.Customer")]
            public string CustomerName { get; set; }
            
            public int CustomerId { get; set; }

            [NopResourceDisplayName("Admin.Orders.Fields.Store")]
            public string StoreName { get; set; }

            [NopResourceDisplayName("Ralfeus.OrderItems.Status")]
            public int OrderItemStatusId { get; set; }

            [NopResourceDisplayName("Ralfeus.OrderItems.Status")]
            public string OrderItemStatus { get; set; }

            [NopResourceDisplayName("Ralfeus.OrderItems.OrderId")]
            public int OrderId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Fields.Price")]
            public decimal Price { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Fields.Weight")]
            public decimal Weight { get; set; }

            [NopResourceDisplayName("Ralfeus.OrderItems.PrivateComment")]
            public string PrivateComment { get; set; }
            
            [NopResourceDisplayName("Ralfeus.OrderItems.PublicComment")]
            public string PublicComment { get; set; }     
        }
    }
}