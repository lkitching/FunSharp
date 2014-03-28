using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FunSharp
{
    /// <summary>Represents a computation that may fail, and contains either the result or generated exception. Based on the scala.util.Try class.</summary>
    /// <typeparam name="T">The result type of the computation.</typeparam>
    public interface ITry<T> : IEnumerable<T>
    {
        /// <summary>Indicates whether this instance represents a successful computation with a value.</summary>
        bool IsSuccess { get; }

        /// <summary>Gets the value from this instance if it exists. Otherwise throws the inner excpetion if this instance represents failure.</summary>
        T Value { get; }

        /// <summary>
        /// Gets a successful try containing the exception for this instance if it exists. If this instance is a success, then a 
        /// successful try containing a <see cref="InvalidOperationException"/>.
        /// </summary>
        ITry<Exception> Failed { get; }

        /// <summary>Creates a new successful try from the exception in this instance if it is a failure.</summary>
        /// <param name="recoverFunc">Delegate to create a value of <typeparamref name="T"/> given the exception for this instance.</param>
        /// <returns>
        /// This instance if it is a success. If this instance represents failure, then a successful try containing the result returned from <paramref name="recoverFunc"/>.
        /// If <paramref name="recoverFunc"/> is null or throws an exception, then a failed try containing the exception.
        /// </returns>
        ITry<T> Recover(Func<Exception, T> recoverFunc);

        /// <summary>Creates a new try given the exception for this instance if it is a failure.</summary>
        /// <param name="recoveryFunc">Delegate to construct a new try from the exception of this instance.</param>
        /// <returns>This instance if it is a success. The try returned from <paramref name="recoveryFunc"/> if this instance is a failure, or a failed try if it is null or throws an exception.</returns>
        ITry<T> RecoverWith(Func<Exception, ITry<T>> recoveryFunc);

        /// <summary>Transforms either the value or exception contained within this instance.</summary>
        /// <typeparam name="U">The result type of the returned try.</typeparam>
        /// <param name="resultFunc">Delegate to create a try from the result of this instance if it is a success.</param>
        /// <param name="exceptionFunc">Delegate to create a try from the exception for this instance if it is a failure.</param>
        /// <returns>
        /// The try returned from either <paramref name="resultFunc"/> or <paramref name="exceptionFunc"/>. If the corresponding delegate
        /// throws is null or throws an exception, then a failed try is returned.
        /// </returns>
        ITry<U> Transform<U>(Func<T, ITry<U>> resultFunc, Func<Exception, ITry<U>> exceptionFunc);

        /// <summary>Transforms the value in this instance with the given transform delegate.</summary>
        /// <typeparam name="U">The result type for the transform delegate.</typeparam>
        /// <param name="selector">The delegate to transform the value for this instance.</param>
        /// <returns>
        /// A successful try containing the value returned from <paramref name="selector"/> if this instance is successful and has a value.
        /// Otherwise return this instance if it is failure, or a failed try containing the exception thrown by <paramref name="selector"/>.
        /// </returns>
        ITry<U> Select<U>(Func<T, U> selector);

        /// <summary>Monadic bind operator to create a new try from the result of this instance.</summary>
        /// <typeparam name="U">The result type of the try returned from <paramref name="bindFunc"/>.</typeparam>
        /// <param name="bindFunc">The continuation to apply to the value for this instance.</param>
        /// <returns>
        /// The value returned from <paramref name="bindFunc"/> if this instance is a success. If <paramref name="bindFunc"/> is null or
        /// throws an exception then a failed try. If this instance is failure then a new try containing the inner exception is returned.
        /// </returns>
        ITry<U> SelectMany<U>(Func<T, ITry<U>> bindFunc);

        /// <summary>Filters the value contained in this instance if it exists.</summary>
        /// <param name="predicate">A predicate to apply to the value for this instance.</param>
        /// <returns>This instance if it is successful and <paramref name="predicate"/> returns true. If <paramref name="predicate"/> returns 
        /// false, then a failed try containing a <see cref="NoSuchElementException"/>. If this instance represents failure, or if <paramref name="predicate"/>
        /// throws an exception, a failed try containing the inner exception.</returns>
        ITry<T> Where(Func<T, bool> predicate);
    }

    /// <summary>Represents a successful <see cref="ITry{T}"/> which contains a value.</summary>
    /// <typeparam name="T">The type of the contained value.</typeparam>
    public sealed class Success<T> : ITry<T>
    {
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="value">The value for this try.</param>
        public Success(T value)
        {
            this.Value = value;
        }

        /// <summary>Returns true.</summary>
        public bool IsSuccess
        {
            get { return true; }
        }

        /// <summary>Gets the value for this instance.</summary>
        public T Value { get; private set; }

        /// <summary>Returns a successful try containing an <see cref="InvalidOperationException"/> since this instance has no exception to return.</summary>
        public ITry<Exception> Failed
        {
            get { return Try.Success<Exception>(new InvalidOperationException("No exception for Success")); }
        }

        /// <summary>Returns this.</summary>
        /// <returns>This instance.</returns>
        public ITry<T> Recover(Func<Exception, T> recoverFunc) { return this; }

        /// <summary>Returns this.</summary>
        /// <returns>This instance.</returns>
        public ITry<T> RecoverWith(Func<Exception, ITry<T>> recoveryFunc) { return this; }

        /// <summary>Transforms the value in this instance with the given success transform delegate.</summary>
        /// <typeparam name="U">The result type of the transformed try.</typeparam>
        /// <param name="resultFunc">Delegate to construct a new try from the value of this instance.</param>
        /// <param name="exceptionFunc">Ignored.</param>
        /// <returns>The try returned from <paramref name="resultFunc"/> or a failed try if it throws an exception.</returns>
        public ITry<U> Transform<U>(Func<T, ITry<U>> resultFunc, Func<Exception, ITry<U>> exceptionFunc)
        {
            return Try.Create(() => resultFunc(this.Value));
        }

        /// <summary>Transforms the value for this instance with the given delegate.</summary>
        /// <typeparam name="U">Result type of the mapping function.</typeparam>
        /// <param name="selector">The delegate to transform the value of this instance.</param>
        /// <returns>A new success containing the transformed value returned from <paramref name="selector"/> or a failed try if it throws an exception.</returns>
        public ITry<U> Select<U>(Func<T, U> selector)
        {
            return Try.From(() => selector(this.Value));
        }

        /// <summary>Monadic bind operator to apply the given continuation to the value for this instance.</summary>
        /// <typeparam name="U">The result type of the try returned by <paramref name="bindFunc"/>.</typeparam>
        /// <param name="bindFunc">The continuation to apply to the value for this instance.</param>
        /// <returns>The try returned from <paramref name="bindFunc"/>. A failed try if <paramref name="bindFunc"/> is null or throws an exception.</returns>
        public ITry<U> SelectMany<U>(Func<T, ITry<U>> bindFunc)
        {
            if (bindFunc == null) return new Failure<U>(new ArgumentNullException("bindFunc"));
            try { return bindFunc(this.Value); }
            catch (Exception ex)
            {
                return Try.Failed<U>(ex);
            }
        }

        /// <summary>Filter the value for this instance with the given predicate.</summary>
        /// <param name="predicate">Predicate for the value contained in this instance.</param>
        /// <returns>
        /// This instance if the value passes <paramref name="predicate"/>. If <paramref name="predicate"/> returns false a failed try containing a <see cref="NoSuchElementException"/>
        /// is returned. If <paramref name="predicate"/> is null or throws an exception, a failed try containing the resulting exception.
        /// </returns>
        public ITry<T> Where(Func<T, bool> predicate)
        {
            if (predicate == null) return Try.Failed<T>(new ArgumentNullException("predicate"));
            try
            {
                if (predicate(this.Value)) return this;
                else
                {
                    string message = string.Format("Predicate does not hold for {0}", this.Value);
                    return Try.Failed<T>(new NoSuchElementException(message));
                }
            }
            catch (Exception ex) { return Try.Failed<T>(ex); }
        }

        /// <summary>Gets an enumerator for the values in this collection.</summary>
        /// <returns>An enumerator for a sequence containing the value for this instance.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            yield return this.Value;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>Represents a failed computation.</summary>
    /// <typeparam name="T">The value type of the failed computation.</typeparam>
    public sealed class Failure<T> : ITry<T>
    {
        private readonly Exception exception;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="ex">The exception describing the failure.</param>
        public Failure(Exception ex)
        {
            this.exception = ex ?? new ArgumentNullException("ex");
        }

        /// <summary>Returns false.</summary>
        public bool IsSuccess
        {
            get { return false; }
        }

        /// <summary>Throws the exception contained in this instance.</summary>
        public T Value
        {
            get { throw this.exception; }
        }

        /// <summary>Returns a successful try containing the exception for this instance.</summary>
        public ITry<Exception> Failed
        {
            get { return Try.Success(this.exception); }
        }

        /// <summary>Creates a successful try containing the value produced by the given recovery function.</summary>
        /// <param name="recoverFunc">Delegate to create a value of type <typeparamref name="T"/> from the exception contained in this instance.</param>
        /// <returns>A successful try containing the value returned by <paramref name="recoverFunc"/>, or a failed try if it throws an exception.</returns>
        public ITry<T> Recover(Func<Exception, T> recoverFunc)
        {
            return Try.From(() => recoverFunc(this.exception));
        }

        /// <summary>Creates a try from the exception contained in this instance.</summary>
        /// <param name="recoveryFunc">Delegate to create the new try given the inner exception for this instance.</param>
        /// <returns>The try returned from <paramref name="recoveryFunc"/> or a failed try containing any exception it throws.</returns>
        public ITry<T> RecoverWith(Func<Exception, ITry<T>> recoveryFunc)
        {
            return Try.Create(() => recoveryFunc(this.exception));
        }

        /// <summary>Transforms the exception for this instance with the given transformation delegate.</summary>
        /// <typeparam name="U">Result type of the try returned from <paramref name="exceptionFunc"/>.</typeparam>
        /// <param name="resultFunc">Ignored.</param>
        /// <param name="exceptionFunc">Delegate to transform the exception for this instance.</param>
        /// <returns>The try returned from <paramref name="exceptionFunc"/> or a failed try containing the exception it throws.</returns>
        public ITry<U> Transform<U>(Func<T, ITry<U>> resultFunc, Func<Exception, ITry<U>> exceptionFunc)
        {
            return Try.Create(() => exceptionFunc(this.exception));
        }

        /// <summary>Propagates the failure represented by this instance.</summary>
        /// <typeparam name="U">The result type of the transform delegate.</typeparam>
        /// <param name="selector">Ignored.</param>
        /// <returns>A new try containing the exception of this instance.</returns>
        public ITry<U> Select<U>(Func<T, U> selector)
        {
            return new Failure<U>(this.exception);
        }

        /// <summary>Propagates the failure represented by this instance.</summary>
        /// <typeparam name="U">The result type of the transform delegate.</typeparam>
        /// <param name="selector">Ignored.</param>
        /// <returns>A new try containing the exception of this instance.</returns>
        public ITry<U> SelectMany<U>(Func<T, ITry<U>> bindFunc)
        {
            return new Failure<U>(this.exception);
        }

        /// <summary>Returns this.</summary>
        /// <param name="predicate">Ignored.</param>
        /// <returns>This instance.</returns>
        public ITry<T> Where(Func<T, bool> predicate)
        {
            return this;
        }

        /// <summary>Gets an enumerator for the values in this collection.</summary>
        /// <returns>An enumerator for an empty sequence.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>Utility/extension methods for <see cref="Try{T}"/>.</summary>
    public static class Try
    {
        /// <summary>Creates a successful try with the given value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A successful try with the given value.</returns>
        public static ITry<T> Success<T>(T value)
        {
            return new Success<T>(value);
        }

        /// <summary>Creates a failed try with the given exception.</summary>
        /// <typeparam name="T">The value type of the failed try.</typeparam>
        /// <param name="ex">The inner exception.</param>
        /// <returns>A failed try of the specified type.</returns>
        public static ITry<T> Failed<T>(Exception ex)
        {
            return new Failure<T>(ex);
        }

        /// <summary>Creates a succeeded try from the result of the given delegate.</summary>
        /// <typeparam name="T">The delegate to create the value of the try.</typeparam>
        /// <param name="func">Delegate to create the result of the try.</param>
        /// <returns>A successful try containing the value returned from <see cref="func"/>, otherwise a failed try if <paramref name="func"/> is null or throws an exception.</returns>
        public static ITry<T> From<T>(Func<T> func)
        {
            if (func == null) return Failed<T>(new ArgumentNullException("func"));
            return Create(() => Success(func()));
        }

        /// <summary>Creates a try from the given delegate.</summary>
        /// <typeparam name="T">The value type of the created try.</typeparam>
        /// <param name="func">The delegate to create the try.</param>
        /// <returns>The try returned from <paramref name="func"/> or a failed try if <paramref name="func"/> is null or throws an exception.</returns>
        public static ITry<T> Create<T>(Func<ITry<T>> func)
        {
            if (func == null) return new Failure<T>(new ArgumentNullException("func"));
            try { return func(); }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }

        /// <summary>Gets the value from a try, or the given default if the try is failed.</summary>
        /// <typeparam name="T">The value type of <paramref name="try"/>.</typeparam>
        /// <param name="try">The try to get the value from if it is succeeded.</param>
        /// <param name="default">The default value to return if <paramref name="try"/> is failed.</param>
        /// <returns>The value for <paramref name="try"/> it if is succeeded, otherwise <paramref name="default"/>.</returns>
        public static T GetOr<T>(this ITry<T> @try, T @default)
        {
            return @try.IsSuccess ? @try.Value : @default;
        }

        /// <summary>Gets the value from a try, or the value returned from the given delegate if the try is failed.</summary>
        /// <typeparam name="T">The value type of <paramref name="try"/>.</typeparam>
        /// <param name="try">The try to get the value from if one exists.</param>
        /// <param name="defaultFunc">Delegate to get the default value from it <paramref name="try"/> is failed.</param>
        /// <returns>The value from <paramref name="try"/> if it exists, otherwise the value returned from <paramref name="defaultFunc"/>.</returns>
        public static T GetOr<T>(this ITry<T> @try, Func<T> defaultFunc)
        {
            Contract.Requires(defaultFunc != null);
            return @try.IsSuccess ? @try.Value : defaultFunc();
        }

        /// <summary>Returns the first try if it is successful, otherwise the second.</summary>
        /// <typeparam name="T">The result type of the two tries.</typeparam>
        /// <param name="first">The first try.</param>
        /// <param name="second">The second.</param>
        /// <returns><paramref name="first"/> if it is successful, otherwise <paramref name="second"/>.</returns>
        public static ITry<T> OrElse<T>(this ITry<T> first, ITry<T> second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);

            return first.IsSuccess ? first : second;
        }

        /// <summary>Returns the first try if it is successful, otherwise the value returned from the given delegate.</summary>
        /// <typeparam name="T">The result type of the two tries.</typeparam>
        /// <param name="first">The first try.</param>
        /// <param name="secondFunc">Delegate to create the second try if <paramref name="first"/> is failed.</param>
        /// <returns>
        /// <paramref name="first"/> if it is successful, otherwise the try returned from <paramref name="secondFunc"/>.
        /// If <paramref name="secondFunc"/> is null or throws an exception, the a failed try containing the exception.
        /// </returns>
        public static ITry<T> OrElse<T>(this ITry<T> first, Func<ITry<T>> secondFunc)
        {
            Contract.Requires(first != null);
            Contract.Requires(secondFunc != null);

            return first.IsSuccess ? first : Create(secondFunc);
        }

        /// <summary>Convert the given try to a <see cref="Maybe{T}"/>.</summary>
        /// <typeparam name="T">The value type of the try.</typeparam>
        /// <param name="try">The try to convert.</param>
        /// <returns>None if <paramref name="try"/> is failure, otherwise a <see cref="Maybe{T}"/> containing its value.</returns>
        public static Maybe<T> ToMaybe<T>(this ITry<T> @try)
        {
            return @try.IsSuccess ? Maybe.Some(@try.Value) : Maybe.None<T>();
        }
    }
}
