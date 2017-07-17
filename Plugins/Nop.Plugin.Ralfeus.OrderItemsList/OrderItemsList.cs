using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Services.Localization;

namespace Nop.Plugin.Ralfeus.OrderItemsList
{
    public class OrderItemsList : BasePlugin, IAdminMenuPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public OrderItemsList(ILocalizationService localizationService, ILanguageService languageService)
        {
            _localizationService = localizationService;
            _languageService = languageService;
        }

        #region IAdminPlugin

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Ralfeus.OrderItemsList",
                Title = "Order Items",
//                ControllerName = "OrderItems",
//                ActionName = "List",
                Visible = true,
                Url = "/Admin/OrderItems",
                RouteValues = new RouteValueDictionary() { { "area", "admin" } },
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        #endregion

        #region BasePlugin

        public override void Install()
        {
            base.Install();
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Actions", "Actions");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Comments", "Comments");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Customer", "Customer");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Customer.Hint", "Choose customer to filter by");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Description", "Item description");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Ids", "IDs");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.OrderItemId", "Order item ID");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.OrderItemId.Hint", "ID of the order item. Use it to find specific order item");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.OrderId", "Order ID");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.OrderId.Hint", "ID of the order, containing this order item. Use it to find all items of single order");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Price", "Price");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.PrivateComment", "Private comment");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.PrivateComment.Hint", "Private comment, visible to admin only");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.ProductId", "Product ID");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.ProductId.Hint", "ID of the ordered product");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.PublicComment", "Public comment");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.PublicComment.Hint", "Public comment, visible to admin and the customer");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Vendor", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Quantity", "Quantity");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Status", "Order item status");
            this.AddOrUpdatePluginLocaleResource("Ralfeus.OrderItems.Weight", "Weight");
        }

        public override void Uninstall()
        {
            var languages = this._languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                var resources = this._localizationService.GetAllResources(language.Id)
                    .Where(r => r.ResourceName.StartsWith("Ralfeus.OrderItems"));
                foreach (var res in resources)
                {
                    this.DeletePluginLocaleResource(res.ResourceName);                    
                }
            }
//            this._settingService.DeleteSetting<OrderItemsSe>();
            base.Uninstall();
        }

        #endregion

    }
}
