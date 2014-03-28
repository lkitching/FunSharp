using System.Collections.Generic;

namespace FunSharp
{
    /// <summary>Container class for utility methods.</summary>
    public static class Utils
    {
        /// <summary>Gets the minimum of two items.</summary>
        /// <typeparam name="T">The element types to compare.</typeparam>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <param name="comp">Compare to compare <paramref name="item1"/> and <paramref name="item2"/>.</param>
        /// <returns>The smaller of <paramref name="item1"/> and <paramref name="item2"/> according to <paramref name="comp"/>.</returns>
        public static T Min<T>(T item1, T item2, IComparer<T> comp = null)
        {
            comp = comp ?? Comparer<T>.Default;
            return comp.Compare(item1, item2) < 0 ? item1 : item2;
        }

        /// <summary>Gets the maximum of two items.</summary>
        /// <typeparam name="T">The element types to compare.</typeparam>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <param name="comp">Comparer to compare <paramref name="item1"/> and <paramref name="item2"/>.</param>
        /// <returns>The larger of <paramref name="item1"/> and <paramref name="item2"/> according to <paramref name="comp"/>.</returns>
        public static T Max<T>(T item1, T item2, IComparer<T> comp = null)
        {
            comp = comp ?? Comparer<T>.Default;
            return comp.Compare(item1, item2) >= 0 ? item1 : item2;
        }
    }
}
