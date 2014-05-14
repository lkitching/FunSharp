using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FunSharp.Ord
{
    [TestFixture]
    public class ComparerExtensionsTests
    {
        [Test]
        public void ShouldReverse()
        {
            int smaller = 3;
            int larger = 10;
            var comp = new ComparableComparer<int>().Reverse();

            Assert.Less(comp.Compare(larger, smaller), 0);
            Assert.AreEqual(0, comp.Compare(1, 1));
            Assert.Greater(comp.Compare(smaller, larger), 0);
        }
    }
}
