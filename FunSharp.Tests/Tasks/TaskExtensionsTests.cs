using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace FunSharp.Tasks
{
    /// <summary>Tests for Task extensions.</summary>
    [TestFixture]
    public class TaskExtensionsTests
    {
        [Test]
        public void Select_Should_Map_Result()
        {
            string value = "value";
            Func<string, int> f = s => s.Length;

            var task = Task.FromResult(value);
            var mapped = task.Select(f);

            Assert.AreEqual(f(value), mapped.Result, "Select should map task result");
        }

        [Test]
        public void Select_Should_Propagate_Failure()
        {
            var ex = new InvalidOperationException("!!!");
            var task = TaskOf.Failed<int>(ex);

            var mapped = task.Select(i => i * 2);
            TestAssert.TaskException<InvalidOperationException>(task, iex => { Assert.AreEqual(ex, iex); });
        }

        [Test]
        public void Select_Should_Propagate_Canceled()
        {
            var task = TaskOf.Canceled<string>();
            var mapped = task.Select(s => s.ToUpper());
            Assert.IsTrue(mapped.IsCanceled, "Mapped task should be canceled");
        }

        [Test]
        public void Select_Should_Return_Failed_Task_If_Map_Function_Throws()
        {
            var toThrow = new InvalidCastException();
            var task = Task.FromResult<string>("value");

            var mapped = task.Select<string, int>(s => { throw toThrow; });
            TestAssert.TaskException<InvalidCastException>(mapped, cex => { Assert.AreEqual(toThrow, cex); });
        }

        [Test]
        public void SelectMany_Should_Combine_Results()
        {
            int value = 4;
            Func<int, int> f = i => i * 2;
            Func<int, int, int> g = (x, y) => x + y;

            var task = Task.FromResult(value).SelectMany(i => Task.FromResult(f(i)), g);
            Assert.AreEqual(g(value, f(value)), task.Result);
        }

        [Test]
        public void SelectMany_Should_Fail_If_First_Task_Fails()
        {
            var ex = new InvalidOperationException("!!!");
            var task = TaskOf.Failed<int>(ex).SelectMany(i => Task.FromResult(i * 2), (x, y) => y);

            TestAssert.TaskException<InvalidOperationException>(task, iex => { Assert.AreEqual(ex, iex); });
        }

        [Test]
        public void SelectMany_Should_Fail_If_Select_Task_Fails()
        {
            var ex = new InvalidOperationException(":(");
            var task = Task.FromResult(3).SelectMany(_ => TaskOf.Failed<int>(ex), (x, y) => x);

            TestAssert.TaskException<InvalidOperationException>(task, iex => { Assert.AreEqual(ex, iex); });
        }

        [Test]
        public void SelectMany_Should_Cancel_If_First_Task_Canceled()
        {
            var task = TaskOf.Canceled<int>().SelectMany(i => Task.FromResult(i * 2), (x, y) => x * y);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public void SelectMany_Should_Cancel_If_Second_Task_Canceled()
        {
            var task = Task.FromResult(3).SelectMany(_ => TaskOf.Canceled<string>(), (i, str) => str);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public void Where_Should_Return_Value_If_Passes_Filter()
        {
            int value = 5;
            var task = Task.FromResult(value);

            var filtered = task.Where(i => i > 2);
            Assert.AreEqual(value, filtered.Result);
        }

        [Test]
        public void Where_Should_Return_Failed_Task_If_Fails_Filter()
        {
            var task = Task.FromResult(5);
            var filtered = task.Where(i => i > 10);
            TestAssert.TaskException<NoSuchElementException>(filtered);
        }

        [Test]
        public void Where_Should_Return_Canceled_If_Task_Canceled()
        {
            var task = TaskOf.Canceled<string>();
            var filtered = task.Where(_ => true);
            Assert.IsTrue(filtered.IsCanceled, "Filtered task should be canceled");
        }

        [Test]
        public void Where_Should_Propagate_Failure()
        {
            var ex = new InvalidOperationException(":(");
            var task = TaskOf.Failed<int>(ex);

            var filtered = task.Where(_ => true);
            TestAssert.TaskException<InvalidOperationException>(filtered, tex =>
            {
                Assert.AreEqual(ex, tex, "Unexpected inner exception");
            });
        }

        [Test]
        public void Where_Should_Fail_If_Predicate_Throws()
        {
            var toThrow = new ArgumentException();
            var task = Task.FromResult(4);

            var filtered = task.Where(_ => { throw toThrow; });
            TestAssert.TaskException<ArgumentException>(filtered, aex =>
            {
                Assert.AreEqual(toThrow, aex, "Task should propagate thrown exception");
            });
        }

        [Test]
        public void TryResult_Should_Map_Result_To_Success()
        {
            int result = 7;
            var task = Task.FromResult(result);

            ITry<int> t = task.TryResult().Result;
            TestAssert.IsSuccess(t, result);
        }

        [Test]
        public void Try_Result_Should_Map_Failure_To_Failed()
        {
            var ex = new InvalidOperationException("!!!");
            var task = TaskOf.Failed<string>(ex);

            ITry<string> t = task.TryResult().Result;
            TestAssert.IsFailure(t, ex);
        }

        [Test]
        public void Try_Result_Should_Map_Canceled_To_Failed()
        {
            var task = TaskOf.Canceled<object>();

            ITry<object> t = task.TryResult().Result;
            Assert.IsFalse(t.IsSuccess);

            Exception ex = t.Failed.Value;
            Assert.IsInstanceOf<OperationCanceledException>(ex);
        }

        [Test]
        public void Recover_Should_Return_Result_If_Successful()
        {
            int result = 5;
            var task = Task.FromResult(result).Recover(_ => Maybe.Some(result + 1));

            Assert.AreEqual(result, task.Result);
        }

        [Test]
        public void Recover_Should_Not_Invoke_Recovery_Func_If_Task_Succeeds()
        {
            bool invoked = false;
            Func<Exception, Maybe<int>> rf = ex => { invoked = true; return Maybe.Some(1); };

            var task = Task.FromResult(3).Recover(rf);
            task.Wait();

            Assert.IsFalse(invoked);
        }

        [Test]
        public void Recover_Should_Return_Recovered_Result()
        {
            string recovered = "recovered";
            var task = TaskOf.Failed<string>(new ArgumentException()).Recover(_ => Maybe.Some(recovered));

            Assert.AreEqual(recovered, task.Result);
        }

        [Test]
        public void Recover_Should_Propagate_Exception_If_Recovery_Func_Returns_None()
        {
            var ex = new InvalidOperationException("!!!");
            var task = TaskOf.Failed<string>(ex).Recover(_ => Maybe.None<string>());

            TestAssert.TaskException<InvalidOperationException>(task, iex => { Assert.AreEqual(ex, iex); });
        }

        [Test]
        public void Query_Test()
        {
            int i = 4;
            int j = 8;

            var task = from x in Task.FromResult(i)
                       from y in Task.FromResult(j)
                       let sum = x + y
                       where sum > 10
                       select x + y;

            Assert.AreEqual(i + j, task.Result);
        }
    }
}
