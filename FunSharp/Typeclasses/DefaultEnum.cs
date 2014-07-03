using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunSharp.Typeclasses
{
    public abstract class DefaultEnum<T> : IEnum<T>
    {
        public T Succ(T e)
        {
            var maybeNext = this.MaybeFromEnum(e).SelectMany(i => this.MaybeFromInt(i + 1));
            return GetOrOutOfRange(maybeNext);
        }

        public T Pred(T e)
        {
            var maybePred = this.MaybeFromEnum(e).SelectMany(i => this.MaybeFromInt(i - 1));
            return GetOrOutOfRange(maybePred);
        }

        public T ToEnum(int i)
        {
            return GetOrOutOfRange(this.MaybeFromInt(i));
        }

        public int FromEnum(T e)
        {
            return GetOrOutOfRange(this.MaybeFromEnum(e));
        }

        public IEnumerable<T> EnumFrom(T e)
        {
            var maybeIdx = this.MaybeFromEnum(e);
            if (maybeIdx.HasValue)
            {
                yield return e;

                for (int idx = maybeIdx.Value + 1; ; ++idx)
                {
                    var maybeNext = this.MaybeFromInt(idx);
                    if (maybeNext.HasValue)
                    {
                        yield return maybeNext.Value;
                    }
                    else break;
                }
            }
        }

        public IEnumerable<T> EnumFromTo(T first, T last)
        {
            int minIdx = FromEnum(first);
            int maxIdx = FromEnum(last);
            return EnumRangeWithStep(minIdx, maxIdx, 1);
        }

        public IEnumerable<T> EnumFromThenTo(T min, T second, T max)
        {
            int minIdx = FromEnum(min);
            int secondIdx = FromEnum(second);
            int maxIdx = FromEnum(second);

            return EnumRangeWithStep(minIdx, maxIdx, secondIdx - minIdx);
        }

        private IEnumerable<T> EnumRangeWithStep(int first, int max, int step)
        {
            if (first > max) throw new ArgumentOutOfRangeException("Range [first, max] must be non-empty");

            //NOTE: Haskell creates an infinite sequence if step is negative while Scala creates an empty sequence.
            //Similarly if step is 0 Haskell creates an infinite sequence containing first, while scala creates a one-element sequence.
            if (step < 0) return Enumerable.Empty<T>();
            else if (step == 0) return new T[] { ToEnum(first) };
            else return EnumRangeWithStepCore(first, max, step);
        }

        private IEnumerable<T> EnumRangeWithStepCore(int min, int max, int step)
        {
            Debug.Assert(min <= max);
            Debug.Assert(step > 0);

            for (int idx = min; idx <= max; idx += step)
            {
                yield return this.ToEnum(idx);
            }
        }

        protected abstract Maybe<int> MaybeFromEnum(T value);
        protected abstract Maybe<T> MaybeFromInt(int i);

        private static A GetOrOutOfRange<A>(Maybe<A> mt)
        {
            return mt.GetOr(() => { throw new ArgumentOutOfRangeException(); });
        }
    }
}
