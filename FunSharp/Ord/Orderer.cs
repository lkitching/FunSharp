using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Ord
{
    public delegate Ordering Orderer<in T>(T x, T y);

    public static class OrdererExtensions
    {
        public static IComparer<T> ToComparer<T>(this Orderer<T> ord)
        {
            return new OrdererComparer<T>(ord);
        }

        public static Comparison<T> ToComparison<T>(this Orderer<T> ord)
        {
            return (x, y) => OrdererUtils.ToComparisonResult(ord(x, y));
        }

        public static Orderer<T> ToOrderer<T>(this Comparison<T> comp)
        {
            return (x, y) => OrdererUtils.FromComparisonResult(comp(x, y));
        }

        public static Orderer<T> ToOrderer<T>(this IComparer<T> comp)
        {
            return (x, y) => OrdererUtils.FromComparisonResult(comp.Compare(x, y));
        }
    }

    internal static class OrdererUtils
    {
        internal static Ordering FromComparisonResult(int result)
        {
            if (result < 0) return Ordering.LT;
            else if (result > 0) return Ordering.GT;
            else return 0;
        }

        internal static int ToComparisonResult(Ordering ord)
        {
            return (int)ord;
        }
    }
}
