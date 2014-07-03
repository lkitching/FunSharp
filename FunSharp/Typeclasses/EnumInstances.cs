using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Typeclasses
{
    public static class EnumInstances
    {
        public static readonly IEnum<bool> Bool = new BoolEnum();
        public static readonly IEnum<int> Int = new IntEnum();

        public static IEnum<T> OfEnum<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("Cannot create instance for non-enum type");
            return new EnumEnum<T>();
        }

        private class EnumEnum<T> : DefaultEnum<T> where T : struct, IComparable, IFormattable, IConvertible
        {
            private readonly T[] values;
            private readonly Dictionary<T, int> indices;

            public EnumEnum()
            {

                this.values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
                this.indices = new Dictionary<T, int>(this.values.Length);
                for (int i = 0; i < this.values.Length; ++i)
                {
                    this.indices.Add(this.values[i], i);
                }
            }

            protected override Maybe<int> MaybeFromEnum(T value)
            {
                int idx;
                return this.indices.TryGetValue(value, out idx) ? Maybe.Some(idx) : Maybe.None<int>();
            }

            protected override Maybe<T> MaybeFromInt(int i)
            {
                return i < 0 || i >= this.values.Length ? Maybe.None<T>() : Maybe.Some(this.values[i]);
            }
        }

        private class BoolEnum : DefaultEnum<bool>
        {
            protected override Maybe<int> MaybeFromEnum(bool value)
            {
                int i = value ? 1 : 0;
                return Maybe.Some(i);
            }

            protected override Maybe<bool> MaybeFromInt(int i)
            {
                switch (i)
                {
                    case 0: return Maybe.Some(false);
                    case 1: return Maybe.Some(true);
                    default: return Maybe.None<bool>();
                }
            }
        }

        private class IntEnum : DefaultEnum<int>
        {
            protected override Maybe<int> MaybeFromEnum(int value)
            {
                return Maybe.Some(value);
            }

            protected override Maybe<int> MaybeFromInt(int i)
            {
                return Maybe.Some(i);
            }
        }
    }
}
