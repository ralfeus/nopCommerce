using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Ralfeus.Agent.Data;
using Nop.Plugin.Ralfeus.Agent.Domain;
using Nop.Plugin.Ralfeus.Agent.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Ralfeus.Agent
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_agent_order";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<AgentOrderService>().As<IAgentOrderService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<AgentOrderObjectContext>(builder, CONTEXT_NAME );

            //override required repository with our custom context
            builder.RegisterType<EfRepository<AgentOrder>>()
                .As<IRepository<AgentOrder>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME ))
                .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}