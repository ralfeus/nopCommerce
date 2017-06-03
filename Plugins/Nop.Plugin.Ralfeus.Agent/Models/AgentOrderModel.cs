using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Ralfeus.Agent.Validators;

namespace Nop.Plugin.Ralfeus.Agent.Models
{
    [Validator(typeof(AgentOrderValidator))]
    public class AgentOrderModel : BaseNopEntityModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.ItemUrl")]
        public string ItemUrl { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.ImagePath")]
        public string ImagePath { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.ItemName")]
        public string ItemName { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.SourceShopName")]
        public string SourceShopName { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Color")]
        public string Color { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Size")]
        public string Size { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Quantity")]
        public int Quantity { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Price")]
        public decimal Price { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Price")]
        public string PriceFormated { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Comment")]
        public string Comment { get; set; }
        [NopResourceDisplayName("Ralfeus.Agent.Order.OrderId")]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Ralfeus.Agent.Order.Customer")]
        public string CustomerName { get; set; }
        [NopResourceDisplayName("Ralfeus.Agent.Order.Status")]
        public int StatusId { get; set; }
        [NopResourceDisplayName("Ralfeus.Agent.Order.Id")]
        public int OrderItemId { get; set; }
    }
}