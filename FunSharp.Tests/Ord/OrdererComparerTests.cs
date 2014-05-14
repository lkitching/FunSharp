using NUnit.Framework;

namespace FunSharp.Ord
{
    [TestFixture]
    public class OrdererComparerTests
    {
        [Test]
        public void Should_Compare()
        {
            Orderer<int> ord = (x, y) => Ordering.LT;
            var comp = new OrdererComparer<int>(ord);
            Assert.Less(comp.Compare(1, 2), 0);
        }
    }
}
