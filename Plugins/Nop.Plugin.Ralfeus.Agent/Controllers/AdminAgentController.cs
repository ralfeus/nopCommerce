using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using Nop.Plugin.Ralfeus.Agent.Domain;
using Nop.Plugin.Ralfeus.Agent.Models;
using Nop.Plugin.Ralfeus.Agent.Services;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Ralfeus.Agent.Controllers
{
    public class AdminAgentController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IAgentOrderService _agentOrderService;
        private readonly ICustomerService _customerService;

        public AdminAgentController(IPermissionService permissionService, IAgentOrderService agentOrderService, ICustomerService customerService)
        {
            _permissionService = permissionService;
            _agentOrderService = agentOrderService;
            _customerService = customerService;
        }

        public ActionResult List()
        {
            var model = new AdminAgentModel();
            return View("~/Plugins/Ralfeus.Agent/Views/AdminAgent/List.cshtml", model);
        }

        [HttpPost]
        public ActionResult AgentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var gridModel = new DataSourceResult
            {
                Data = this._agentOrderService.GetAgentOrders().ToList().Select(ao => new AgentOrderModel
                {
                    Id = ao.Id,
                    OrderItemId = ao.OrderItemId,
                    Price = ao.Price,
                    Color = ao.Color,
                    Comment = ao.Comment,
//                    CustomerId = ao.CustomerId,
                    CustomerName = this._customerService.GetCustomerById(ao.CustomerId).GetFullName(),
                    ImagePath = ao.ImagePath,
                    ItemName = ao.ItemName,
                    OrderId = ao.OrderId,
                    ItemUrl = ao.ItemUrl,
                    Quantity = ao.Quantity,
                    Size = ao.Size,
                    SourceShopName = ao.SourceShopName,
                    StatusId = (int)ao.Status
                }).OrderByDescending(dt => dt.Id)
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult UpdateAgent(AgentOrderModel model)
        {
            if (!ModelState.IsValid)
            {
                return new NullJsonResult();
            }
            var updatedAgentOrder = new AgentOrder
            {
                Id = model.Id,
                Price = model.Price,
                Status = (AgentOrderStatus) model.StatusId
            };
            this._agentOrderService.SetAgentOrder(updatedAgentOrder);
            return new NullJsonResult();
        }
    }
}