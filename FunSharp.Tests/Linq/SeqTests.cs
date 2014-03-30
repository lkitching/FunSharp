using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Linq
{
    [TestFixture]
    public class SeqTests
    {
        [Test]
        public void Should_Unfold_Sequence()
        {
            var seq = Seq.Unfold(i => i >= 10 ? Maybe.None<Tuple<int, int>>() : Maybe.Some(Tuple.Create(i, i + 1)), 0).ToArray();
            CollectionAssert.AreEqual(Enumerable.Range(0, 10), seq, "Unexpected unfolded sequence");
        }

        [Test]
        public void Unfold_Should_Return_Empty_Sequence_If_Accumulator_Returns_Empty()
        {
            CollectionAssert.IsEmpty(Seq.Unfold(_ => Maybe.None<Tuple<int, int>>(), 1), "Unfolded sequence should be empty.");
        }

        [Test]
        public void Singleton_Sequence_Should_Contain_Element()
        {
            var item = "value";
            CollectionAssert.AreEqual(new[] { item }, Seq.Singleton(item));
        }
    }
}
