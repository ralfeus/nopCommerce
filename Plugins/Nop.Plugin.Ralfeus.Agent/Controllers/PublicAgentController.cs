using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Ralfeus.Agent.Domain;
using Nop.Plugin.Ralfeus.Agent.Models;
using Nop.Plugin.Ralfeus.Agent.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Ralfeus.Agent.Controllers
{
    public class PublicAgentController : BasePluginController
    {
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IAgentOrderService _agentOrderService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;

        public PublicAgentController(IProductService productService, ISettingService settingService,
            IProductAttributeService productAttributeService, IWorkContext workContext, IAgentOrderService agentOrderService,
            ICurrencyService currencyService, IPriceFormatter priceFormatter, IOrderService orderService)
        {
            _productService = productService;
            _settingService = settingService;
            _productAttributeService = productAttributeService;
            _workContext = workContext;
            _agentOrderService = agentOrderService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _orderService = orderService;
        }

        public ActionResult AccountInfoLink()
        {
            return this._workContext.CurrentCustomer.IsRegistered()
                ? View("~/Plugins/Ralfeus.Agent/Views/PublicAgent/AccountInfoLink.cshtml")
                : null;
        }

        public ActionResult HeaderMenuLink()
        {
            return _workContext.CurrentCustomer.IsRegistered()
                ? View("~/Plugins/Ralfeus.Agent/Views/PublicAgent/HeaderMenuLink.cshtml")
                : null;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult NewOrder(AgentOrderModel model)
        {
            if (model == null)
            {
                model = new AgentOrderModel();
            }
            return View("~/Plugins/Ralfeus.Agent/Views/PublicAgent/NewOrder.cshtml", model);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AddToCart(AgentOrderModel model)
        {
            var settings = this._settingService.LoadSetting<AgentSettings>();
            var productId = settings.OrderAgentProductId;


            var form = new FormCollection
            {
//                {$"addtocart_{productId}.CustomerEnteredPrice", model.Price.ToString(CultureInfo.InvariantCulture)},
                {$"addtocart_{productId}.EnteredQuantity", model.Quantity.ToString()},
                {$"product_attribute_{settings.ItemUrlAttributeMappingId}", model.ItemUrl},
                {$"product_attribute_{settings.SourceShopNameAttributeMappingId}", model.SourceShopName},
                {$"product_attribute_{settings.ImagePathAttributeMappingId}", model.ImagePath},
                {$"product_attribute_{settings.ItemNameAttributeMappingId}", model.ItemName},
                {$"product_attribute_{settings.ColorAttributeMappingId}", model.Color},
                {$"product_attribute_{settings.SizeAttributeMappingId}", model.Size},
                {$"product_attribute_{settings.PriceAttributeMappingId}", model.Price.ToString(CultureInfo.InvariantCulture)},
                {$"product_attribute_{settings.CommentAttributeMappingId}", model.Comment}
            };

            var shoppingCartController = DependencyResolver.Current.GetService<ShoppingCartController>();
            shoppingCartController.ControllerContext = new ControllerContext(this.ControllerContext.RequestContext, shoppingCartController);
            shoppingCartController.RouteData.Values["controller"] = "ShoppingCart";
            shoppingCartController.RouteData.Values["action"] = "AddProductToCart_Details";
            shoppingCartController.Url = this.Url;
            return shoppingCartController.AddProductToCart_Details(productId, (int) ShoppingCartType.ShoppingCart, form);
        }

        public ActionResult AgentOrders()
        {
            var model = new PublicAgentModel();
            return View("~/Plugins/Ralfeus.Agent/Views/PublicAgent/AgentOrders.cshtml", model);
        }

        [HttpPost]
        public ActionResult AgentList()
        {
            var gridModel = new DataSourceResult
            {
                Data = this._agentOrderService.GetAgentOrders(this._workContext.CurrentCustomer).Select(ao => new AgentOrderModel
                {
                    Id = ao.Id,
                    OrderItemId =  ao.OrderItemId,
                    PriceFormated = this._priceFormatter.FormatPrice(
                        this._currencyService.ConvertFromPrimaryStoreCurrency(ao.Price, this._workContext.WorkingCurrency)),
                    Color = ao.Color,
                    Comment = ao.Comment,
//                    CustomerId = ao.CustomerId,
//                    CustomerName = this._customerService.GetCustomerById(ao.CustomerId).GetFullName(),
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
                Status = (AgentOrderStatus) model.StatusId
            };
            updatedAgentOrder = this._agentOrderService.SetAgentOrder(updatedAgentOrder);
            var result = new JsonResult {Data = new {UpdateResult = "No action"}};
            var underlyingOrder = this._orderService.GetOrderById(updatedAgentOrder.OrderId);
            if (underlyingOrder.PaymentStatus == PaymentStatus.Pending)
            {
                result.Data = new
                {
                    UpdateResult = "To pay",
                    OrderId = underlyingOrder.Id
                };
            }
            return result;
        }
    }
}