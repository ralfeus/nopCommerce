using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Domain
{
    public enum OrderItemStatus
    {
        Pending = 1,
        Processing,
        Ordered,
        Ready,
        Packed,
        Complete,
        Soldout,
        Cancelled
    }
}
