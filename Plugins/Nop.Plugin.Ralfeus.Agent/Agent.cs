using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
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
            return new[] {"header_links_before", "account_navigation_after"};
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            switch (widgetZone)
            {
                case "account_navigation_after":
                    actionName = "AccountInfoLink";
                    break;
                case "header_links_before":
                    actionName = "HeaderMenuLink";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{widgetZone} isn't supported by this plugin");
            }
            controllerName = "PublicAgent";
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
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.NewOrder", "New agent order");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.Id", "Order ID");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.ItemUrl", "Item URL");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.ProductUrlRequired", "The item URL is required");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.OrderId", "Containing order ID");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.Customer", "Customer");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.ItemName", "Item name");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.Price", "Price");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.Quantity", "Quantity");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.Status", "Status");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.Agent.Order.Comment", "Comment");
            this.InstallSupportingEntities();
            base.Install();
        }

        private void InstallSupportingEntities()
        {
            #region Product

            var orderAgentProduct = new Product
            {
                Name = "Order Agent Product",
                CustomerEntersPrice = false,
                IsShipEnabled = true,
//                MaximumCustomerEnteredPrice = int.MaxValue,
//                MinimumCustomerEnteredPrice = 0,
                OrderMaximumQuantity = int.MaxValue,
                ProductType = ProductType.SimpleProduct,
                Published = true,
                ShortDescription = "Used to create agent order",
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            #region Product attributes

            var productAttributes = new Dictionary<string, ProductAttribute>
            {
                {"ItemURL", new ProductAttribute {Name = "Agent: Item URL"}},
                {"SourceShopName", new ProductAttribute {Name = "Agent: Source shop name"}},
                {"ImagePath", new ProductAttribute {Name = "Agent: Image path"}},
                {"ItemName", new ProductAttribute {Name = "Agent: Item name"}},
                {"Color", new ProductAttribute {Name = "Agent: Color"}},
                {"Size", new ProductAttribute {Name = "Agent: Size"}},
                {"Price", new ProductAttribute {Name = "Agent: Approximate price"}},
                {"Comment", new ProductAttribute {Name = "Agent: Comment"}}
            };
            foreach (var productAttribute in productAttributes.Values)
            {
                this._attributeService.InsertProductAttribute(productAttribute);
                orderAgentProduct.ProductAttributeMappings.Add(new ProductAttributeMapping
                {
                    ProductAttribute = productAttribute,
                    Product = orderAgentProduct,
                    AttributeControlType = AttributeControlType.TextBox
                });
            }

            #endregion

            this._productService.InsertProduct(orderAgentProduct);

            #endregion

            var settings = new AgentSettings
            {
                OrderAgentProductId = orderAgentProduct.Id,
                ItemUrlAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["ItemURL"].Id).Id,
                SourceShopNameAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["SourceShopName"].Id).Id,
                ImagePathAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["ImagePath"].Id).Id,
                ItemNameAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["ItemName"].Id).Id,
                ColorAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["Color"].Id).Id,
                SizeAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["Size"].Id).Id,
                PriceAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["Price"].Id).Id,
                CommentAttributeMappingId = orderAgentProduct.ProductAttributeMappings
                    .First(pa => pa.ProductAttributeId == productAttributes["Comment"].Id).Id
            };
            this._settingService.SaveSetting(settings);


        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Ralfeus.Agent");
            this.DeletePluginLocaleResource("Ralfeus.Agent.NewOrder");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.ItemUrl");
            this.DeletePluginLocaleResource("Ralfeus.Agent.ProductUrlRequired");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.OrderId");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.Customer");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.ItemName");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.Price");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.Quantity");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.Status");
            this.DeletePluginLocaleResource("Ralfeus.Agent.Order.Comment");

            var settings = this._settingService.LoadSetting<AgentSettings>();
            var orderAgentProduct = this._productService.GetProductById(settings.OrderAgentProductId);
            foreach (var attributeMapping in orderAgentProduct.ProductAttributeMappings.ToList())
            {
                var attribute = attributeMapping.ProductAttribute;
                this._attributeService.DeleteProductAttributeMapping(attributeMapping);
                this._attributeService.DeleteProductAttribute(attribute);
            }
            this._productService.DeleteProduct(orderAgentProduct);

            this._settingService.DeleteSetting<AgentSettings>();
            base.Uninstall();
        }
    }
}