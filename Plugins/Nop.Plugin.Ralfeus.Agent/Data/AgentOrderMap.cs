using System.ComponentModel.DataAnnotations.Schema;
using Nop.Data.Mapping;
using Nop.Plugin.Ralfeus.Agent.Domain;

namespace Nop.Plugin.Ralfeus.Agent.Data
{
    public class AgentOrderMap : NopEntityTypeConfiguration<AgentOrder>
    {
        public AgentOrderMap()
        {
            ToTable("AgentOrder");
            HasKey(m => m.Id);
            Property(m => m.Status);
        }
    }
}