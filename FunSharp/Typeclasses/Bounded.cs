using System;
using FunSharp.Ord;

namespace FunSharp.Typeclasses
{
    /// <summary>Represents a type with a minimum and maximum value.</summary>
    /// <typeparam name="T">The bounded type.</typeparam>
    public interface IBounded<T>
    {
        /// <summary>Gets the minimum value for <typeparam name="T"/>.</summary>
        T MinBound { get; }

        /// <summary>Gets the maximum value for <typeparam name="T"/>.</summary>
        T MaxBound { get; }
    }

    /// <summary>Implementation of <see cref="IBounded{T}"/>.</summary>
    /// <typeparam name="T">The  bounded type.</typeparam>
    public class Bounded<T> : IBounded<T>
    {
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="min">The minimum value of this type.</param>
        /// <param name="max">The maximum value of this type.</param>
        public Bounded(T min, T max)
        {
            this.MinBound = min;
            this.MaxBound = max;
        }

        /// <summary>Gets the minimum value.</summary>
        public T MinBound { get; private set; }

        /// <summary>Gets the maximum value.</summary>
        public T MaxBound { get; private set; }
    }

    public static class BoundedInstances
    {
        public static readonly IBounded<bool> Bool = new Bounded<bool>(false, true);
        public static readonly IBounded<sbyte> SByte = new Bounded<sbyte>(sbyte.MinValue, sbyte.MaxValue);
        public static readonly IBounded<byte> Byte = new Bounded<byte>(byte.MinValue, byte.MaxValue);
        public static readonly IBounded<char> Char = new Bounded<char>(char.MinValue, char.MaxValue);
        public static readonly IBounded<short> Short = new Bounded<short>(short.MinValue, short.MaxValue);
        public static readonly IBounded<ushort> UShort = new Bounded<ushort>(ushort.MinValue, ushort.MaxValue);
        public static readonly IBounded<int> Int = new Bounded<int>(int.MinValue, int.MaxValue);
        public static readonly IBounded<uint> UInt = new Bounded<uint>(uint.MinValue, uint.MaxValue);
        public static readonly IBounded<long> Long = new Bounded<long>(long.MinValue, long.MaxValue);
        public static readonly IBounded<ulong> ULong = new Bounded<ulong>(ulong.MinValue, ulong.MaxValue);
        public static readonly IBounded<float> Float = new Bounded<float>(float.MinValue, float.MaxValue);
        public static readonly IBounded<double> Double = new Bounded<double>(double.MinValue, double.MaxValue);
        public static readonly IBounded<decimal> Decimal = new Bounded<decimal>(decimal.MinValue, decimal.MaxValue);
        public static readonly IBounded<DateTime> DateTime = new Bounded<DateTime>(System.DateTime.MinValue, System.DateTime.MaxValue);
        public static readonly IBounded<Unit> Unit = new Bounded<Unit>(FunSharp.Unit.Instance, FunSharp.Unit.Instance);
        public static readonly IBounded<Ordering> Ordering = new Bounded<Ordering>(Ord.Ordering.LT, Ord.Ordering.GT);
    }
}
