using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderItemListModel : BaseNopModel
    {
        public IList<SelectListItem> AvailableOrderItemStatuses { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public int OrderItemStatusId { get; set; }
        public int ProductId { get; set; }
    }
}