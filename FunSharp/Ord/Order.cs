using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Ord
{
    public class Order
    {
        public static IComparer<T> By<T, P>(Func<T, P> selector) where P : IComparable<P>
        {
            return By<T, P>(selector, new ComparableComparer<P>());
        }

        public static IComparer<T> By<T, P>(Func<T, P> selector, IComparer<P> comp)
        {
            return new FuncComparer<T, P>(selector, comp);
        }
    }
}
