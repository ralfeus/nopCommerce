using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Plugin.Ralfeus.Agent.Validators;

namespace Nop.Plugin.Ralfeus.Agent.Models
{
    [Validator(typeof(AgentOrderValidator))]
    public class AgentOrderModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.ProductUrl")]
        public string ProductUrl { get; set; }
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
        [NopResourceDisplayName("Ralfeus.Agent.Order.ApproxPrice")]
        public decimal ApproxPrice { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Ralfeus.Agent.Order.Comment")]
        public string Comment { get; set; }
    }
}