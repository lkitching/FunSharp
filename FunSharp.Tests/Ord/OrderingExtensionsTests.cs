using NUnit.Framework;

namespace FunSharp.Ord
{
    [TestFixture]
    public class OrderingExtensionsTests
    {
        [Test]
        public void Should_Convert_To_Comparison_Results()
        {
            Assert.Less(Ordering.LT.ToComparisonResult(), 0);
            Assert.AreEqual(0, Ordering.EQ.ToComparisonResult());
            Assert.Greater(Ordering.GT.ToComparisonResult(), 0);
        }

        [Test]
        public void Should_Convert_From_Comparison_Results()
        {
            Assert.AreEqual(Ordering.LT, (-3).FromComparisonResult());
            Assert.AreEqual(Ordering.GT, 4.FromComparisonResult());
            Assert.AreEqual(Ordering.EQ, 0.FromComparisonResult());
        }

        [Test]
        public void Should_Reverse_Orderings()
        {
            Assert.AreEqual(Ordering.LT, Ordering.GT.Reverse());
            Assert.AreEqual(Ordering.GT, Ordering.LT.Reverse());
            Assert.AreEqual(Ordering.EQ, Ordering.EQ.Reverse());
        }
    }
}
