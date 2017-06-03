using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Ralfeus.Agent.Domain;

namespace Nop.Plugin.Ralfeus.Agent.Models
{
    public class PublicAgentModel
    {
        public IList<AgentOrderStatus> AvailableStatuses => (IList<AgentOrderStatus>)Enum.GetValues(typeof(AgentOrderStatus)).Cast<AgentOrderStatus>();
    }
}