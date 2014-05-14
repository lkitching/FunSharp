using System;
namespace FunSharp.Ord
{
    /// <summary>Represents the result of a comparison.</summary>
    public enum Ordering
    {
        /// <summary>Elements are equal.</summary>
        EQ = 0,

        /// <summary>First element is less than the second element.</summary>
        LT = -1,

        /// <summary>First element is greater than the second element.</summary>
        GT = 1
    }

    /// <summary>Extension methods for <see cref="Ordering"/>.</summary>
    public static class OrderingExtensions
    {
        /// <summary>Converts the result of a comparison e.g. from <see cref="IComparable{T}.Compare(x,y)"/> to an <see cref="Ordering"/>.</summary>
        /// <param name="result">The comparison result to convert.</param>
        /// <returns>An equivalent <see cref="Ordering"/> value.</returns>
        public static Ordering FromComparisonResult(this int result)
        {
            if (result < 0) return Ordering.LT;
            else if (result > 0) return Ordering.GT;
            else return 0;
        }

        /// <summary>Converts an <see cref="Ordering"/> to a comparison result.</summary>
        /// <param name="ord">The ordering to convert.</param>
        /// <returns>An equivalent comparison result.</returns>
        public static int ToComparisonResult(this Ordering ord)
        {
            return (int)ord;
        }

        /// <summary>Reverses the given <see cref="Ordering"/> i.e. converts GT to LT and vice versa.</summary>
        /// <param name="ord">The <see cref="Ordering"/> value to convert.</param>
        /// <returns>The reverse of <paramref name="ord"/>.</returns>
        public static Ordering Reverse(this Ordering ord)
        {
            return (Ordering)(-1 * (int)ord);
        }
    }
}
