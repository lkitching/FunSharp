using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Linq
{
    [TestFixture]
    public class NonEmptyEnumerableTests
    {
        [Test]
        public void Constructor_Should_Set_First()
        {
            var first = "a";
            var seq = new NonEmptyEnumerable<string>(first, Enumerable.Empty<string>());
            Assert.AreEqual(first, seq.First);
        }

        [Test]
        public void Should_Enumerate_Sequence()
        {
            var first = 1;
            var rest = new[] { 5, 2, 3 };

            var seq = new NonEmptyEnumerable<int>(first, rest);
            CollectionAssert.AreEqual(new[] { first }.Concat(rest), seq);
        }
    }
}
