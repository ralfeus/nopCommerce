using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Nop.Services;
using Nop.Web.Framework.Security;
using Nop.Services.Customers;
using Nop.Admin.Controllers;
using Nop.Plugin.Ralfeus.OrderItemsList.Models;
using Nop.Plugin.Ralfeus.OrderItemsList.Domain;
using Nop.Plugin.Ralfeus.OrderItemsList.Services;
using Nop.Web.Framework.Mvc;
using OrderModel = Nop.Plugin.Ralfeus.OrderItemsList.Models.OrderModel;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Controllers
{
    public class OrderItemController : BaseAdminController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderItemsService _orderItemService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public OrderItemController(ICustomerService customerService,
            IOrderService orderService, 
            IOrderReportService orderReportService, 
            IOrderProcessingService orderProcessingService,
            IPriceCalculationService priceCalculationService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IEncryptionService encryptionService,
            IPaymentService paymentService,
            IMeasureService measureService,
            IPdfService pdfService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IProductService productService,
            IExportManager exportManager,
            IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService,
            IProductAttributeService productAttributeService, 
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter, 
            IShoppingCartService shoppingCartService,
            IGiftCardService giftCardService, 
            IDownloadService downloadService,
            IShipmentService shipmentService, 
            IShippingService shippingService,
            IStoreService storeService,
            IVendorService vendorService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAffiliateService affiliateService,
            IPictureService pictureService,
            CurrencySettings currencySettings, 
            TaxSettings taxSettings,
            MeasureSettings measureSettings,
            AddressSettings addressSettings,
            ShippingSettings shippingSettings, IOrderItemsService orderItemService)
		{
		    this._orderService = orderService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._productService = productService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._vendorService = vendorService;
		    _pictureService = pictureService;
		    _priceFormatter = priceFormatter;
		    _orderItemService = orderItemService;
		    _customerService = customerService;
		}
        
        #endregion

        #region Utilities

        [NonAction]
        protected virtual bool HasAccessToOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorProducts = order.OrderItems.Any(orderItem => orderItem.Product.VendorId == vendorId);
            return hasVendorProducts;
        }

        [NonAction]
        protected virtual bool HasAccessToOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return orderItem.Product.VendorId == vendorId;
        }

        [NonAction]
        protected virtual bool HasAccessToProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return product.VendorId == vendorId;
        }

        [NonAction]
        protected virtual bool HasAccessToShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var hasVendorProducts = false;
            var vendorId = _workContext.CurrentVendor.Id;
            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem != null)
                {
                    if (orderItem.Product.VendorId == vendorId)
                    {
                        hasVendorProducts = true;
                        break;
                    }
                }
            }
            return hasVendorProducts;
        }

        #endregion

        #region Order items list

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? orderItemStatusId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new OrderItemsListModel
            {
                AvailableCustomers = new List<SelectListItem>(),
                AvailableOrderItemStatuses = OrderItemStatus.Pending.ToSelectList(false).ToList(),
                AvailableStores = new List<SelectListItem>(),
                AvailableVendors = new List<SelectListItem>()
            };
            
            //customers
            model.AvailableCustomers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var customer in _customerService.GetAllCustomers())
                model.AvailableCustomers.Add(new SelectListItem { Text = customer.GetFullName(), Value = customer.Id.ToString() });
            
            //order statuses
//            model.AvailableOrderItemStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            if (orderItemStatusId.HasValue)
            {
                //pre-select value?
                var item = model.AvailableOrderItemStatuses.FirstOrDefault(x => x.Value == orderItemStatusId.Value.ToString());
                if (item != null)
                    item.Selected = true;
            }

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            model.CurrencyFormat = _workContext.WorkingCurrency.CustomFormatting;

            return View("~/Plugins/Ralfeus.OrderItemsList/Views/OrderItem/List.cshtml", model);
		}

        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)]
        public ActionResult OrderItemsList(OrderItemsListModel model, IEnumerable<Sort> sort = null)//)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orderItemStatusIds = !model.OrderItemStatusIds.Contains(0) ? model.OrderItemStatusIds : null;


            var i = 0;
            var orderItems = this._orderService.SearchOrders(model.StoreId, model.VendorId, model.CustomerId)
                .SelectMany(o => o.OrderItems);
            if (model.OrderId != 0)
            {
                orderItems = orderItems.Where(oi => oi.OrderId == model.OrderId);
            }
            if (model.OrderItemId != 0)
            {
                orderItems = orderItems.Where(oi => oi.Id == model.OrderItemId);
            }
            if (model.OrderItemStatusIds[0] != 0)
            {
                orderItems = orderItems.Where(oi =>
                    model.OrderItemStatusIds.Contains(this._orderItemService.GetOrderItemById(oi.Id)
                        .OrderItemStatusId));
            }
            if (model.ProductId != 0)
            {
                orderItems = orderItems.Where(oi => oi.ProductId == model.ProductId);
            }
            if (model.CustomerId != 0)
            {
                orderItems = orderItems.Where(oi => oi.Order.CustomerId == model.CustomerId);
            }
            if (model.PrivateComment != null)
            {
                orderItems = orderItems.Where(oi =>
                {
                    var oi2 = this._orderItemService.GetOrderItemById(oi.Id);
                    return oi2.PrivateComment != null && oi2.PrivateComment.Contains(model.PrivateComment);
                });
            }
            if (model.PublicComment != null)
            {
                orderItems = orderItems.Where(oi =>
                {
                    var oi2 = this._orderItemService.GetOrderItemById(oi.Id);
                    return oi2.PublicComment != null && oi2.PublicComment.Contains(model.PublicComment);
                });
            }
            
            var filteredOrderItems = orderItems.Select(oi =>
                {
                    var oi2 = this._orderItemService.GetOrderItemById(oi.Id);
//                    var store = _storeService.GetStoreById(oi.Order.StoreId);
                    var vendor = _vendorService.GetVendorById(oi.Product.VendorId);
//                    //picture
                    Debug.Print(i++.ToString());
                    var defaultProductPicture = _pictureService.GetPicturesByProductId(oi.Product.Id, 1).FirstOrDefault();
                    Debug.Print(oi.Product.Price.ToString(CultureInfo.InvariantCulture));
                    Debug.Print(_priceFormatter.FormatPrice(oi.Product.Price));
                    return new OrderModel.OrderItemModel
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        AttributeInfo = oi.AttributeDescription,
                        CustomerEmail = oi.Order.BillingAddress.Email,
                        CustomerName = $"{oi.Order.BillingAddress.FirstName} {oi.Order.BillingAddress.LastName}",
                        CustomerId = oi.Order.CustomerId,
                        OrderItemStatusId = (int)oi2.OrderItemStatus,
                        OrderItemStatus = oi2.OrderItemStatus.GetLocalizedEnum(this._localizationService, this._workContext),
                        PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true),
                        Price = oi.Product.Price,
                        PrivateComment = oi2.PrivateComment ?? "",
                        ProductName = oi.Product.Name,
                        PublicComment = oi2.PublicComment ?? "",
                        Quantity = oi.Quantity,
                        VendorName = vendor != null ? vendor.Name : "Unknown",
                        Weight = oi.Product.Weight
                    };
                })
//                .AsQueryable()
//                .Filter(filter)
//                .Sort(sort);
                .ToList();

            return new JsonResult
            {
                Data = new DataSourceResult
                {
                    Data = filteredOrderItems,
                    //Total = orderItems.TotalCount
                }
            };
        }

        //      [HttpPost, ActionName("List")]
        //      [FormValueRequired("go-to-order-by-number")]
        //      public ActionResult GoToOrderId(OrderListModel model)
        //      {
        //          var order = _orderService.GetOrderById(model.GoDirectlyToNumber);
        //          if (order == null)
        //              return List();

        //          return RedirectToAction("Edit", "Order", new { id = order.Id });
        //      }

          public ActionResult ProductSearchAutoComplete(string term)
          {
              const int searchTermMinimumLength = 3;
              if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                  return Content("");

              //a vendor should have access only to his products
              var vendorId = 0;
              if (_workContext.CurrentVendor != null)
              {
                  vendorId = _workContext.CurrentVendor.Id;
              }

              //products
              const int productNumber = 15;
              var products = _productService.SearchProducts(
                  vendorId: vendorId,
                  keywords: term,
                  pageSize: productNumber,
                  showHidden: true);

              var result = (from p in products
                            select new
                            {
                                label = p.Name,
                                productid = p.Id
                            })
                            .ToList();
              return Json(result, JsonRequestBehavior.AllowGet);
          }

        #endregion

        public ActionResult UpdateOrderItem(OrderModel.OrderItemModel model)
        {
            var orderItem = this._orderItemService.GetOrderItemById(model.Id);
            orderItem.OrderItemStatusId = model.OrderItemStatusId;
            orderItem.PrivateComment = model.PrivateComment;
            orderItem.PublicComment = model.PublicComment;
            this._orderItemService.UpdateOrderItem(orderItem);
            return new NullJsonResult();
        }
    }
}
