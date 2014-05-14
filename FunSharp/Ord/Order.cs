using System;
using System.Collections.Generic;

namespace FunSharp.Ord
{
    /// <summary>Utility class for creating comparers.</summary>
    public class Order
    {
        /// <summary>Creates a comparer to compare items by the values returned from a given delegate.</summary>
        /// <typeparam name="T">The type to compare.</typeparam>
        /// <typeparam name="P">The result type to do the comparison on.</typeparam>
        /// <param name="selector">Transformation delegate.</param>
        /// <returns>A comparer to compare items of type <typeparamref name="T"/> by comparing values of the result type <typeparamref name="P"/> returned from <paramref name="selector"/>.</returns>
        public static IComparer<T> By<T, P>(Func<T, P> selector) where P : IComparable<P>
        {
            return By<T, P>(selector, new ComparableComparer<P>());
        }

        /// <summary>Creates a comparer to compare items by the values returned from a given delegate.</summary>
        /// <typeparam name="T">The type to compare.</typeparam>
        /// <typeparam name="P">The result type to do the comparison on.</typeparam>
        /// <param name="selector">Transformation delegate.</param>
        /// <param name="comp">Comparer used to compare the results from <paramref name="selector"/>.</param>
        /// <returns>A comparer to compare items of type <typeparamref name="T"/> by applying the delegate <paramref name="selector"/> and comparing the results with <paramref name="comp"/>.</returns>
        public static IComparer<T> By<T, P>(Func<T, P> selector, IComparer<P> comp)
        {
            return new FuncComparer<T, P>(selector, comp);
        }
    }
}
