using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Linq
{
    /// <summary>Exension methods for <see cref="IEnumerable{T}"/>.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>Finds the element at the given element in a sequence.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <param name="index">The index to find.</param>
        /// <returns>Empty if <paramref name="index"/> is out of range for <paramref name="seq"/>, otherwise the item at the given index.</returns>
        public static Maybe<T> MaybeElementAt<T>(this IEnumerable<T> seq, int index)
        {
            Contract.Requires(seq != null);

            if (!MaybeInRange(seq, index)) return new Maybe<T>();
            Debug.Assert(index >= 0);

            var roList = seq as IReadOnlyList<T>;
            if (roList != null)
            {
                Debug.Assert(index < roList.Count);
                return new Maybe<T>(roList[index]);
            }

            var list = seq as IList<T>;
            if (list != null)
            {
                Debug.Assert(index < list.Count);
                return new Maybe<T>(list[index]);
            }

            //search until there are no more elements or the given index is reached
            using (var enumerator = seq.GetEnumerator())
            {
                while (enumerator.MoveNext() && (--index) >= 0) ;

                //found element if index < 0 otherwise index is out of range
                if (index < 0)
                {
                    Debug.Assert(index == -1);
                    return new Maybe<T>(enumerator.Current);
                }
                else return new Maybe<T>();
            }
        }

        /// <summary>Gets the first element in a sequence if one exists.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <returns>The first element of <paramref name="seq"/> if it is non-empty, otherwise none.</returns>
        public static Maybe<T> MaybeFirst<T>(this IEnumerable<T> seq)
        {
            return MaybeFirst<T>(seq, _ => true);
        }

        /// <summary>Gets the first element in a sequence that matches the given predicate if any exist.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <param name="predicate">The predicate to match elements with.</param>
        /// <returns>The first item in <paramref name="seq"/> to match <paramref name="predicate"/>, or none if there are no matches.</returns>
        public static Maybe<T> MaybeFirst<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
        {
            Contract.Requires(seq != null);
            Contract.Requires(predicate != null);

            foreach (T item in seq)
            {
                if (predicate(item)) return new Maybe<T>(item);
            }

            //no match
            return new Maybe<T>();
        }

        /// <summary>Gets the last element in an input sequence if any exist.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <returns>The last element in <paramref name="seq"/> if it is non-empty, otherwise none.</returns>
        public static Maybe<T> MaybeLast<T>(this IEnumerable<T> seq)
        {
            Contract.Requires(seq != null);

            var roList = seq as IReadOnlyList<T>;
            if (roList != null) return roList.Count > 0 ? new Maybe<T>(roList[roList.Count - 1]) : new Maybe<T>();

            var list = seq as IList<T>;
            if (list != null) return list.Count > 0 ? new Maybe<T>(list[list.Count - 1]) : new Maybe<T>();

            return MaybeLast<T>(seq, _ => true);
        }

        /// <summary>Gets the last element in an input sequence to match the given predicate if any exist.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <param name="predicate">The predicate to match elements with.</param>
        /// <returns>The last element in <paramref name="seq"/> to match <paramref name="predicate"/>, or none if there are no matches.</returns>
        public static Maybe<T> MaybeLast<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
        {
            Contract.Requires(seq != null);
            Contract.Requires(predicate != null);

            T lastMatch = default(T);
            bool anyMatch = false;

            foreach (T item in seq)
            {
                if (predicate(item))
                {
                    lastMatch = item;
                    anyMatch = true;
                }
            }

            return anyMatch ? new Maybe<T>(lastMatch) : new Maybe<T>();
        }

        /// <summary>Gets the single item from a sequence.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <returns>The single element of <paramref name="seq"/> or none if it contains zero or multiple elements.</returns>
        public static Maybe<T> MaybeSingle<T>(this IEnumerable<T> seq)
        {
            Contract.Requires(seq != null);

            using (var enumerator = seq.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    T value = enumerator.Current;
                    return enumerator.MoveNext() ? Maybe.None<T>() : Maybe.Some(value);
                }
                else return Maybe.None<T>();
            }
        }

        /// <summary>Gets the single element of a sequence which matches a given predicate.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="predicate">The predicate to match element of <paramref name="seq"/>.</param>
        /// <returns>The single element of <paramref name="seq"/> which matches <paramref name="predicate"/>, or none if there are zero or multiple matches.</returns>
        public static Maybe<T> MaybeSingle<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
        {
            Contract.Requires(seq != null);
            Contract.Requires(predicate != null);

            T match = default(T);
            bool anyMatch = false;

            foreach (T item in seq)
            {
                if (predicate(item))
                {
                    if (anyMatch) return Maybe.None<T>();        //duplicate match
                    else
                    {
                        match = item;
                        anyMatch = true;
                    }
                }
            }

            return anyMatch ? Maybe.Some(match) : Maybe.None<T>();
        }

        /// <summary>Gets the maximum element from a sequence if it exists.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="comp">Comparer to compare the items of the sequence.</param>
        /// <returns>The maximum element of <paramref name="seq"/> or none if the sequence is empty.</returns>
        public static Maybe<T> MaybeMax<T>(this IEnumerable<T> seq, IComparer<T> comp = null)
        {
            return MaybeComp<T>(seq, comp, Utils.Max);
        }

        /// <summary>Gets the minimum element from a sequence if it exists.</summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="comp">Compare to compare the element of the sequence.</param>
        /// <returns>The minimum element of <paramref name="seq"/> or none if the sequence is empty.</returns>
        public static Maybe<T> MaybeMin<T>(this IEnumerable<T> seq, IComparer<T> comp = null)
        {
            return MaybeComp<T>(seq, comp, Utils.Min);
        }

        private static Maybe<T> MaybeComp<T>(this IEnumerable<T> seq, IComparer<T> comp, Func<T, T, IComparer<T>, T> nextComp)
        {
            comp = comp ?? Comparer<T>.Default;

            T value = default(T);
            bool foundAny = false;

            foreach (T item in seq)
            {
                if (!foundAny)
                {
                    foundAny = true;
                    value = item;
                }
                else { value = nextComp(item, value, comp); }
            }

            return foundAny ? Maybe.Some(value) : Maybe.None<T>();
        }

        private static bool MaybeInRange<T>(IEnumerable<T> seq, int index)
        {
            if (index < 0) return false;

            //NOTE: IReadOnlyList<T> extends IReadOnlyCollection<T> so don't need to check
            var rocol = seq as IReadOnlyCollection<T>;
            if (rocol != null && index >= rocol.Count) return false;

            //NOTE: IList<T> extends ICollection<T> so don't need to check
            var col = seq as ICollection<T>;
            if (col != null && index >= col.Count) return false;

            return true;
        }
    }
}
