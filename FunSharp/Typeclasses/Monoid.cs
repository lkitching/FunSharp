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

    /// <summary>Class containing instances and extension methods for monoids.</summary>
    public static class Monoids
    {
        /// <summary>Monoid instance for <see cref="Unit"/>.</summary>
        public static IMonoid<Unit> Unit
        {
            get { return new Monoid<Unit> { Identity = FunSharp.Unit.Instance, AppendFunc = (u1, u2) => FunSharp.Unit.Instance }; }
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
            {
                Identity = Tuple.Create(am.Identity, bm.Identity),
                AppendFunc = (t1, t2) => Tuple.Create(am.Append(t1.Item1, t2.Item1), bm.Append(t1.Item2, t2.Item2))
            };
        }

        /// <summary>Monoid instance for functions with a monoidal return type.</summary>
        /// <typeparam name="T">Argument type of the function.</typeparam>
        /// <typeparam name="TResult">Result type of the function.</typeparam>
        /// <param name="resultM">Monoid for the result type.</param>
        /// <returns>A monoid instance for functions returnining values of type <typeparamref name="TResult"/>.</returns>
        public static IMonoid<Func<T, TResult>> Func<T, TResult>(IMonoid<TResult> resultM)
        {
            return new Monoid<Func<T, TResult>>
            {
                Identity = _ => resultM.Identity,
                AppendFunc = (f, g) => x => resultM.Append(f(x), g(x))
            };
        }

        /// <summary>Monoid instance for booleans under logical (&&).</summary>
        public static IMonoid<bool> All
        {
            get { return new Monoid<bool> { Identity = true, AppendFunc = (b1, b2) => b1 && b2 }; }
        }

        /// <summary>Monoid instance for booleans under logical (||).</summary>
        public static IMonoid<bool> Any
        {
            get { return new Monoid<bool> { Identity = false, AppendFunc = (b1, b2) => b1 || b2 }; }
        }

        /// <summary>Sum monoid for numbers.</summary>
        /// <typeparam name="T">The underlying numeric type.</typeparam>
        /// <param name="numInstance">The num instance for the numeric type.</param>
        /// <returns>Sum monoid for the numeric type <typeparamref name="T"/>.</returns>
        public static IMonoid<T> Sum<T>(INum<T> numInstance)
        {
            return new Monoid<T> { Identity = numInstance.Zero, AppendFunc = (a, b) => numInstance.Plus(a, b) };
        }

        /// <summary>Produce monoid for numbers.</summary>
        /// <typeparam name="T">The underlying numeric type.</typeparam>
        /// <param name="numInstance">Num instance for the numeric type.</param>
        /// <returns>A monoid instance of multiplication for the numeric type <typeparamref name="T"/>.</returns>
        public static IMonoid<T> Product<T>(INum<T> numInstance)
        {
            return new Monoid<T> { Identity = numInstance.One, AppendFunc = (a, b) => numInstance.Mult(a, b) };
        }

        /// <summary>Monoid instance for strings.</summary>
        public static IMonoid<string> String
        {
            get { return new Monoid<string> { Identity = string.Empty, AppendFunc = string.Concat }; }
        }

        /// <summary>Monoid instance for sequences.</summary>
        /// <typeparam name="T">Element type of the sequence.</typeparam>
        /// <returns>A monoid instance for sequences.</returns>
        public static IMonoid<IEnumerable<T>> Seq<T>()
        {
            return new Monoid<IEnumerable<T>> { Identity = Enumerable.Empty<T>(), AppendFunc = (sa, sb) => sa.Concat(sb) };
        }

        /// <summary>Monoid instance for Maybe which returns the first non-empty value.</summary>
        /// <typeparam name="T">The inner maybe type.</typeparam>
        /// <returns>A monoid instance for Maybe which returns the first non-empty value.</returns>
        public static IMonoid<Maybe<T>> MaybeFirst<T>()
        {
            return new Monoid<Maybe<T>>
            {
                Identity = Maybe.None<T>(),
                AppendFunc = (ma, mb) => ma.HasValue ? ma : mb
            };
        }

        /// <summary>Monoid instance for Maybe which returns the last non-empty value.</summary>
        /// <typeparam name="T">The inner maybe type.</typeparam>
        /// <returns>Monoid instance for Maybe which returns the last non-empty value.</returns>
        public static IMonoid<Maybe<T>> MaybeLast<T>()
        {
            return new Monoid<Maybe<T>> { Identity = Maybe.None<T>(), AppendFunc = (ma, mb) => mb.HasValue ? mb : ma };
        }

        /// <summary>Creates the dual of the given monoid by reversing the argument to Append.</summary>
        /// <typeparam name="T">The value type for the monoid.</typeparam>
        /// <param name="m">The monoid to find the dual of.</param>
        /// <returns>Returns a monoid instance the same as <paramref name="m"/> with the arguments to Append reversed.</returns>
        public static IMonoid<T> Dual<T>(this IMonoid<T> m)
        {
            return new Monoid<T> { Identity = m.Identity, AppendFunc = (a, b) => m.Append(b, a) };
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
            return new Monoid<Func<T, T>> { Identity = F.Id<T>(), AppendFunc = (fa, fb) => fa.Comp(fb) };
        }

        private class Monoid<T> : IMonoid<T>
        {
            public T Identity { get; set; }
            public Func<T, T, T> AppendFunc { get; set; }

            public T Append(T ma, T mb)
            {
                return this.AppendFunc(ma, mb);
            }
        }
    }
}
