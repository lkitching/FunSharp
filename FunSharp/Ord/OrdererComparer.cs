using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunSharp.Ord
{
    /// <summary>Comparer which uses the result of the given <see cref="Orderer{T}"/> delegate.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    public class OrdererComparer<T> : IComparer<T>
    {
        private readonly Orderer<T> ord;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="ordFunc"></param>
        public OrdererComparer(Orderer<T> ordFunc)
        {
            Contract.Requires(ordFunc != null);
            this.ord = ordFunc;
        }

        /// <summary>Compares two items.</summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item.</param>
        /// <returns>The result of applying the given ordering delegate converted to an int comparison result.</returns>
        public int Compare(T x, T y)
        {
            Ordering result = this.ord(x, y);
            return OrderingExtensions.ToComparisonResult(result);
        }
    }
}
