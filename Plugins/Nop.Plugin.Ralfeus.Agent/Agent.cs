using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FluentValidation.Validators;
using Nop.Core.Domain.Catalog;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Ralfeus.Agent
{
    public class Agent : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly IProductService _productService;
        private readonly IProductAttributeService _attributeService;
        private readonly ISettingService _settingService;

        public Agent(IProductService productService, IProductAttributeService attributeService, ISettingService settingService)
        {
            _productService = productService;
            _attributeService = attributeService;
            _settingService = settingService;
        }

        #region IWidgetPlugin

        public IList<string> GetWidgetZones()
        {
            return new[] {"header_links_before"};
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            controllerName = "PublicAgent";
            actionName = "Link";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Ralfeus.Agent.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAdminPlugin

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Ralfeus.Agent",
                Title = "Order Agent",
                ControllerName = "AdminAgent",
                ActionName = "List",
                Visible = true,
//                Url = "/admin/agent",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if(pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        #endregion

        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent", "Order Agent");
            //Ralfeus.Agent.NewOrder
            //Ralfeus.Agent.Order.ProductUrl
            //Ralfeus.Agent.ProductUrlRequired
            this.InstallSupportingEntities();
            base.Install();
        }

        private void InstallSupportingEntities()
        {
            #region Product

            var orderAgentProduct = new Product
            {
                Name = "Order Agent Product",
                CustomerEntersPrice = true,
                IsShipEnabled = true,
                MaximumCustomerEnteredPrice = int.MaxValue,
                MinimumCustomerEnteredPrice = 0,
                OrderMaximumQuantity = int.MaxValue,
                ProductType = ProductType.SimpleProduct,
                Published = true,
                ShortDescription = "Used to create agent order",
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            var attribute = this._attributeService.GetAllProductAttributes()
                .FirstOrDefault(pa => pa.Name == "Custom Text");
            if (attribute == null) {
                throw new InvalidOperationException(
                    "The plugin can't be installed as product attribute <Custom Text> it depends on doesn't exist");
            }
            orderAgentProduct.ProductAttributeMappings.Add(new ProductAttributeMapping
            {
                ProductAttribute = attribute,
                Product = orderAgentProduct,
                AttributeControlType = AttributeControlType.TextBox
            });
            this._productService.InsertProduct(orderAgentProduct);

            #endregion

            #region Product attribute

            var productAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                .First(pa => pa.ProductAttributeId == attribute.Id)
                .Id;

            #endregion

            var settings = new AgentSettings
            {
                OrderAgentProductId = orderAgentProduct.Id,
                ProductAttributeMappingId = productAttributeMappingId
            };
            this._settingService.SaveSetting(settings);


        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Ralfeus.Agent");

            var settings = this._settingService.LoadSetting<AgentSettings>();
            var orderAgentProduct = this._productService.GetProductById(settings.OrderAgentProductId);
            this._productService.DeleteProduct(orderAgentProduct);

            this._settingService.DeleteSetting<AgentSettings>();
            base.Uninstall();
        }
    }
}