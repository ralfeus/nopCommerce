using System.Linq;

namespace Nop.Plugin.Ralfeus.OrderItemsList.Infrastructure
{
    public static class Ext
    {
        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }
    }
}