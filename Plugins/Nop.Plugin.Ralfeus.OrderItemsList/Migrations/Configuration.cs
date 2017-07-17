using System.Data.Entity.Infrastructure;
using System.Reflection;
using Nop.Core.Data;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Nop.Plugin.Ralfeus.OrderItemsList.Data.OrderItemObjectContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
 
            AutomaticMigrationDataLossAllowed = true;
 
            MigrationsAssembly = Assembly.GetExecutingAssembly();
            MigrationsNamespace = this.GetType().Namespace;
 
            //specify database so that it doesn't throw error during migration. Otherwise, for some reasons it defaults to sqlce and gives error 
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();
            TargetDatabase = new DbConnectionInfo(dataSettings.DataConnectionString, "System.Data.SqlClient");
        }

        protected override void Seed(Nop.Plugin.Ralfeus.OrderItemsList.Data.OrderItemObjectContext context)
        {


        }
    }
}
