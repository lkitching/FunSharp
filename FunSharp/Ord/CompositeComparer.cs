using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunSharp.Ord
{
    /// <summary>Comparer which compares items by a comparer and then by a second comparer if the result of the first comparison was equal.</summary>
    /// <typeparam name="T">Item type to compare.</typeparam>
    public class CompositeComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> firstComp;
        private readonly IComparer<T> secondComp;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="first">The first comparer to use.</param>
        /// <param name="second">The second comparer to use.</param>
        public CompositeComparer(IComparer<T> first, IComparer<T> second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);

            this.firstComp = first;
            this.secondComp = second;
        }

        /// <summary>Comparers two items.</summary>
        /// <param name="x">The first item to compare.</param>
        /// <param name="y">The second item to compare.</param>
        /// <returns>
        /// The result of the first comparison if it is non-zero (i.e. the items are not equal). Otherwise the result of the second comparer.
        /// </returns>
        public int Compare(T x, T y)
        {
            int pComp = this.firstComp.Compare(x, y);
            if (pComp != 0) return pComp;

            return this.secondComp.Compare(x, y);
        }
    }
}
