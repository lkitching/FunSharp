using System;
using System.Collections.Generic;

namespace FunSharp.Ord
{
    /// <summary>Comparer which uses the IComparable{T} implementation of the compared type.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    public class ComparableComparer<T> : IComparer<T> where T : IComparable<T>
    {
        /// <summary>Compares two values of type <typeparamref name="T"/>.</summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item.</param>
        /// <returns>The result of <code>x.CompareTo(y)</code></returns>
        public int Compare(T x, T y)
        {
            return x.CompareTo(y);
        }
    }
}
