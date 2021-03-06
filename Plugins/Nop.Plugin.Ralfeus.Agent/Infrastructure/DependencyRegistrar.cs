﻿using System;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Plugin.Ralfeus.Agent.Data;
using Nop.Plugin.Ralfeus.Agent.Domain;
using Nop.Plugin.Ralfeus.Agent.Services;
using Nop.Services.Orders;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Ralfeus.Agent.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_agent_order";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            if (new PluginFinder().GetPluginDescriptorBySystemName("Ralfeus.Agent") == null)
            {
                return;
            }

            builder.RegisterType<AgentOrderService>().As<IAgentOrderService>().InstancePerLifetimeScope();

            builder.RegisterType<AgentOrderTotalCalculationService>()
                .As<IOrderTotalCalculationService>()
                .InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<AgentOrderObjectContext>(builder, CONTEXT_NAME );

            //override required repository with our custom context
            builder.RegisterType<EfRepository<AgentOrder>>()
                .As<IRepository<AgentOrder>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME ))
                .InstancePerLifetimeScope();
        }

        public int Order => 100;
    }
}