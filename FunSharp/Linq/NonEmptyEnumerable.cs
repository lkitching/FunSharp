using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunSharp.Linq
{
    /// <summary>Represents a non-empty sequence of elements.</summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    public interface INonEmptyEnumerable<out T> : IEnumerable<T>
    {
        /// <summary>Gets the first item in this sequence.</summary>
        T First { get; }
    }

    /// <summary>Represent a non-empty sequence.</summary>
    /// <typeparam name="T">The element type of this sequence.</typeparam>
    public class NonEmptyEnumerable<T> : INonEmptyEnumerable<T>
    {
        private readonly IEnumerable<T> rest;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="first">The first element of this sequence.</param>
        /// <param name="rest">The remaining elements of this sequence.</param>
        public NonEmptyEnumerable(T first, IEnumerable<T> rest)
        {
            Contract.Requires(rest != null);
            this.First = first;
            this.rest = rest;
        }

        /// <summary>Gets the first element of this sequence.</summary>
        public T First { get; private set; }

        /// <summary>Gets an enumerator for this sequence.</summary>
        /// <returns>An enumerator for the elements of this sequence.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            yield return this.First;
            foreach (T item in this.rest)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
