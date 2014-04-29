using FunSharp.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FunSharp
{
    /// <summary>Represents a value which can be one of two given types.</summary>
    /// <typeparam name="L">The type of 'Left' values.</typeparam>
    /// <typeparam name="R">The type of 'Right' values.</typeparam>
    public interface IEither<L, R>
    {
        /// <summary>Whether this instance is a Left.</summary>
        bool IsLeft { get; }

        /// <summary>Whether this instance is a Right.</summary>
        bool IsRight { get; }

        /// <summary>Gets the Left value for this instance if it exists.</summary>
        /// <exception cref="InvalidOperation">If this instance is not a Left.</exception>
        L Left { get; }

        /// <summary>Gets the Right value for this instance if it exists.</summary>
        /// <exception cref="InvalidOperation">If this instance is not a Right.</exception>
        R Right { get; }

        /// <summary>Maps this instance to a value of a single type by applying the corresponding processing function.</summary>
        /// <typeparam name="T">The result type of the mapping.</typeparam>
        /// <param name="leftFunc">Function to apply to Left values.</param>
        /// <param name="rightFunc">Function to apply to Right values.</param>
        /// <returns>The result of mapping the inner value for this instance.</returns>
        T Either<T>(Func<L, T> leftFunc, Func<R, T> rightFunc);

        /// <summary>Maps the value in this instance with the corresponding map function.</summary>
        /// <typeparam name="LL">The resulting Left type.</typeparam>
        /// <typeparam name="RR">The resulting Right type.</typeparam>
        /// <param name="leftMap">Function to map Left values.</param>
        /// <param name="rightMap">Function to map Right values.</param>
        /// <returns>A mapped Left if this is a Left, otherwise a mapped Right.</returns>
        IEither<LL, RR> SelectBoth<LL, RR>(Func<L, LL> leftMap, Func<R, RR> rightMap);
    }

    public class EitherLeft<L, R> : IEither<L, R>
    {
        public EitherLeft(L value)
        {
            this.Left = value;
        }

        public bool IsLeft
        {
            get { return true; }
        }

        public bool IsRight
        {
            get { return false; }
        }

        public L Left { get; private set; }

        public R Right
        {
            get { throw new InvalidOperationException("No Right value in Left"); }
        }

        public T Either<T>(Func<L, T> leftFunc, Func<R, T> rightFunc)
        {
            return leftFunc(this.Left);
        }

        public IEither<LL, RR> SelectBoth<LL, RR>(Func<L, LL> leftMap, Func<R, RR> rightMap)
        {
            return new EitherLeft<LL, RR>(leftMap(this.Left));
        }
    }

    public class EitherRight<L, R> : IEither<L, R>
    {
        public EitherRight(R value)
        {
            this.Right = value;
        }

        public bool IsLeft
        {
            get { return false; }
        }

        public bool IsRight
        {
            get { return true; }
        }

        public L Left
        {
            get { throw new InvalidOperationException("No Left in a Right"); }
        }

        public R Right { get; private set; }

        public T Either<T>(Func<L, T> leftFunc, Func<R, T> rightFunc)
        {
            return rightFunc(this.Right);
        }

        public IEither<LL, RR> SelectBoth<LL, RR>(Func<L, LL> leftMap, Func<R, RR> rightMap)
        {
            return new EitherRight<LL, RR>(rightMap(this.Right));
        }
    }

    /// <summary>Utility/extension methods for handling Either values.</summary>
    public static class Either
    {
        /// <summary>Creates a new Left with the given value.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="left">The Left value.</param>
        /// <returns>A Left with the given value.</returns>
        public static IEither<L, R> Left<L, R>(L left)
        {
            return new EitherLeft<L, R>(left);
        }

        /// <summary>Creates a new Right with the given value.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="right">The Right value.</param>
        /// <returns>A Left with the given value.</returns>
        public static IEither<L, R> Right<L, R>(R right)
        {
            return new EitherRight<L, R>(right);
        }

        /// <summary>Gets the Left value from an Either if it exists.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="either">The either.</param>
        /// <returns>The Left value in <paramref name="either"/> if it exists.</returns>
        public static Maybe<L> MaybeLeft<L, R>(this IEither<L, R> either)
        {
            return either.IsLeft ? Maybe.Some(either.Left) : Maybe.None<L>();
        }

        /// <summary>Gets the Right value from an Either if it exists.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="either">The either.</param>
        /// <returns>The Right value in <paramref name="either"/> if it exists.</returns>
        public static Maybe<R> MaybeRight<L, R>(this IEither<L, R> either)
        {
            return either.IsRight ? Maybe.Some(either.Right) : Maybe.None<R>();
        }

        /// <summary>Extracts all the Left values from a sequence of Eithers.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="seq">The sequence to extract Left values from.</param>
        /// <returns>Collection of Left values in the input sequence.</returns>
        public static IEnumerable<L> Lefts<L, R>(this IEnumerable<IEither<L, R>> seq)
        {
            return seq.Where(e => e.IsLeft).Select(e => e.Left);
        }

        /// <summary>Extracts all the Right values from a sequence of Eithers.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="seq">The sequence to extract Right values from.</param>
        /// <returns>Collection of Right values in the input sequence.</returns>
        public static IEnumerable<R> Rights<L, R>(this IEnumerable<IEither<L, R>> seq)
        {
            return seq.Where(e => e.IsRight).Select(e => e.Right);
        }

        /// <summary>Extracts the Left and Right values from a sequence of Either values.</summary>
        /// <typeparam name="L">The type of Left values.</typeparam>
        /// <typeparam name="R">The type of Right values.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <returns>Pair containing all Left values and all Right values in the input sequence.</returns>
        public static Tuple<IEnumerable<L>, IEnumerable<R>> PartitionEithers<L, R>(this IEnumerable<IEither<L, R>> seq)
        {
            return seq
                .Partition(e => e.IsLeft)
                .SelectBoth(ls => ls.Select(l => l.Left), rs => rs.Select(r => r.Right));
        }
    }
}
