using System;
using FunSharp.Typeclasses;

namespace FunSharp.Applicative
{
    /// <summary>
    /// Implementation of Const from Control.Applicative. Described in 'Applicative programming with effects'
    /// </summary>
    /// <seealso cref="http://www.soi.city.ac.uk/~ross/papers/Applicative.html"/>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public class Const<A, B>
    {
        /// <summary>Creates a new instance of this class with the given inner value.</summary>
        /// <param name="value"></param>
        public Const(A value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Maps this instance with the given transform function. Since <typeparamref name="B"/> is a phantom, 
        /// <paramref name="f"/> is not used.
        /// </summary>
        /// <typeparam name="TOut">Output type of the transformation delegate.</typeparam>
        /// <param name="f">The transform delegate.</param>
        /// <returns>A new Const instance with the same inner value as this instance with a phantom type of <typeparamref name="TOut"/>.</returns>
        public Const<A, TOut> Select<TOut>(Func<B, TOut> f)
        {
            return new Const<A, TOut>(this.Value);
        }

        /// <summary>Gets the value for this instance.</summary>
        public A Value { get; private set; }
    }

    /// <summary>Static/extension methods for <see cref="Const{A, B}"/> instances.</summary>
    public static class Const
    {
        /// <summary>Gets a monoid instance for Const given a monoid instance for the embedded data type.</summary>
        /// <typeparam name="A">Type of the embedded value.</typeparam>
        /// <typeparam name="B">Phantom type.</typeparam>
        /// <param name="mon">Monoid instance for the data type <typeparamref name="A"/>.</param>
        /// <returns>A monoid instance for <see cref="Const{A, B}"/>.</returns>
        public static IMonoid<Const<A, B>> Monoid<A, B>(IMonoid<A> mon)
        {
            return new Monoid<Const<A, B>>
            (
                new Const<A,B>(mon.Identity),
                (ac, bc) => new Const<A, B>(mon.Append(ac.Value, bc.Value))
            );
        }

        /// <summary>Creates a new <see cref="Const{M, A}"/> from a monoid instance. Implementation of pure from the Applicative class.</summary>
        /// <typeparam name="M">The monoid instance to use.</typeparam>
        /// <typeparam name="A">Phantom type.</typeparam>
        /// <param name="_">Value of the phantom type. This value is not used.</param>
        /// <param name="monoid">The monoid instance to use.</param>
        /// <returns>An instance of <see cref="Const{M, A}"/> with an inner value of the identity for <paramref name="monoid"/>.</returns>
        public static Const<M, A> Pure<M, A>(A _, IMonoid<M> monoid)
        {
            return new Const<M, A>(monoid.Identity);
        }

        /// <summary>Applicative 'ap' (<*>) function.</summary>
        /// <typeparam name="M">Value type for the input const instances.</typeparam>
        /// <typeparam name="A">Phantom type for <paramref name="c"/></typeparam>
        /// <typeparam name="B">Return type for the phantom function type.</typeparam>
        /// <param name="fc">Const instance.</param>
        /// <param name="c">Const instance.</param>
        /// <param name="monoid">Monoid instance for the embedded value type.</param>
        /// <returns>Const containing the embedded values in <paramref name="fc"/> and <paramref name="c"/> combined under <paramref name="m"/>.</returns>
        public static Const<M, B> Ap<M, A, B>(this Const<M, Func<A, B>> fc, Const<M, A> c, IMonoid<M> monoid)
        {
            return new Const<M, B>(monoid.Append(fc.Value, c.Value));
        }
    }
}
