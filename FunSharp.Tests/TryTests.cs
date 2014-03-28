using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp
{
    [TestFixture]
    public class TryTests
    {
        [Test]
        public void Success_Constructor_Should_Set_Value()
        {
            int value = 5;
            var t = new Success<int>(value);
            Assert.IsTrue(t.IsSuccess, "Try should be successful");
            Assert.AreEqual(value, t.Value);
        }

        [Test]
        public void Failure_Constructor_Should_Set_Value()
        {
            var ex = new InvalidOperationException("!!!");
            var t = new Failure<int>(ex);

            Assert.IsFalse(t.IsSuccess, "Try should be failure");
            var thrown = Assert.Throws<InvalidOperationException>(() => { var _ = t.Value; });
            Assert.AreEqual(ex, thrown);
        }

        [Test]
        public void Success_Should_Return_New_Exception_For_Failed()
        {
            var t = Try.Success("value");
            var failed = t.Failed;

            Assert.IsInstanceOf<InvalidOperationException>(failed.Value, "Expected InvalidOperationException value for Success.Failed");
        }

        [Test]
        public void Failure_Should_Return_Inner_Exception_For_Failed()
        {
            var ex = new ArgumentOutOfRangeException();
            var t = Try.Failed<string>(ex);

            var failed = t.Failed;
            Assert.AreEqual(ex, failed.Value, "Expected inner exception for Failure.Failed");
        }

        [Test]
        public void Success_Should_Return_Self_For_Recover()
        {
            var t = Try.Success("value");
            var recovered = t.Recover(ex => "it lives!");

            Assert.AreSame(t, recovered, "Success.Failed should return self for Recover");
        }

        [Test]
        public void Failure_Should_Invoke_Recovery_Func_For_Recover()
        {
            Exception inner = null;
            var ex = new InvalidCastException();
            string recoveryValue = "recovered";

            var t = Try.Failed<string>(ex);
            var recovered = t.Recover(e => { inner = e; return recoveryValue; });

            Assert.AreEqual(ex, inner, "Failure should pass inner exception to recovery function");
            TestAssert.IsSuccess(recovered, recoveryValue);
        }

        [Test]
        public void Success_Should_Return_Self_For_RecoverWith()
        {
            var t = Try.Success(":D");
            var failed = t.RecoverWith(e => Try.Success("That was close"));

            Assert.AreSame(t, failed, "Success.Failed should return self for RecoverWith");
        }

        [Test]
        public void Failure_Should_Invoke_Recovery_For_RecoverWith()
        {
            Exception inner = null;
            var ex = new InvalidOperationException("!!!");
            var recovery = Try.Success("recovered");

            var t = Try.Failed<string>(ex);
            var recovered = t.RecoverWith(e => { inner = e; return recovery; });

            Assert.AreSame(ex, inner, "Failure should pass inner exception to recovery function");
            Assert.AreSame(recovery, recovered, "Failure.RecoverWith should return recovery try");
        }

        [Test]
        public void Success_Should_Pass_Value_To_Transformation()
        {
            string value = ":D";
            string inner = null;

            var t = Try.Success(value);
            var transformed = t.Transform(str => { inner = str; return Try.Success(str.Length); }, ex => Try.Success(-1));

            Assert.AreSame(value, inner, "Success should pass inner value to transform function");
            TestAssert.IsSuccess(transformed, value.Length);
        }

        [Test]
        public void Success_Should_Return_Failed_For_Transform_If_Transformation_Func_Throws()
        {
            var toThrow = new InvalidOperationException("!!!");
            var t = Try.Success(4);

            var transformed = t.Transform(_ => { throw toThrow; }, ex => Try.Success(0));

            TestAssert.IsFailure(transformed, toThrow);
        }

        [Test]
        public void Failure_Should_Pass_Exception_To_Transformation()
        {
            var ex = new ArgumentException(":(");
            Exception inner = null;

            var t = Try.Failed<string>(ex);
            var transformed = t.Transform(Try.Success, e => { inner = ex; return Try.Success(ex.Message); });

            Assert.AreSame(ex, inner, "Failure should pass inner exception to transform function");
            TestAssert.IsSuccess(transformed, ex.Message);
        }

        [Test]
        public void Failure_Should_Return_Failed_For_Transform_If_Transformation_Func_Throws()
        {
            var toThrow = new InvalidOperationException("!!!");
            var t = Try.Failed<int>(new ArgumentOutOfRangeException());

            var transformed = t.Transform(Try.Success, ex => { throw toThrow; });
            TestAssert.IsFailure(transformed, toThrow);
        }

        [Test]
        public void Success_Should_Return_Failure_If_Bind_Func_Null()
        {
            var t = Try.Success("value");
            var result = t.SelectMany<int>(null);

            TestAssert.IsFailure<int, ArgumentNullException>(result);
        }

        [Test]
        public void Success_Should_Pass_Value_To_Bind_Func()
        {
            string value = "value";
            var t = Try.Success(value);
            string inner = null;

            var result = t.SelectMany(str => { inner = str; return Try.Success(str.Length); });
            Assert.AreSame(value, inner, "Success should pass inner value to continuation function");
            TestAssert.IsSuccess(result, value.Length);
        }

        [Test]
        public void Success_Should_Return_Failure_If_Bind_Func_Throws()
        {
            var toThrow = new InvalidOperationException("!!!");
            var t = Try.Success(1);

            var result = t.SelectMany<string>(_ => { throw toThrow; });
            TestAssert.IsFailure(result, toThrow);
        }

        [Test]
        public void Failure_Should_Propagate_Exception_For_SelectMany()
        {
            var ex = new InvalidOperationException("!!!");
            var t = Try.Failed<int>(ex);

            var result = t.SelectMany(Try.Success);
            TestAssert.IsFailure(result, ex);
        }

        [Test]
        public void Success_Should_Return_Failed_For_Where_If_Predicate_Null()
        {
            var t = Try.Success(2);
            TestAssert.IsFailure<int, ArgumentNullException>(t.Where(null));
        }

        [Test]
        public void Success_Should_Return_Self_If_Predicate_Passes()
        {
            var t = Try.Success(4);
            var result = t.Where(i => i < 10);
            Assert.AreSame(t, result, "Success should return self if predicate passes");
        }

        [Test]
        public void Success_Should_Return_Failure_If_Predicate_Fails()
        {
            var t = Try.Success(4);
            var result = t.Where(i => i > 10);
            TestAssert.IsFailure<int, NoSuchElementException>(result);
        }

        [Test]
        public void Success_Should_Return_Failure_If_Predicate_Throws()
        {
            var toThrow = new ArgumentOutOfRangeException();
            var t = Try.Success(3);

            var result = t.Where(_ => { throw toThrow; });
            TestAssert.IsFailure(result, toThrow);
        }

        [Test]
        public void Failure_Should_Return_Self_For_Where()
        {
            var t = Try.Failed<string>(new Exception());
            var result = t.Where(_ => true);
            Assert.AreSame(t, result, "Failure should return self for Where");
        }

        [Test]
        public void Sequence_Should_Contain_Value_For_Success()
        {
            int value = 2;
            var t = Try.Success(value);

            CollectionAssert.AreEqual(new[] { value }, t, "Sequence should contain inner value for Success");
        }

        [Test]
        public void Sequence_Should_Be_Empty_For_Failure()
        {
            var t = Try.Failed<string>(new InvalidCastException());
            CollectionAssert.IsEmpty(t, "Sequence should be empty for Failure");
        }

        //extension methods

        [Test]
        public void From_Should_Return_Success()
        {
            int value = 3;
            var t = Try.From(() => value);
            TestAssert.IsSuccess(t, value);
        }

        [Test]
        public void From_Should_Return_Failure_If_Func_Null()
        {
            var t = Try.From<int>(null);
            TestAssert.IsFailure<int, ArgumentNullException>(t);
        }

        [Test]
        public void From_Should_Return_Failure_If_Func_Throws()
        {
            var toThrow = new FormatException();
            var t = Try.From<string>(() => { throw toThrow; });
            TestAssert.IsFailure(t, toThrow);
        }

        [Test]
        public void Create_Should_Return_Result()
        {
            var t = Try.Success("!!!");
            var created = Try.Create(() => t);
            Assert.AreSame(t, created, "Create should return result from func");
        }

        [Test]
        public void Create_Should_Return_Failed_If_Func_Null()
        {
            var t = Try.Create<int>(null);
            TestAssert.IsFailure<int, ArgumentNullException>(t);
        }

        [Test]
        public void Create_Should_Return_Failed_If_Func_Throws()
        {
            var toThrow = new InvalidOperationException("!!!");
            var t = Try.Create<string>(null);
            TestAssert.IsFailure<string, ArgumentNullException>(t);
        }

        [Test]
        public void Select_Should_Map_Value_For_Success()
        {
            string value = "value";
            var t = Try.Success(value);

            var mapped = t.Select(str => str.Length);
            TestAssert.IsSuccess(mapped, value.Length);
        }

        [Test]
        public void Select_Should_Return_Failed_If_Map_Func_Throws()
        {
            var toThrow = new InvalidOperationException("!!!");
            var t = Try.Success("value");

            var mapped = t.Select<int>(_ => { throw toThrow; });
            TestAssert.IsFailure(mapped, toThrow);
        }

        [Test]
        public void Select_Should_Propagate_Failed()
        {
            var ex = new ArgumentOutOfRangeException();
            var t = Try.Failed<int>(ex);

            var mapped = t.Select(i => i + 3);
            TestAssert.IsFailure(mapped, ex);
        }

        [Test]
        public void GetOr_Should_Get_Value_For_Success()
        {
            var value = 2;
            var t = Try.Success(value);
            Assert.AreEqual(value, t.GetOr(-1), "GetOr should return value for Success");
        }

        [Test]
        public void GetOr_Should_Return_Default_For_Failure()
        {
            int @default = -1;
            var t = Try.Failed<int>(new Exception());
            Assert.AreEqual(@default, t.GetOr(@default), "GetOr should return default for Failure");
        }

        [Test]
        public void GetOr_Func_Should_Get_Value_For_Success()
        {
            var value = 7;
            var t = Try.Success(value);
            Assert.AreEqual(value, t.GetOr(() => -1), "GetOr(Func) should return value for Success");
        }

        [Test]
        public void GetOr_Func_Should_Return_Default_For_Failure()
        {
            int @default = -1;
            var t = Try.Failed<int>(new Exception());
            Assert.AreEqual(@default, t.GetOr(() => @default), "GetOr(Func) should return default for Failure");
        }

        [Test]
        public void OrElse_Should_Return_Success()
        {
            var t = Try.Success(4);
            Assert.AreSame(t, t.OrElse(Try.Success(7)), "OrElse should return first if Success");
        }

        [Test]
        public void OrElse_Should_Return_Default_Try_For_Failure()
        {
            var t = Try.Failed<int>(new Exception());
            var defaultTry = Try.Success(4);
            Assert.AreSame(defaultTry, t.OrElse(defaultTry), "OrElse should return default try if Failure");
        }

        [Test]
        public void OrElse_Func_Should_Return_Success()
        {
            var t = Try.Success(":D");
            Assert.AreSame(t, t.OrElse(() => Try.Success("!!!")), "OrElse(Func) should return first if Success");
        }

        [Test]
        public void OrElse_Func_Should_Return_Default_Try_For_Failure()
        {
            var t = Try.Failed<string>(new InvalidCastException());
            var defaultTry = Try.Success("That was close");
            Assert.AreSame(defaultTry, t.OrElse(() => defaultTry), "OrElse(Func) should return default try if Failure");
        }

        [Test]
        public void Should_Convert_Success_To_Maybe()
        {
            var value = "value";
            var t = Try.Success(value);
            TestAssert.IsSome(t.ToMaybe(), value);
        }

        [Test]
        public void Should_Convert_Failed_To_Maybe()
        {
            var t = Try.Failed<int>(new ArgumentOutOfRangeException());
            TestAssert.IsNone(t.ToMaybe());
        }
    }
}
