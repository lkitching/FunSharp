using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Ord
{
    public static class ComparableExtensions
    {
        public static bool LessThan<T>(this T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) < 0;
        }

        public static bool LessThanOrEqual<T>(this T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool GreaterThan<T>(this T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) > 0;
        }

        public static bool GreaterThanOrEqual<T>(this T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool Eq<T>(this T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) == 0;
        }
    }
}
