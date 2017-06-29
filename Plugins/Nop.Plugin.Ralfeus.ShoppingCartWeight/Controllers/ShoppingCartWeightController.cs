using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Plugins;
using Nop.Plugin.Ralfeus.ShoppingCartWeight.Models;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Ralfeus.ShoppingCartWeight.Controllers
{
    public class ShoppingCartWeightController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IProductService _productService;

        public ShoppingCartWeightController(IWorkContext workContext, IMeasureService measureService, MeasureSettings measureSettings, IProductService productService)
        {
            _workContext = workContext;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _productService = productService;
        }

        [ChildActionOnly]
        public ActionResult Cart()
        {
            if (!this._workContext.CurrentCustomer.HasShoppingCartItems)
            {
                return null;
            }
            var primaryWeight = this._measureService.GetMeasureWeightById(this._measureSettings.BaseWeightId);
            var model = new ShoppingCartWeightModel
            {
                Weight =
                    $"{this._workContext.CurrentCustomer.ShoppingCartItems.Sum(item => item.Product.Weight * item.Quantity):N0} {primaryWeight.Name}"
            };
            return PartialView("~/Plugins/Ralfeus.ShoppingCartWeight/Views/ShoppingCartWeight/Cart.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult ProductDetails(int productId)
        {
            var primaryWeight = this._measureService.GetMeasureWeightById(this._measureSettings.BaseWeightId);
            var product = _productService.GetProductById(productId);
            var model = new ShoppingCartWeightModel
            {
                Weight =
                    $"{product.Weight:N0} {primaryWeight.Name}"
            };
            return PartialView("~/Plugins/Ralfeus.ShoppingCartWeight/Views/ShoppingCartWeight/ProductDetails.cshtml", model);
        }
    }
}