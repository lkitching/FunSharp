using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FunSharp.Ord
{
    [TestFixture]
    public class FuncComparerTests
    {
        [Test]
        public void Should_Be_Greater()
        {
            var comp = new FuncComparer<string, int>(s => s.Length, new ComparableComparer<int>());
            Assert.Greater(comp.Compare("abcdefg", "hij"), 0);
        }

        [Test]
        public void Should_Be_Equal()
        {
            var comp = new FuncComparer<DateTime, long>(dt => dt.Ticks, new ComparableComparer<long>());
            var now = DateTime.Now;
            TestAssert.AreEqual<int>(0, comp.Compare(now, now));
        }

        [Test]
        public void Should_Be_Less()
        {
            var tc1 = new TestClass { Value = 4 };
            var tc2 = new TestClass { Value = 10 };

            var comp = new FuncComparer<TestClass, int>(c => c.Value, new ComparableComparer<int>());
            Assert.Less(comp.Compare(tc1, tc2), 0);
        }

        private class TestClass
        {
            public int Value { get; set; }
        }
    }
}
