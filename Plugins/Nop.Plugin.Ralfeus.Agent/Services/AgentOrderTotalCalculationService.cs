using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Plugin.Ralfeus.Agent.Services
{
    public class AgentOrderTotalCalculationService : IOrderTotalCalculationService
    {
        private readonly IOrderTotalCalculationService _orderTotalCalculationServiceImplementation;
        private AgentSettings _agentSettings;

        public AgentOrderTotalCalculationService(IWorkContext workContext,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IShippingService shippingService,
            IPaymentService paymentService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IDiscountService discountService,
            IGiftCardService giftCardService,
            IGenericAttributeService genericAttributeService,
            IRewardPointService rewardPointService,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            AgentSettings agentSettings)
        {
            this._orderTotalCalculationServiceImplementation = new OrderTotalCalculationService(
                workContext, storeContext, priceCalculationService, taxService, shippingService, paymentService,
                checkoutAttributeParser, discountService, giftCardService, genericAttributeService, rewardPointService,
                taxSettings, rewardPointsSettings, shippingSettings, shoppingCartSettings, catalogSettings
            );
            this._agentSettings = agentSettings;
        }


        public void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart, bool includingTax, out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            _orderTotalCalculationServiceImplementation.GetShoppingCartSubTotal(cart, includingTax, out discountAmount, out appliedDiscounts, out subTotalWithoutDiscount, out subTotalWithDiscount);
        }

        public void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart, bool includingTax, out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount, out SortedDictionary<decimal, decimal> taxRates)
        {
            _orderTotalCalculationServiceImplementation.GetShoppingCartSubTotal(cart, includingTax, out discountAmount, out appliedDiscounts, out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
        }

        public decimal AdjustShippingRate(decimal shippingRate, IList<ShoppingCartItem> cart, out List<DiscountForCaching> appliedDiscounts)
        {
            return _orderTotalCalculationServiceImplementation.AdjustShippingRate(shippingRate, cart, out appliedDiscounts);
        }

        public decimal GetShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart)
        {
            return _orderTotalCalculationServiceImplementation.GetShoppingCartAdditionalShippingCharge(cart);
        }

        public bool IsFreeShipping(IList<ShoppingCartItem> cart, decimal? subTotal = null)
        {
            return _orderTotalCalculationServiceImplementation.IsFreeShipping(cart, subTotal);
        }

        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart)
        {
            return _orderTotalCalculationServiceImplementation.GetShoppingCartShippingTotal(cart);
        }

        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax)
        {
            return _orderTotalCalculationServiceImplementation.GetShoppingCartShippingTotal(cart, includingTax);
        }

        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, out decimal taxRate)
        {
            return _orderTotalCalculationServiceImplementation.GetShoppingCartShippingTotal(cart, includingTax, out taxRate);
        }

        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, out decimal taxRate, out List<DiscountForCaching> appliedDiscounts)
        {
            return _orderTotalCalculationServiceImplementation.GetShoppingCartShippingTotal(cart, includingTax, out taxRate, out appliedDiscounts);
        }

        public decimal GetTaxTotal(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        {
            return _orderTotalCalculationServiceImplementation.GetTaxTotal(cart, usePaymentMethodAdditionalFee);
        }

        public decimal GetTaxTotal(IList<ShoppingCartItem> cart, out SortedDictionary<decimal, decimal> taxRates, bool usePaymentMethodAdditionalFee = true)
        {
            return _orderTotalCalculationServiceImplementation.GetTaxTotal(cart, out taxRates, usePaymentMethodAdditionalFee);
        }

        public decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart, bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            throw new System.NotImplementedException();
        }

        public decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart, out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards, out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            throw new System.NotImplementedException();
        }

        public decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart, bool ignoreRewardPonts = false, bool usePaymentMethodAdditionalFee = true)
        {
            var total = _orderTotalCalculationServiceImplementation.GetShoppingCartTotal(cart, ignoreRewardPonts, usePaymentMethodAdditionalFee);
            if (total.HasValue && total == 0 &&
                cart.All(sci => sci.ProductId == this._agentSettings.OrderAgentProductId))
            {
                total = null;
            }
            return total;
        }

        public decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart, out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards, out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool ignoreRewardPonts = false, bool usePaymentMethodAdditionalFee = true)
        {
            return _orderTotalCalculationServiceImplementation.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscounts, out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount, ignoreRewardPonts, usePaymentMethodAdditionalFee);
        }

        public void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart)
        {
            _orderTotalCalculationServiceImplementation.UpdateOrderTotals(updateOrderParameters, restoredCart);
        }

        public decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            return _orderTotalCalculationServiceImplementation.ConvertRewardPointsToAmount(rewardPoints);
        }

        public int ConvertAmountToRewardPoints(decimal amount)
        {
            return _orderTotalCalculationServiceImplementation.ConvertAmountToRewardPoints(amount);
        }

        public bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            return _orderTotalCalculationServiceImplementation.CheckMinimumRewardPointsToUseRequirement(rewardPoints);
        }

        public decimal CalculateApplicableOrderTotalForRewardPoints(decimal orderShippingInclTax, decimal orderTotal)
        {
            throw new System.NotImplementedException();
        }

        public int CalculateRewardPoints(Customer customer, decimal amount)
        {
            return _orderTotalCalculationServiceImplementation.CalculateRewardPoints(customer, amount);
        }
    }
}