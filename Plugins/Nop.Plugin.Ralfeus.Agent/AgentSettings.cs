using Nop.Core.Configuration;

namespace Nop.Plugin.Ralfeus.Agent
{
    public class AgentSettings : ISettings
    {
        public int OrderAgentProductId { get; set; }
        public int ProductAttributeMappingId { get; set; }
    }
}