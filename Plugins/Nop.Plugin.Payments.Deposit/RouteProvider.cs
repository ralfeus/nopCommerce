using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Deposit
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapRoute("Plugin.Payments.Deposit.Admin.List",
                "admin/deposit",
                new {controller = "AdminDeposit", action = "List", area = "Admin"},
                new[] {"Nop.Plugin.Payments.Deposit.Controllers"}
            );
            route.DataTokens.Add("area", "admin");
            routes.Remove(route);
            routes.Insert(0, route);

//            System.Web.Mvc.ViewEngines.Engines.Insert(0, new CustomViewEngine());

            routes.MapRoute("Plugin.Payments.Deposit.ChangeCurrency",
                "customer/deposit/changeCurrency",
                new { controller = "PublicDeposit", action = "ChangeCurrency" },
                new[] { "Nop.Plugin.Payments.Deposit.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Deposit.CustomerDeposit",
                "customer/deposit",
                new { controller = "PublicDeposit", action = "PublicInfo" },
                new[] { "Nop.Plugin.Payments.Deposit.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Deposit.Charge",
                "customer/deposit/charge/selectMethod",
                new {controller = "PublicDeposit", action = "Charge"},
                new[] {"Nop.Plugin.Payments.Deposit.Controllers"}
            );

            routes.MapRoute("Plugin.Payments.Deposit.ProceedChargePayment",
                "customer/deposit/charge/proceed",
                new {controller = "PublicDeposit", action = "ProceedPayment"},
                new[] {"Nop.Plugin.Payments.Deposit.Controllers"}
            );

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