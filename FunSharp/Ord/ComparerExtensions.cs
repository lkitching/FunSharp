using System.Collections.Generic;

namespace FunSharp.Ord
{
    /// <summary>Extension methods for <see cref="IComparer{T}"/>.</summary>
    public static class ComparerExtensions
    {
        /// <summary>
        /// Creates a comparer that first compares with <paramref name="comp"/> and then by <paramref name="next"/> if the result of the first comparison was equal.
        /// </summary>
        /// <typeparam name="T">The element type to compare.</typeparam>
        /// <param name="comp">The first comparer to use.</param>
        /// <param name="next">The second comparer to use.</param>
        /// <returns>A comparer which first compares by <paramref name="comp"/>, then by <paramref name="next"/>.</returns>
        public static IComparer<T> Then<T>(this IComparer<T> comp, IComparer<T> next)
        {
            return new CompositeComparer<T>(comp, next);
        }

        /// <summary>Reverses the result of the comparison for the given comparer.</summary>
        /// <typeparam name="T">The comparison type.</typeparam>
        /// <param name="comp">The comparer to reverse.</param>
        /// <returns>A comparer which reverse the comparison result from <paramref name="comp"/>.</returns>
        public static IComparer<T> Reverse<T>(this IComparer<T> comp)
        {
            return new ReverseComparer<T>(comp);
        }

        private class ReverseComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> toReverse;
            public ReverseComparer(IComparer<T> toReverse)
            {
                this.toReverse = toReverse;
            }

            public int Compare(T x, T y)
            {
                return this.toReverse.Compare(y, x);
            }
        }
    }
}
