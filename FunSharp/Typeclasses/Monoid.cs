using FunSharp.Ord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FunSharp.Typeclasses
{
    /// <summary>Represents a monoid.</summary>
    /// <typeparam name="T">The underlying type of the monoid.</typeparam>
    public interface IMonoid<T>
    {
        /// <summary>Identity element.</summary>
        T Identity { get; }

        /// <summary>Binary operation for the monoid.</summary>
        /// <param name="ma">First argument to the operation..</param>
        /// <param name="mb">Second argument to the operation.</param>
        /// <returns>Result of the binary operation.</returns>
        T Append(T ma, T mb);
    }

    /// <summary>Implementation of <see cref="IMonoid{T}"/>.</summary>
    /// <typeparam name="T">Underlying value type of this monoid.</typeparam>
    public class Monoid<T> : IMonoid<T>
    {
        private readonly Func<T, T, T> appendFunc;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="id">Identity for this monoid.</param>
        /// <param name="appendFunc">Delegate for combining two values of <typeparamref name="T"/>.</param>
        public Monoid(T id, Func<T, T, T> appendFunc)
        {
            this.Identity = id;
            this.appendFunc = appendFunc;
        }

        /// <summary>Gets the identity for this monoid.</summary>
        public T Identity { get; private set; }

        /// <summary>Combines two elements from <typeparamref name="T"/>.</summary>
        /// <param name="ma">The first value.</param>
        /// <param name="mb">The second.</param>
        /// <returns>Combination of <paramref name="ma"/> and <paramref name="mb"/>.</returns>
        public T Append(T ma, T mb)
        {
            return this.appendFunc(ma, mb);
        }
    }

    /// <summary>Class containing instances and extension methods for monoids.</summary>
    public static class Monoids
    {
        /// <summary>Monoid instance for <see cref="Unit"/>.</summary>
        public static IMonoid<Unit> Unit
        {
            get { return new Monoid<Unit>(FunSharp.Unit.Instance, (u1, u2) => FunSharp.Unit.Instance); }
        }

        /// <summary>Monoid instance for pairs of monoidal values.</summary>
        /// <typeparam name="A">Type of the first element.</typeparam>
        /// <typeparam name="B">Type of the second element.</typeparam>
        /// <param name="am">Monoid instance for the first element.</param>
        /// <param name="bm">Monoid instance for the second elemenent.</param>
        /// <returns>A monoid instance for pairs of <typeparamref name="A"/> and <typeparamref name="B"/>.</returns>
        public static IMonoid<Tuple<A, B>> Tuple2<A, B>(IMonoid<A> am, IMonoid<B> bm)
        {
            return new Monoid<Tuple<A, B>>
            (
                Tuple.Create(am.Identity, bm.Identity),
                (t1, t2) => Tuple.Create(am.Append(t1.Item1, t2.Item1), bm.Append(t1.Item2, t2.Item2))
            );
        }

        /// <summary>Monoid instance for <see cref="Ordering"/>.</summary>
        public static IMonoid<Ordering> Ordering
        {
            get { return new Monoid<Ordering>(Ord.Ordering.LT, (x, y) => x == Ord.Ordering.EQ ? y : x); }
        }

        /// <summary>Monoid instance for functions with a monoidal return type.</summary>
        /// <typeparam name="T">Argument type of the function.</typeparam>
        /// <typeparam name="TResult">Result type of the function.</typeparam>
        /// <param name="resultM">Monoid for the result type.</param>
        /// <returns>A monoid instance for functions returnining values of type <typeparamref name="TResult"/>.</returns>
        public static IMonoid<Func<T, TResult>> Func<T, TResult>(IMonoid<TResult> resultM)
        {
            return new Monoid<Func<T, TResult>>
            (
                _ => resultM.Identity,
                (f, g) => x => resultM.Append(f(x), g(x))
            );
        }

        /// <summary>Monoid instance for booleans under logical (&&).</summary>
        public static IMonoid<bool> All
        {
            get { return new Monoid<bool>(true, (b1, b2) => b1 && b2); }
        }

        /// <summary>Monoid instance for booleans under logical (||).</summary>
        public static IMonoid<bool> Any
        {
            get { return new Monoid<bool>(false, (b1, b2) => b1 || b2); }
        }

        /// <summary>Sum monoid for numbers.</summary>
        /// <typeparam name="T">The underlying numeric type.</typeparam>
        /// <param name="numInstance">The num instance for the numeric type.</param>
        /// <returns>Sum monoid for the numeric type <typeparamref name="T"/>.</returns>
        public static IMonoid<T> Sum<T>(INum<T> numInstance)
        {
            return new Monoid<T>(numInstance.Zero, (a, b) => numInstance.Plus(a, b));
        }

        /// <summary>Produce monoid for numbers.</summary>
        /// <typeparam name="T">The underlying numeric type.</typeparam>
        /// <param name="numInstance">Num instance for the numeric type.</param>
        /// <returns>A monoid instance of multiplication for the numeric type <typeparamref name="T"/>.</returns>
        public static IMonoid<T> Product<T>(INum<T> numInstance)
        {
            return new Monoid<T>(numInstance.One, (a, b) => numInstance.Mult(a, b));
        }

        /// <summary>Monoid instance for strings.</summary>
        public static IMonoid<string> String
        {
            get { return new Monoid<string>(string.Empty, string.Concat); }
        }

        /// <summary>Monoid instance for sequences.</summary>
        /// <typeparam name="T">Element type of the sequence.</typeparam>
        /// <returns>A monoid instance for sequences.</returns>
        public static IMonoid<IEnumerable<T>> Seq<T>()
        {
            return new Monoid<IEnumerable<T>>(Enumerable.Empty<T>(), (sa, sb) => sa.Concat(sb));
        }

        /// <summary>Monoid instance for Maybe which returns the first non-empty value.</summary>
        /// <typeparam name="T">The inner maybe type.</typeparam>
        /// <returns>A monoid instance for Maybe which returns the first non-empty value.</returns>
        public static IMonoid<Maybe<T>> MaybeFirst<T>()
        {
            return new Monoid<Maybe<T>>(Maybe.None<T>(), (ma, mb) => ma.HasValue ? ma : mb);
        }

        /// <summary>Monoid instance for Maybe which returns the last non-empty value.</summary>
        /// <typeparam name="T">The inner maybe type.</typeparam>
        /// <returns>Monoid instance for Maybe which returns the last non-empty value.</returns>
        public static IMonoid<Maybe<T>> MaybeLast<T>()
        {
            return new Monoid<Maybe<T>>(Maybe.None<T>(), (ma, mb) => mb.HasValue ? mb : ma);
        }

        /// <summary>Creates the dual of the given monoid by reversing the argument to Append.</summary>
        /// <typeparam name="T">The value type for the monoid.</typeparam>
        /// <param name="m">The monoid to find the dual of.</param>
        /// <returns>Returns a monoid instance the same as <paramref name="m"/> with the arguments to Append reversed.</returns>
        public static IMonoid<T> Dual<T>(this IMonoid<T> m)
        {
            return new Monoid<T>(m.Identity, (a, b) => m.Append(b, a));
        }

        /// <summary>Reduces a sequence of monoidal values into a single value.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The sequence to combine.</param>
        /// <param name="m">The monoid instance to use for the operation.</param>
        /// <returns>The combined value of the elements in <paramref name="seq"/> according to the monoid instance <paramref name="m"/>.</returns>
        public static T MConcat<T>(this IEnumerable<T> seq, IMonoid<T> m)
        {
            return seq.Aggregate(m.Identity, m.Append);
        }

        public static IMonoid<Func<T, T>> Endo<T>()
        {
            return new Monoid<Func<T, T>>(F.Id<T>(), (fa, fb) => fa.Comp(fb));
        }
    }
}
