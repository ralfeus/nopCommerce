using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Ralfeus.OrderItemsList.Migrations;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Data
{
    public class OrderItemObjectContext : DbContext, IDbContext
    {
        public bool AutoDetectChangesEnabled
        {
            get => this.Configuration.AutoDetectChangesEnabled; 
            set => this.Configuration.AutoDetectChangesEnabled = value;
        }

        public bool ProxyCreationEnabled
        {
            get => this.Configuration.ProxyCreationEnabled;
            set => this.Configuration.ProxyCreationEnabled = value;
        }

        public OrderItemObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

//        public OrderItemObjectContext() : base ("Data Source=localhost;Initial Catalog=nopCommerce;Integrated Security=True;Persist Security Info=False") { }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Detach(object entity)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new OrderItemMap());

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
//            Database.SetInitializer<OrderItemObjectContext>(null);
//
//            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
//            SaveChanges();
        }

        public void Uninstall()
        {
            Database.ExecuteSqlCommand("SELECT * INTO [OrderItem2_backup_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") +
                                       " FROM OrderItem2");
//            this.DropPluginTable("OrderItem2");
            try
            {
                var migrator = new DbMigrator(new Configuration());
                migrator.Update("0");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
    }
}