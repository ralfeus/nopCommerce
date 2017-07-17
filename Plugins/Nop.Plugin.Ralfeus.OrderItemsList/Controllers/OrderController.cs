using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Security;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Controllers
{
    public partial class OrderController
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            this._orderService = orderService;
        }
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)]
        public ActionResult GetCustomersList()
        {
            var customers = _orderService.SearchOrders()
                .GroupBy(o => o.CustomerId)
                .Select(o => o.First().Customer)
                .Select(c => new
                {
                    CustomerId = c.Id,
                    CustomerName = c.GetFullName()
                });
            return new JsonResult
            {
                Data = new DataSourceResult
                {
                    Data = customers
                }
            };
        }
    }
}