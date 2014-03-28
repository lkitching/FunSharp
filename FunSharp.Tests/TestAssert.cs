using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp
{
    public static class TestAssert
    {
        public static void IsNone<T>(Maybe<T> maybe)
        {
            Assert.IsFalse(maybe.HasValue, "Maybe should be empty");
        }

        public static void IsSome<T>(Maybe<T> maybe, T value)
        {
            Assert.IsTrue(maybe.HasValue, "Maybe should have value");
            Assert.AreEqual(maybe.Value, value, "Unexpected maybe value");
        }
    }
}
