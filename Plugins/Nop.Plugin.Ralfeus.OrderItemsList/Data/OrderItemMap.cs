using Nop.Data.Mapping;
using Nop.Plugin.Ralfeus.OrderItemsList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Data
{
    public class OrderItemMap : NopEntityTypeConfiguration<OrderItem2>
    {
        protected override void PostInitialize()
        {
            this.Ignore(o => o.Order);
            this.Ignore(o => o.OrderId);
            this.Ignore(o => o.OrderItemStatus);
            this.Ignore(o => o.Product);
        }
    }
}
