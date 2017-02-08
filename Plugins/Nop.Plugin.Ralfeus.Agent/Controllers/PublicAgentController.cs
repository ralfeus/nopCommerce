using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Ralfeus.Agent.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Plugin.Ralfeus.Agent.Controllers
{
    public class PublicAgentController : BasePluginController
    {
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IProductAttributeService _productAttributeService;

        public PublicAgentController(IProductService productService, ISettingService settingService, IProductAttributeService productAttributeService)
        {
            _productService = productService;
            _settingService = settingService;
            _productAttributeService = productAttributeService;
        }

        public ActionResult Link()
        {
            return View("~/Plugins/Ralfeus.Agent/Views/PublicAgent/Link.cshtml");
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
            var orderText = new StringBuilder()
                .AppendLine($"Item URL: {model.ProductUrl}")
                .AppendLine($"Source shop URL: {model.SourceShopName}")
                .AppendLine($"Image path: {model.ImagePath}")
                .AppendLine($"Item name: {model.ItemName}")
                .AppendLine($"Color: {model.Color}")
                .AppendLine($"Size: {model.Size}")
                .AppendLine($"Approximate price: {model.ApproxPrice}")
                .AppendLine($"Quantity: {model.Quantity}")
                .AppendLine($"Comment: {model.Comment}");

            var form = new FormCollection
            {
                {$"addtocart_{productId}.CustomerEnteredPrice", model.ApproxPrice.ToString(CultureInfo.InvariantCulture)},
                {$"addtocart_{productId}.EnteredQuantity", model.Quantity.ToString()},
                {$"product_attribute_{settings.ProductAttributeMappingId}", orderText.ToString()}
            };

            var shoppingCartController = DependencyResolver.Current.GetService<ShoppingCartController>();
            shoppingCartController.ControllerContext = new ControllerContext(this.ControllerContext.RequestContext, shoppingCartController);
            shoppingCartController.RouteData.Values["controller"] = "ShoppingCart";
            shoppingCartController.RouteData.Values["action"] = "AddProductToCart_Details";
            shoppingCartController.Url = this.Url;
            return shoppingCartController.AddProductToCart_Details(productId, (int) ShoppingCartType.ShoppingCart, form);
        }
    }
}