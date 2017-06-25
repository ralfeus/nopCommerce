using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Ralfeus.ShoppingCartWeight.Models;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Ralfeus.ShoppingCartWeight.Controllers
{
    public class ShoppingCartWeightController : BasePluginController
    {
        private readonly IWorkContext _workContext;

        public ShoppingCartWeightController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            if (!this._workContext.CurrentCustomer.HasShoppingCartItems)
            {
                return null;
            }
            var model = new ShoppingCartWeightModel
            {
                Weight = this._workContext.CurrentCustomer.ShoppingCartItems.Sum(item => item.Product.Weight * item.Quantity)
            };
            return PartialView(model);
        }
    }
}