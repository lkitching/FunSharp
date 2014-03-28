using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Linq
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void ElementAt_Should_Return_Empty_If_Index_Negative()
        {
            var seq = new List<int> { 1, 2 };
            var result = seq.MaybeElementAt(-1);
            TestAssert.IsNone(result);
        }

        [Test]
        public void ElementAt_Should_Return_Empty_If_Index_Too_Large_ReadOnlyList()
        {
            var roList = new TestReadOnlyList<int>(new[] { 1, 2 });
            var result = roList.MaybeElementAt(roList.Count);
            TestAssert.IsNone(result);
        }

        [Test]
        public void ElementAt_Should_Return_Empty_If_Index_Too_Large_List()
        {
            var mutList = new TestMutableList<int>(new[] { 1, 2 });
            var result = mutList.MaybeElementAt(mutList.Count);
            TestAssert.IsNone(result);
        }

        [Test]
        public void ElementAt_Should_Use_Indexer_ReadOnlyList()
        {
            var roList = new TestReadOnlyList<int>(new[] { 1, 2, 3 });
            int index = 1;

            var result = roList.MaybeElementAt(index);
            TestAssert.IsSome(result, roList[index]);
            Assert.AreEqual(index, roList.LastReadIndex, "Should use indexer for IReadOnlyList");
        }

        [Test]
        public void ElementAt_Should_Use_Indexer_List()
        {
            var mutList = new TestMutableList<int>(new[] { 1, 2, 0 });
            int index = 2;

            var result = mutList.MaybeElementAt(index);
            TestAssert.IsSome(result, mutList[index]);
            Assert.AreEqual(index, mutList.LastReadIndex, "Should use indexer for IList");
        }

        [Test]
        public void ElementAt_Should_Find_Item_NonIndexable_Sequence()
        {
            var seq = new TestSeq<string>(new[] { "a", "b", "c" });

            var result = seq.MaybeElementAt(2);
            TestAssert.IsSome(result, "c");
        }

        [Test]
        public void ElementAt_Should_Return_Empty_If_Index_Out_Of_Range_NonIndexable_Sequence()
        {
            var seq = new TestSeq<string>(new[] { "a", "b", "c" });
            var result = seq.MaybeElementAt(5);

            TestAssert.IsNone(result);
        }

        [Test]
        public void MaybeFirst_Should_Return_Empty_For_Empty_Sequence()
        {
            TestAssert.IsNone(new int[0].MaybeFirst());
        }

        [Test]
        public void MaybeFirst_Should_Get_First_Element()
        {
            var seq = new int[] { -3, 4, 5 };
            TestAssert.IsSome(seq.MaybeFirst(), seq[0]);
        }

        [Test]
        public void MaybeFirst_Predicate_Should_Return_Empty_If_None_Matches()
        {
            var seq = new[] { 1, 5, 2, 7 };
            TestAssert.IsNone(seq.MaybeFirst(i => i > 10));
        }

        [Test]
        public void MaybeFirst_Predicate_Should_Return_First_Match()
        {
            var words = new[] { "the", "quick", "brown", "fox" };
            Func<string, bool> predicate = w => w.Length == 5;

            var result = words.MaybeFirst(predicate);
            var expected = words.First(predicate);
            TestAssert.IsSome(result, expected);
        }

        [Test]
        public void MaybeLast_Should_Use_ReadOnlyList_Count()
        {
            var roList = new TestReadOnlyList<int>(new int[0]);
            TestAssert.IsNone(roList.MaybeLast());
            Assert.IsTrue(roList.ReadCount, "MaybeLast should use count to check IReadOnlyList for empty");
        }

        [Test]
        public void MaybeLast_Should_Use_ReadOnlyList_Indexer()
        {
            string last = "last";
            var elems = new[] { "a", "b", "c", last };
            var roList = new TestReadOnlyList<string>(elems);

            TestAssert.IsSome(roList.MaybeLast(), last);
            Assert.AreEqual(roList.LastReadIndex, elems.Length - 1, "MaybeLast should use indexer to get last item in an IReadOnlyList");
        }

        [Test]
        public void MaybeLast_Should_Use_List_Count()
        {
            var mutList = new TestMutableList<int>(new int[0]);
            TestAssert.IsNone(mutList.MaybeLast());
            Assert.IsTrue(mutList.ReadCount, "MaybeLast should use count to check IList for empty");
        }

        [Test]
        public void MaybeLast_Should_Use_List_Indexer()
        {
            int last = -1;
            var elems = new[] { 4, 3, 8, last };
            var mutList = new TestMutableList<int>(elems);

            TestAssert.IsSome(mutList.MaybeLast(), last);
            Assert.AreEqual(mutList.LastReadIndex, elems.Length - 1, "MaybeLast should use indexer to get last item in an IList");
        }

        [Test]
        public void MaybeLast_Should_Get_Last_In_NonIndexable_Sequence()
        {
            int last = 2;
            var seq = new TestSeq<int>(new[] { 4, 6, last });
            TestAssert.IsSome(seq.MaybeLast(), last);
        }

        [Test]
        public void MaybeLast_Should_Return_Empty_If_NonIndexable_Sequence_Empty()
        {
            var seq = new TestSeq<int>(new int[0]);
            TestAssert.IsNone(seq.MaybeLast());
        }

        [Test]
        public void MaybeLast_Predicate_Should_Return_Empty_If_None_Matches()
        {
            TestAssert.IsNone(new[] { 1, 2, 3, 7 }.MaybeLast(i => i > 10));
        }

        [Test]
        public void MaybeLast_Predicate_Should_Return_Last_Match()
        {
            var words = new[] { "the", "quick", "brown", "fox" };
            Func<string, bool> predicate = str => str.Length == 5;

            TestAssert.IsSome(words.MaybeLast(predicate), words.Last(predicate));
        }

        [Test]
        public void MaybeSingle_Should_Return_Empty_For_Empty_Sequence()
        {
            TestAssert.IsNone(new string[0].MaybeSingle());
        }

        [Test]
        public void MaybeSingle_Should_Return_Only_Element()
        {
            string value = "value";
            TestAssert.IsSome(new[] { value }.MaybeSingle(), value);
        }

        [Test]
        public void MaybeSingle_Should_Return_Empty_If_Sequence_Contains_More_Than_One_Element()
        {
            TestAssert.IsNone(new[] { 1, 2 }.MaybeSingle());
        }

        [Test]
        public void MaybeSingle_Predicate_Should_Return_Empty_If_None_Matches()
        {
            TestAssert.IsNone(new[] { 1, 2, 5 }.MaybeSingle(i => i > 10));
        }

        [Test]
        public void MaybeSingle_Predicate_Should_Return_Only_Match()
        {
            TestAssert.IsSome(new[] { 1, 2, 5 }.MaybeSingle(i => i > 3), 5);
        }

        [Test]
        public void MaybeSingle_Predicate_Should_Return_Empty_If_Multiple_Matches()
        {
            TestAssert.IsNone(new[] { 1, 4, 7, 2 }.MaybeSingle(i => i > 3));
        }

        [Test]
        public void MaybeMax_Should_Return_Empty_For_Empty_Sequence()
        {
            TestAssert.IsNone(new int[0].MaybeMax());
        }

        [Test]
        public void MaybeMax_Should_Get_Max()
        {
            var seq = new[] { 4, 7, 1, 2 };
            TestAssert.IsSome(seq.MaybeMax(), seq.Max());
        }

        [Test]
        public void MaybeMin_Should_Return_Empty_For_Empty_Sequence()
        {
            TestAssert.IsNone(new int[0].MaybeMin());
        }

        [Test]
        public void MaybeMin_Should_Get_Min()
        {
            var seq = new[] { 5, 2, -2, -10, 2 };
            TestAssert.IsSome(seq.MaybeMin(), seq.Min());
        }

        private class TestReadOnlyList<T> : IReadOnlyList<T>
        {
            private readonly List<T> items;

            public TestReadOnlyList(IEnumerable<T> seq)
            {
                this.items = seq.ToList();
                this.LastReadIndex = -1;
            }

            public int LastReadIndex { get; private set; }

            public T this[int index]
            {
                get
                {
                    this.LastReadIndex = index;
                    return this.items[index];
                }
            }

            public bool ReadCount { get; private set; }

            public int Count
            {
                get
                {
                    this.ReadCount = true;
                    return this.items.Count;
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class TestMutableList<T> : IList<T>
        {
            private readonly List<T> items;

            public TestMutableList(IEnumerable<T> seq)
            {
                this.items = seq.ToList();
                this.LastReadIndex = -1;
            }

            public int LastReadIndex { get; private set; }

            public int IndexOf(T item)
            {
                return this.items.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                this.items.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                this.items.RemoveAt(index);
            }

            public T this[int index]
            {
                get
                {
                    this.LastReadIndex = index;
                    return this.items[index];
                }
                set
                {
                    this.items[index] = value;
                }
            }

            public void Add(T item)
            {
                this.items.Add(item);
            }

            public void Clear()
            {
                this.items.Clear();
            }

            public bool Contains(T item)
            {
                return this.items.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                this.items.CopyTo(array, arrayIndex);
            }

            public bool ReadCount { get; private set; }

            public int Count
            {
                get
                {
                    this.ReadCount = true;
                    return this.items.Count;
                }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(T item)
            {
                return this.items.Remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class TestSeq<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> items;
            public TestSeq(IEnumerable<T> items)
            {
                this.items = items;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

    }
}
