using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp
{
    [TestFixture]
    public class EitherTests
    {
        [Test]
        public void Left_Should_Return_Value_From_Left()
        {
            int value = 4;
            var e = Either.Left<int, string>(value);
            Assert.AreEqual(value, e.Left);
        }

        [Test]
        public void Right_Should_Throw_For_Left()
        {
            var e = Either.Left<int, string>(-1);
            Assert.Throws<InvalidOperationException>(() => { var _ = e.Right; });
        }

        [Test]
        public void Right_Should_Return_Value_From_Right()
        {
            string value = ":D";
            var e = Either.Right<int, string>(value);
            Assert.AreEqual(value, e.Right);
        }

        [Test]
        public void Left_Should_Throw_For_Right()
        {
            var e = Either.Right<int, string>(":(");
            Assert.Throws<InvalidOperationException>(() => { var _ = e.Left; });
        }

        [Test]
        public void Either_Should_Apply_Left()
        {
            int value = 4;
            Func<int, int> f = i => i + 3;

            var e = Either.Left<int, string>(value);
            Assert.AreEqual(f(value), e.Either(f, s => s.Length));
        }

        [Test]
        public void Either_Should_Apply_Right()
        {
            string value = "value";
            Func<string, int> f = s => s.Length;

            var e = Either.Right<int, string>(value);
            Assert.AreEqual(f(value), e.Either(i => i, f));
        }

        [Test]
        public void SelectBoth_Should_Map_Left()
        {
            int value = 4;
            Func<int, int> f = i => i * 2;

            var e = Either.Left<int, string>(value);
            var mapped = e.SelectBoth(f, s => s.Length);
            Assert.AreEqual(f(value), mapped.Left);
        }

        [Test]
        public void SelectBoth_Should_Map_Right()
        {
            string value = "The quick brown fox";
            Func<string, int> f = s => s.Length;

            var e = Either.Right<int, string>(value);
            var mapped = e.SelectBoth(i => i, f);
            Assert.AreEqual(f(value), mapped.Right);
        }

        [Test]
        public void Should_Get_Left_Values()
        {
            var es = new IEither<int, string>[]
            {
                Either.Left<int, string>(1),
                Either.Right<int, string>("s1"),
                Either.Right<int, string>("s2"),
                Either.Left<int, string>(2)
            };

            CollectionAssert.AreEqual(new[] { 1, 2 }, es.Lefts());
        }

        [Test]
        public void Should_Get_Right_Values()
        {
            var es = new IEither<int, string>[]
            {
                Either.Left<int, string>(1),
                Either.Right<int, string>("s1"),
                Either.Right<int, string>("s2"),
                Either.Left<int, string>(2)
            };

            CollectionAssert.AreEqual(new[] { "s1", "s2" }, es.Rights());
        }

        [Test]
        public void Should_Partition_Either_Values()
        {
            var es = new IEither<int, string>[]
            {
                Either.Left<int, string>(1),
                Either.Right<int, string>("s1"),
                Either.Right<int, string>("s2"),
                Either.Left<int, string>(2)
            };

            var ps = es.PartitionEithers();
            CollectionAssert.AreEqual(new[] { 1, 2 }, ps.Item1);
            CollectionAssert.AreEqual(new[] { "s1", "s2" }, ps.Item2);
        }
    }
}
