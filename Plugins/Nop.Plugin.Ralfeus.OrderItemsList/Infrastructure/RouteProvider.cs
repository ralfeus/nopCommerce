using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Ralfeus.OrderItemsList.Controllers;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapRoute(
                name: "Plugin.Ralfeus.OrderItemsList.Admin",
                url: "Admin/OrderItems/{action}/{id}",
                defaults: new { controller = "OrderItem", action = "List", id = UrlParameter.Optional , area = "Admin"},
                namespaces: new[] {$"{typeof(OrderItemController).Namespace}.Controllers"}
            );
            route.DataTokens.Add("area", "admin");
            routes.Remove(route);
            routes.Insert(0, route);
        }

        public int Priority => 0;
    }
}