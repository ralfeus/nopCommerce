using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Ralfeus.Agent.Controllers;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Ralfeus.Agent
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapRoute(
                name: "Plugin.Ralfeus.Agent.Admin",
                url: "Admin/AdminAgent/{action}/{id}",
                defaults: new { controller = "AdminAgent", action = "List", id = UrlParameter.Optional , area = "Admin"},
                namespaces: new[] {$"{typeof(AdminAgentController).Namespace}.Controllers"}
            );
            route.DataTokens.Add("area", "admin");
            routes.Remove(route);
            routes.Insert(0, route);

            routes.MapRoute(
                name: "Plugin.Ralfeus.Agent",
                url: "agent/{action}/{id}",
                defaults: new { controller = "PublicAgent", action = "NewOrder", id = UrlParameter.Optional },
                namespaces: new[] { $"{typeof(PublicAgentController).Namespace}.Controllers" }
            );
        }

        public int Priority => 0;
    }
}