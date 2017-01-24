using Nop.Web.Framework.Themes;

namespace Nop.Plugin.Payments.Deposit.ViewEngines
{
    public class CustomViewEngine : ThemeableRazorViewEngine
    {
        public CustomViewEngine()
        {
            ViewLocationFormats = new[] {"~/Plugins/Payments.Deposit/Views/{0}.cshtml"};
            PartialViewLocationFormats = new[] {"~/Plugins/Payments.Deposit/Views/{0}.cshtml"};
        }
    }
}