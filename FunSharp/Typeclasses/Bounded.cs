using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Typeclasses
{
    public interface IBounded<T>
    {
        T MinBound { get; }
        T MaxBound { get; }
    }

    public class Bounded<T> : IBounded<T>
    {
        public Bounded(T min, T max)
        {
            this.MinBound = min;
            this.MaxBound = max;
        }

        public T MinBound { get; private set; }
        public T MaxBound { get; private set; }
    }

    public static class BoundedInstances
    {
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
    }
}
