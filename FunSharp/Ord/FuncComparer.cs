using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunSharp.Ord
{
    /// <summary>Compares elements of type <typeparamref name="T"/> by comparing the result from a delegate with a given comparer.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <typeparam name="P">The result type for the delegate.</typeparam>
    public class FuncComparer<T, P> : IComparer<T>
    {
        private readonly Func<T, P> compFunc;
        private readonly IComparer<P> resultComparer;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="compFunc">Delegate used to project values of type <typeparamref name="T"/>.</param>
        /// <param name="resultComparer">Used to compare the results returned from <paramref name="compFunc"/>.</param>
        public FuncComparer(Func<T, P> compFunc, IComparer<P> resultComparer)
        {
            Contract.Requires(compFunc != null);
            Contract.Requires(resultComparer != null);

            this.compFunc = compFunc;
            this.resultComparer = resultComparer;
        }

        /// <summary>Compares two elements of type <typeparamref name="T"/>.</summary>
        /// <param name="x">First item.</param>
        /// <param name="y">Second item.</param>
        /// <returns>
        /// The selector delegate is applied to both <paramref name="x"/> and <paramref name="y"/>.
        /// The two resulting values of type <typeparamref name="P"/> are then compared with the
        /// result comparer.
        /// </returns>
        public int Compare(T x, T y)
        {
            return this.resultComparer.Compare(compFunc(x), compFunc(y));
        }
    }
}
