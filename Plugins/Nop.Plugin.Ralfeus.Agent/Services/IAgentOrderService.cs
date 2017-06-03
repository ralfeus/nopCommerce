using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Ralfeus.Agent.Domain;

namespace Nop.Plugin.Ralfeus.Agent.Services
{
    public interface IAgentOrderService
    {
        IPagedList<AgentOrder> GetAgentOrders(Customer customer = null, int start = 0, int pageSize = 2147483647);
        AgentOrder SetAgentOrder(AgentOrder agentOrder);
        void AddAgentOrder(AgentOrder agentOrder);
        void AddAgentOrders(IEnumerable<AgentOrder> agentOrders);
    }
}