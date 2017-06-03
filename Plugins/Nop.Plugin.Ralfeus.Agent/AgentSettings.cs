using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Ralfeus.Agent
{
    public class AgentSettings : ISettings
    {
        public int OrderAgentProductId { get; set; }
        public int ItemUrlAttributeMappingId { get; set; }
        public int SourceShopNameAttributeMappingId { get; set; }
        public int ImagePathAttributeMappingId { get; set; }
        public int ItemNameAttributeMappingId { get; set; }
        public int ColorAttributeMappingId { get; set; }
        public int SizeAttributeMappingId { get; set; }
        public int PriceAttributeMappingId { get; set; }
        public int CommentAttributeMappingId { get; set; }
    }
}