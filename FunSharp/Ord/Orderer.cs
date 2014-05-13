using System;
using System.Collections.Generic;

namespace FunSharp.Ord
{
    /// <summary>Delegate for comparing two values of a given type.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <param name="x">The first item.</param>
    /// <param name="y">The second item.</param>
    /// <returns>An <see cref="Ordering"/> representing the result of the comparison.</returns>
    public delegate Ordering Orderer<in T>(T x, T y);

    public static class OrdererExtensions
    {
        public static IComparer<T> ToComparer<T>(this Orderer<T> ord)
        {
            return new OrdererComparer<T>(ord);
        }

        public static Comparison<T> ToComparison<T>(this Orderer<T> ord)
        {
            return (x, y) => OrderingExtensions.ToComparisonResult(ord(x, y));
        }

        public static Orderer<T> ToOrderer<T>(this Comparison<T> comp)
        {
            return (x, y) => OrderingExtensions.FromComparisonResult(comp(x, y));
        }

        public static Orderer<T> ToOrderer<T>(this IComparer<T> comp)
        {
            return (x, y) => OrderingExtensions.FromComparisonResult(comp.Compare(x, y));
        }

        public static Orderer<T> Reverse<T>(this Orderer<T> ord)
        {
            return (x, y) => ord(y, x);
        }
    }
}
