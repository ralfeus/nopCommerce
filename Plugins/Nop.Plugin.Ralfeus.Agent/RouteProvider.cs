using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Ralfeus.Agent
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapRoute("Ralfeus.Agent.Admin.Orders",
                "admin/agent",
                new {controller = "AdminAgent", action = "List", area = "Admin"},
                new[] {"Nop.Plugin.Ralfeus.Agent.Controllers"}
            );
            route.DataTokens.Add("area", "admin");
            routes.Remove(route);
            routes.Insert(0, route);

            routes.MapRoute("Ralfeus.Agent.Public.NewOrder",
                "agent/newOrder",
                new { controller = "PublicAgent", action = "NewOrder" },
                new[] { "Nop.Plugin.Ralfeus.Agent.Controllers" }
            );

            routes.MapRoute("Ralfeus.Agent.AddOrderToCart",
                "agent/newOrder/addToCart",
                new {controller = "PublicAgent", action = "AddToCart"},
                new[] {"Nop.Plugin.Ralfeus.Agent.Controllers"}
            );

            routes.MapRoute("Ralfeus.Agent.Public.Orders",
                "customer/agent",
                new { controller = "PublicAgent", action = "List" },
                new[] { "Nop.Plugin.Ralfeus.Agent.Controllers" }
            );
        }

        public int Priority => 0;
    }
}