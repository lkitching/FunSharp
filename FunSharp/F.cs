using System;
using System.Diagnostics.Contracts;

namespace FunSharp
{
    /// <summary>Static class for creating/combining function delegates.</summary>
    public static class F
    {
        /// <summary>Gets an id function of the given type.</summary>
        /// <typeparam name="T">Type of the input.</typeparam>
        /// <returns>Identity function for the type <typeparamref name="T"/>.</returns>
        public static Func<T, T> Id<T>() { return i => i; }

        /// <summary>Creates a function which ignores its argument and returns the given value.</summary>
        /// <typeparam name="T">The type of the ignored parameter to the returned function.</typeparam>
        /// <typeparam name="U">The constant value to return.</typeparam>
        /// <param name="val">The value to return.</param>
        /// <returns>A function which ignores its argument and return <paramref name="val"/>.</returns>
        public static Func<T, U> Const<T, U>(U val) { return _ => val; }

        /// <summary>Composes two functions.</summary>
        /// <typeparam name="T">Argument type for <paramref name="g"/>.</typeparam>
        /// <typeparam name="U">Return type of <paramref name="g"/> and argument type for <paramref name="f"/>.</typeparam>
        /// <typeparam name="V">Return type of <paramref name="f"/>.</typeparam>
        /// <param name="f">First function to compose.</param>
        /// <param name="g">Second function to compose.</param>
        /// <returns>A function h(x) = f(g(x))</returns>
        public static Func<T, V> Comp<T, U, V>(this Func<U, V> f, Func<T, U> g)
        {
            Contract.Requires(f != null);
            Contract.Requires(g != null);

            return x => f(g(x));
        }

        /// <summary>Curries the given function with two arguments.</summary>
        /// <typeparam name="T">Type of the first argument to <paramref name="f"/>.</typeparam>
        /// <typeparam name="U">Type of the second argument to <paramref name="f"/>.</typeparam>
        /// <typeparam name="V">Return type of <paramref name="f"/>.</typeparam>
        /// <param name="f">The function to curry.</param>
        /// <returns>
        /// Returns a function which when given an argument of type <typeparamref name="T"/> returns a function which takes a
        /// single argument of type <typeparamref name="U"/> and returns a value of type <typeparamref name="V"/>.
        /// </returns>
        public static Func<T, Func<U, V>> Curry<T, U, V>(this Func<T, U, V> f)
        {
            Contract.Requires(f != null);
            return x => y => f(x, y);
        }

        /// <summary>Uncurries a given curried function.</summary>
        /// <typeparam name="T">The argument type of <paramref name="f"/>.</typeparam>
        /// <typeparam name="U">The argument type of the function returned from <paramref name="f"/>.</typeparam>
        /// <typeparam name="V">The result type of the argument returned from <paramref name="f"/>.</typeparam>
        /// <param name="f">The function to uncurry.</param>
        /// <returns>A function which takes two arguments and applies the first to <paramref name="f"/> and the second to the returned function.</returns>
        public static Func<T, U, V> Uncurry<T, U, V>(this Func<T, Func<U, V>> f)
        {
            Contract.Requires(f != null);
            return (x, y) => f(x)(y);
        }
    }
}
