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

        public static void IsSuccess<T>(ITry<T> @try, T expectedValue)
        {
            Assert.IsTrue(@try.IsSuccess, "Try should be success");
            Assert.AreEqual(@try.Value, expectedValue, "Unexpected try value");
        }

        public static void IsFailure<T, TEx>(ITry<T> @try, TEx expected = null) where TEx : Exception
        {
            Assert.IsFalse(@try.IsSuccess, "Try should be failure");

            var ex = @try.Failed.Value;
            Assert.IsInstanceOf<TEx>(ex);

            if (expected != null)
            {
                Assert.AreEqual(expected, ex, "Unexpected exception for failed try");
            }
        }
    }
}
