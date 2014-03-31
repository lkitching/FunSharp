using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Typeclasses
{
    public interface IMonoid<T>
    {
        T Identity { get; }
        T Append(T ma, T mb);
    }

    public static class Monoids
    {
        public static IMonoid<bool> All
        {
            get { return new Monoid<bool> { Identity = true, AppendFunc = (b1, b2) => b1 && b2 }; }
        }

        public static IMonoid<bool> Any
        {
            get { return new Monoid<bool> { Identity = false, AppendFunc = (b1, b2) => b1 || b2 }; }
        }

        public static IMonoid<T> Sum<T>(INum<T> numInstance)
        {
            return new Monoid<T> { Identity = numInstance.Zero, AppendFunc = (a, b) => numInstance.Plus(a, b) };
        }

        public static IMonoid<T> Product<T>(INum<T> numInstance)
        {
            return new Monoid<T> { Identity = numInstance.One, AppendFunc = (a, b) => numInstance.Mult(a, b) };
        }

        public static IMonoid<IEnumerable<T>> Seq<T>()
        {
            return new Monoid<IEnumerable<T>> { Identity = Enumerable.Empty<T>(), AppendFunc = (sa, sb) => sa.Concat(sb) };
        }

        public static IMonoid<Maybe<T>> MaybeFirst<T>()
        {
            return new Monoid<Maybe<T>>
            {
                Identity = Maybe.None<T>(),
                AppendFunc = (ma, mb) => ma.HasValue ? ma : mb
            };
        }

        public static IMonoid<Maybe<T>> MaybeLast<T>()
        {
            return new Monoid<Maybe<T>> { Identity = Maybe.None<T>(), AppendFunc = (ma, mb) => mb.HasValue ? mb : ma };
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
