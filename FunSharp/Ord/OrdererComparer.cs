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

        public int Compare(T x, T y)
        {
            Ordering result = this.ord(x, y);
            return OrderingExtensions.ToComparisonResult(result);
        }
    }
}
