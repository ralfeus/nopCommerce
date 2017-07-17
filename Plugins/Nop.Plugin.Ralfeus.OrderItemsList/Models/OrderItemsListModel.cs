using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Models
{
    public class OrderItemsListModel : BaseNopModel
    {
        public IList<SelectListItem> AvailableCustomers { get; set; }
        public IList<SelectListItem> AvailableOrderItemStatuses { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderStatus")]
        [UIHint("MultiSelect")]
        public List<int> OrderItemStatusIds { get; set; }
        
        [NopResourceDisplayName("Ralfeus.OrderItems.ProductId")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Store")]
        public int StoreId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Ralfeus.OrderItems.PrivateComment")]
        public string PrivateComment { get; set; }

        [NopResourceDisplayName("Ralfeus.OrderItems.PublicComment")]
        public string PublicComment { get; set; }

        [NopResourceDisplayName("Ralfeus.OrderItems.Customer")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Ralfeus.OrderItems.OrderId")]
        public int OrderId { get; set; }
        
        [NopResourceDisplayName("Ralfeus.OrderItems.OrderItemId")]
        public int OrderItemId { get; set; }
        
        public string CurrencyFormat { get; set; }

        public OrderItemsListModel()
        {
            this.OrderItemStatusIds = new List<int>();
        }
    }
}