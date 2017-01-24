using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Deposit.Domain;
using Nop.Plugin.Payments.Deposit.Models;
using Nop.Plugin.Payments.Deposit.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit.Controllers
{
    public class AdminDepositController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IDepositTransactionService _depositTransactionService;
        private readonly ICustomerService _customerService;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;

        public AdminDepositController(IPermissionService permissionService, IDepositTransactionService depositTransactionService,
            ICustomerService customerService, IPaymentService paymentService, ILocalizationService localizationService,
            IWorkContext workContext, IPriceFormatter priceFormatter)
        {
            _permissionService = permissionService;
            _depositTransactionService = depositTransactionService;
            _customerService = customerService;
            _paymentService = paymentService;
            _localizationService = localizationService;
            _workContext = workContext;
            _priceFormatter = priceFormatter;
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new AdminDepositModel
            {
//                Transactions = this._depositTransactionService.GetTransactions().Select(dt => new DepositTransactionModel
//                {
//                   CustomerId = dt.CustomerId,
//                   CustomerName =  this._customerService.GetCustomerById(dt.CustomerId).GetFullName(),
//                   PaymentMethodName = this._paymentService
//                       .LoadActivePaymentMethods()
//                       .FirstOrDefault(pm => pm.PluginDescriptor.SystemName == dt.PaymentMethodSystemName)
//                       .GetLocalizedFriendlyName(this._localizationService, _workContext.WorkingLanguage.Id),
//                   TransactionAmount =  dt.TransactionAmount,
//                   TransactionTime = dt.TransactionTime,
//                   Status = dt.Status
//                }).ToList()
                Customers = this._customerService.GetAllCustomers().Where(c => !c.IsSystemAccount).ToList()
            };
            return View("~/Plugins/Payments.Deposit/Views/AdminDeposit/List.cshtml", model);
        }

        [HttpPost]
        public ActionResult DepositList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var gridModel = new DataSourceResult
            {
                Data = this._depositTransactionService.GetTransactions().Select(dt => new DepositTransactionModel
                {
                    CustomerId = dt.CustomerId,
                    CustomerName =  this._customerService.GetCustomerById(dt.CustomerId).GetFullName(),
//                    PaymentMethodName = this._paymentService
//                        .LoadActivePaymentMethods()
//                        .FirstOrDefault(pm => pm.PluginDescriptor.SystemName == dt.PaymentMethodSystemName)
//                        .GetLocalizedFriendlyName(this._localizationService, _workContext.WorkingLanguage.Id),
                    TransactionAmount =  dt.TransactionAmount,
                    TransactionTime = dt.TransactionTime,
                    StatusId = (int)dt.Status,
                    NewBalance = this._priceFormatter.FormatPrice(dt.NewBalance)
                })
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult AddDeposit(DepositTransactionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            if (!ModelState.IsValid) return new NullJsonResult();

            var transaction = new DepositTransaction
            {
                AddedBy = this._workContext.CurrentCustomer.Id,
                CustomerId = model.CustomerId,
                PaymentMethodSystemName = model.PaymentMethodName,
                Status = (PaymentStatus)model.StatusId,
                TransactionAmount = model.TransactionAmount,
                TransactionTime = model.TransactionTime
            };
            this._depositTransactionService.AddTransaction(transaction);

            return new NullJsonResult();
        }
    }
}