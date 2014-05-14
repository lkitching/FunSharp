using NUnit.Framework;
using System.Collections.Generic;

namespace FunSharp.Ord
{
    [TestFixture]
    public class CompositeComparerTests
    {
        private readonly IComparer<TestClass> comp = Order.By((TestClass tc) => tc.Value).Then(Order.By((TestClass tc) => tc.Id));

        [Test]
        public void Should_Return_Less_If_Primary_Not_Equal()
        {
            AssertComp(new TestClass { Value = 2.0, Id = 1 }, new TestClass { Value = 3.0, Id = 2 }, Ordering.LT);
        }

        [Test]
        public void Should_Return_Greater_If_Primary_Not_Equal()
        {
            AssertComp(new TestClass { Value = 6.0, Id = 1 }, new TestClass { Value = -3.0, Id = 2 }, Ordering.GT);
        }

        [Test]
        public void Should_Use_Secondary_If_Primary_Equal()
        {
            AssertComp(new TestClass { Value = 2.0, Id = 5 }, new TestClass { Value = 2.0, Id = 1 }, Ordering.GT);
        }

        private void AssertComp(TestClass tc1, TestClass tc2, Ordering expectedResult)
        {
            int res = this.comp.Compare(tc1, tc2);
            Ordering ord = res.FromComparisonResult();

            Assert.AreEqual(expectedResult, ord, "Unexpected comparison result");
        }

        private class TestClass
        {
            public double Value { get; set; }
            public int Id { get; set; }
        }
    }
}
