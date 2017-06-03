using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Plugin.Ralfeus.Agent.Domain;
using Nop.Plugin.Ralfeus.Agent.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Plugin.Ralfeus.Agent.Infrastructure
{
    public class EventsConsumer : IConsumer<EntityInserted<Order>>, IConsumer<OrderPlacedEvent>, IConsumer<OrderPaidEvent>
    {
        private readonly IAgentOrderService _agentOrderService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IOrderService _orderService;
        private readonly IEventPublisher _publisher;
        private readonly AgentSettings _agentSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;

        public EventsConsumer(IAgentOrderService agentOrderService, IProductAttributeFormatter productAttributeFormatter,
            IOrderService orderService, IEventPublisher publisher, AgentSettings agentSettings, ICurrencyService currencyService,
            IWorkContext workContext)
        {
            _agentOrderService = agentOrderService;
            _productAttributeFormatter = productAttributeFormatter;
            _orderService = orderService;
            _publisher = publisher;
            this._agentSettings = agentSettings;
            _currencyService = currencyService;
            _workContext = workContext;
        }

        public void HandleEvent(EntityInserted<Order> eventMessage)
        {
            #region New order

            if (eventMessage.Entity.Customer.ShoppingCartItems.All(sci => sci.ProductId == this._agentSettings.OrderAgentProductId) ||
                eventMessage.Entity.Customer.ShoppingCartItems.All(sci => sci.ProductId != this._agentSettings.OrderAgentProductId) ||
                (eventMessage.Entity.OrderTotal == 0)) return;
            var newOrder = new Order
            {
                AffiliateId = eventMessage.Entity.AffiliateId,
                AllowStoringCreditCardNumber = eventMessage.Entity.AllowStoringCreditCardNumber,
                BillingAddressId = eventMessage.Entity.BillingAddressId,
                CardCvv2 = eventMessage.Entity.CardCvv2,
                CardExpirationMonth = eventMessage.Entity.CardExpirationMonth,
                CardExpirationYear = eventMessage.Entity.CardExpirationYear,
                CardName = eventMessage.Entity.CardName,
                CardNumber = eventMessage.Entity.CardNumber,
                CardType = eventMessage.Entity.CardType,
                CheckoutAttributeDescription = eventMessage.Entity.CheckoutAttributeDescription,
                CheckoutAttributesXml = eventMessage.Entity.CheckoutAttributesXml,
                CreatedOnUtc = eventMessage.Entity.CreatedOnUtc,
                CurrencyRate = eventMessage.Entity.CurrencyRate,
                CustomerCurrencyCode = eventMessage.Entity.CustomerCurrencyCode,
                CustomerId = eventMessage.Entity.CustomerId,
                CustomerIp = eventMessage.Entity.CustomerIp,
                CustomerLanguageId = eventMessage.Entity.CustomerLanguageId,
                CustomerTaxDisplayType = eventMessage.Entity.CustomerTaxDisplayType,
                CustomerTaxDisplayTypeId = eventMessage.Entity.CustomerTaxDisplayTypeId,
                CustomValuesXml = eventMessage.Entity.CustomValuesXml,
                MaskedCreditCardNumber = eventMessage.Entity.MaskedCreditCardNumber,
                OrderGuid = Guid.NewGuid(),
                OrderStatus = OrderStatus.Pending,
                PaymentMethodAdditionalFeeExclTax = eventMessage.Entity.PaymentMethodAdditionalFeeExclTax,
                PaymentMethodAdditionalFeeInclTax = eventMessage.Entity.PaymentMethodAdditionalFeeInclTax,
                PaymentMethodSystemName = eventMessage.Entity.PaymentMethodSystemName,
                PaymentStatus = PaymentStatus.Paid,
                PickupAddressId = eventMessage.Entity.PickupAddressId,
                PickUpInStore = eventMessage.Entity.PickUpInStore,
                ShippingAddressId = eventMessage.Entity.ShippingAddressId,
                ShippingMethod = eventMessage.Entity.ShippingMethod,
                ShippingRateComputationMethodSystemName =
                    eventMessage.Entity.ShippingRateComputationMethodSystemName,
                ShippingStatusId = eventMessage.Entity.ShippingStatusId,
                StoreId = eventMessage.Entity.StoreId,
                TaxRates = eventMessage.Entity.TaxRates,
                VatNumber = eventMessage.Entity.VatNumber
            };
            this._orderService.InsertOrder(newOrder);

            #endregion

            #region Get Agent orders

            //eventMessage.Entity.Customer.ShoppingCartItems.Except(agentOrders);
            var orderItems =  eventMessage.Entity.Customer.ShoppingCartItems
                .Where(sci => sci.ProductId == this._agentSettings.OrderAgentProductId)
                .Select(ao => new OrderItem
                    {
                        OrderItemGuid = Guid.NewGuid(),
                        Order = newOrder,
                        ProductId = ao.ProductId,
                        UnitPriceInclTax = 0,
                        UnitPriceExclTax = 0,
                        PriceInclTax = 0,
                        PriceExclTax = 0,
                        OriginalProductCost = 0,
                        AttributeDescription = _productAttributeFormatter.FormatAttributes(ao.Product,
                            ao.AttributesXml, eventMessage.Entity.Customer),
                        AttributesXml = ao.AttributesXml,
                        Quantity = ao.Quantity,
                        DiscountAmountInclTax = 0,
                        DiscountAmountExclTax = 0,
                        DownloadCount = 0,
                        IsDownloadActivated = false,
                        LicenseDownloadId = 0,
                        ItemWeight = 0
                    }
                ); //.ToList();
            foreach (var orderItem in orderItems)
            {
                newOrder.OrderItems.Add(orderItem);
            }
            this._orderService.UpdateOrder(newOrder);

            this._publisher.Publish(new OrderPlacedEvent(newOrder));

            #endregion
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order.OrderItems.All(oi => oi.ProductId == this._agentSettings.OrderAgentProductId))
            {
                this._agentOrderService.AddAgentOrders(eventMessage.Order.OrderItems
                    .Where(oi => oi.ProductId == this._agentSettings.OrderAgentProductId)
                    .Select(oi =>
                    {
                        var attributes = XElement.Parse(oi.AttributesXml)
                            .Descendants()
                            .Where(e => e.Name == "ProductAttribute")
                            .ToList();

                        return new AgentOrder
                        {
                            Color = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.ColorAttributeMappingId.ToString())
                                ?.Value,
                            Comment = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.CommentAttributeMappingId.ToString())
                                ?.Value,
                            CustomerId = oi.Order.CustomerId,
                            OrderItemId = oi.Id,
                            ImagePath = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.ImagePathAttributeMappingId.ToString())
                                ?.Value,
                            ItemName = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.ItemNameAttributeMappingId.ToString())
                                ?.Value,
                            ItemUrl = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.ItemUrlAttributeMappingId.ToString())
                                ?.Value,
                            Price = this._currencyService.ConvertToPrimaryStoreCurrency(
                                Convert.ToDecimal(attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.PriceAttributeMappingId.ToString())
                                    ?.Value),
                                this._workContext.WorkingCurrency),
                            OrderId = oi.OrderId,
                            Quantity = oi.Quantity,
                            Size = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value ==
                                    this._agentSettings.SizeAttributeMappingId.ToString())
                                ?.Value,
                            SourceShopName = attributes.FirstOrDefault(e =>
                                    e.Attribute(XName.Get("ID"))?.Value == this._agentSettings
                                        .SourceShopNameAttributeMappingId.ToString())
                                ?.Value,
                            Status = AgentOrderStatus.New
                        };
                    })
                );
            }
            else
            {
                var agentOrders = eventMessage.Order.OrderItems
                    .Where(oi => oi.ProductId == this._agentSettings.OrderAgentProductId)
                    .ToList();
                foreach (var orderItem in agentOrders)
                {
                    this._orderService.DeleteOrderItem(orderItem);
                }
                this._orderService.UpdateOrder(eventMessage.Order);
            }
        }

        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            if (eventMessage.Order.OrderItems.Any(oi => oi.ProductId !=
                                                         this._agentSettings.OrderAgentProductId)) return;
//            eventMessage.Order.PaymentStatus = PaymentStatus.Pending;
//            this._orderService.UpdateOrder(eventMessage.Order);
        }
    }
}