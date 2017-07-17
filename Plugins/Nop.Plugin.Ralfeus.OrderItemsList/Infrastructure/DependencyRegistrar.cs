using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Plugin.Ralfeus.OrderItemsList.Data;
using Nop.Plugin.Ralfeus.OrderItemsList.Domain;
using Nop.Plugin.Ralfeus.OrderItemsList.Migrations;
using Nop.Plugin.Ralfeus.OrderItemsList.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_order_items";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<OrderItemsService>().As<IOrderItemsService>().InstancePerLifetimeScope();

            //Load custom data settings
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();
 
            //Register custom object context
            builder.Register<IDbContext>(c => RegisterIDbContext(c, dataSettings)).Named<IDbContext>(CONTEXT_NAME).InstancePerRequest();
            builder.Register(c => RegisterIDbContext(c, dataSettings)).InstancePerRequest();
 
            var pluginFinderTypes = typeFinder.FindClassesOfType<IPluginFinder>();
 
            var isInstalled = pluginFinderTypes
                .Select(pluginFinderType => Activator.CreateInstance(pluginFinderType) as IPluginFinder)
                .Select(pluginFinder => pluginFinder?.GetPluginDescriptorBySystemName("Ralfeus.OrderItemsList"))
                .Any(pluginDescriptor => pluginDescriptor != null && pluginDescriptor.Installed);

            if (!isInstalled) return;
            //db migrations, let's update if needed
            var migrator = new DbMigrator(new Configuration());
            migrator.Update();
            
            //data context
            this.RegisterPluginDataContext<OrderItemObjectContext>(builder, CONTEXT_NAME );

            //override required repository with our custom context
            builder.RegisterType<EfRepository<OrderItem2>>()
                .As<IRepository<OrderItem2>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME ))
                .InstancePerLifetimeScope();
        }
        
        /// <summary>
        /// Registers the I db context.
        /// </summary>
        /// <param name="componentContext">The component context.</param>
        /// <param name="dataSettings">The data settings.</param>
        /// <returns></returns>
        private OrderItemObjectContext RegisterIDbContext(IComponentContext componentContext, DataSettings dataSettings)
        {
            string dataConnectionStrings;
 
            if (dataSettings != null && dataSettings.IsValid())
                dataConnectionStrings = dataSettings.DataConnectionString;
            else
                dataConnectionStrings = componentContext.Resolve<DataSettings>().DataConnectionString;
 
            return new OrderItemObjectContext(dataConnectionStrings);
 
        }

        public int Order => 100;
    }
}