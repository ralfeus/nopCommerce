using System.Data.Entity.Infrastructure;
using Nop.Core.Data;
using Nop.Plugin.Ralfeus.OrderItemsList.Data;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<OrderItemObjectContext>
    {
        public OrderItemObjectContext Create()
        {
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();
            return new OrderItemObjectContext(dataSettings.DataConnectionString);
        }
    }
}