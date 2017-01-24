using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.Deposit.Data;
using Nop.Plugin.Payments.Deposit.Domain;
using Nop.Plugin.Payments.Deposit.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_deposit_transaction";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<DepositTransactionService>().As<IDepositTransactionService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<DepositTransactionObjectContext>(builder, CONTEXT_NAME );

            //override required repository with our custom context
            builder.RegisterType<EfRepository<DepositTransaction>>()
                .As<IRepository<DepositTransaction>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME ))
                .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}