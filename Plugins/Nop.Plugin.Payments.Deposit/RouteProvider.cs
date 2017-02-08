using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Payments.Deposit.Controllers;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Deposit
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapRoute(
                name: "Plugin.Payments.Deposit.Admin.Default",
                url: "Admin/AdminDeposit/{action}/{id}",
                defaults: new { controller = "AdminDeposit", action = "List", id = UrlParameter.Optional , area = "Admin"},
                namespaces: new[] {$"{typeof(AdminDepositController).Namespace}.Controllers"}
            );
            route.DataTokens.Add("area", "admin");
            routes.Remove(route);
            routes.Insert(0, route);

            routes.MapRoute(
                name: "Plugin.Payments.Deposit.ChangeCurrency",
                url: "customer/deposit/{action}/{id}",
                defaults: new { controller = "PublicDeposit", action = "PublicInfo", id = UrlParameter.Optional },
                namespaces: new[] { $"{typeof(PublicDepositController).Namespace}.Controllers" }
            );

//            routes.MapRoute("Plugin.Payments.Deposit.CustomerDeposit",
//                "customer/deposit",
//                new { controller = "PublicDeposit", action = "PublicInfo" },
//                new[] { "Nop.Plugin.Payments.Deposit.Controllers" }
//            );
//
//            routes.MapRoute("Plugin.Payments.Deposit.Charge",
//                "customer/deposit/charge/selectMethod",
//                new {controller = "PublicDeposit", action = "Charge"},
//                new[] {"Nop.Plugin.Payments.Deposit.Controllers"}
//            );
//
//            routes.MapRoute("Plugin.Payments.Deposit.ProceedChargePayment",
//                "customer/deposit/charge/proceed",
//                new {controller = "PublicDeposit", action = "ProceedPayment"},
//                new[] {"Nop.Plugin.Payments.Deposit.Controllers"}
//            );

            //PDT
            routes.MapRoute("Plugin.Payments.Deposit.PDTHandler",
                "Plugins/PaymentPayPalStandard/PDTHandler",
                new { controller = "PayPalGenericHandler", action = "PdtHandler" },
                new[] { "Nop.Plugin.Payments.Deposit.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Deposit.ChargeComplete",
                "customer/deposit/charge/complete/{transactionId}",
                new { controller = "PublicDeposit", action = "ChargeComplete", transactionId = UrlParameter.Optional },
                new { transactionId = @"\d+" },
                new[] { "Nop.Plugin.Payments.Deposit.Controllers" });


        }

        public int Priority => 10;
    }
}