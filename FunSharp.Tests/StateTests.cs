using System;
using NUnit.Framework;

namespace FunSharp
{
    [TestFixture]
    public class StateTests
    {
        [Test]
        public void From_Result_Test()
        {
            var result = "result";
            var s = State.FromResult<Unit, string>(result);
            var sr = s.Run(Unit.Instance);

            Assert.AreEqual(result, sr.Result, "Unexpected result");
        }

        [Test]
        public void Get_Test()
        {
            var state = "state";
            var actual = State.Get<string>().RunState(state);

            Assert.AreEqual(state, actual, "Unexpected state");
        }

        [Test]
        public void Put_Test()
        {
            var state = "state";
            var actual = State.Put(state).RunState("tmp");

            Assert.AreEqual(state, actual, "Unexpected state");
        }

        [Test]
        public void Modify_Test()
        {
            int initState = new Random().Next();
            Func<int, int> f = i => i * 2;
            var actual = State.Modify(f).RunState(initState);

            var expected = f(initState);
            Assert.AreEqual(expected, actual, "Unexpected state");
        }

        [Test]
        public void BiSelect_Test()
        {
            var initResult = "abc";
            int initState = new Random().Next();
            Func<StateResult<int, string>, StateResult<int, string>> f = sr => new StateResult<int, string>(sr.State * 2, sr.Result + sr.State);

            var actual = State.FromResult<int, string>(initResult).BiSelect(f).Run(initState);
            var expected = f(new StateResult<int, string>(initState, initResult));

            Assert.AreEqual(expected.State, actual.State, "Unexpected state");
            Assert.AreEqual(expected.Result, actual.Result, "Unexpected result");
        }

        [Test]
        public void Select_Test()
        {
            var initResult = new Random().Next();
            Func<int, int> op = i => i * 2;
            var s = State.FromResult<Unit, int>(initResult).Select(op);
            var result = s.RunResult(Unit.Instance);
            var expected = op(initResult);

            Assert.AreEqual(expected, result, "Unexpected result");
        }

        [Test]
        public void SelectMany_Test()
        {
            var state = from s1 in State.Get<int>()
                        from s2 in State.FromResult<int, string>(new string('a', s1))
                        from _ in State.Put(s1 * 2)
                        select s2;

            int initState = new Random().Next(2, 10);
            var result = state.Run(initState);

            Assert.AreEqual(2 * initState, result.State, "Unexpected state");
            Assert.AreEqual(new string('a', initState), result.Result, "Unexpected result");
        }
    }
}
