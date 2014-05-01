using System;
using System.Numerics;

namespace FunSharp.Typeclasses
{
    public interface INum<T>
    {
        T Zero { get; }
        T One { get; }

        T Plus(T a, T b);
        T Mult(T a, T b);
        T Negate(T n);
        T Abs(T n);
        T Signum(T n);

        //T FromBigInt(System.Numerics.BigInteger i);
    }

    public static class NumInstances
    {
        public static readonly INum<sbyte> SByte = new ByteNum();
        public static readonly INum<short> Short = new ShortNum();
        public static readonly INum<int> Int = new IntNum();
        public static readonly INum<long> Long = new LongNum();
        public static readonly INum<float> Float = new FloatNum();
        public static readonly INum<double> Double = new DoubleNum();
        public static readonly INum<decimal> Decimal = new DecimalNum();
        public static readonly INum<BigInteger> BigInteger = new BigIntNum();

        private class ByteNum : INum<sbyte>
        {
            public sbyte Zero
            {
                get { return 0; }
            }

            public sbyte One
            {
                get { return 1; }
            }

            public sbyte Plus(sbyte a, sbyte b)
            {
                return (sbyte)(a + b);
            }

            public sbyte Mult(sbyte a, sbyte b)
            {
                return (sbyte)(a * b);
            }

            public sbyte Negate(sbyte n)
            {
                return (sbyte)-n;
            }

            public sbyte Abs(sbyte n)
            {
                return (sbyte)Math.Abs(n);
            }

            public sbyte Signum(sbyte n)
            {
                return (sbyte)Math.Sign(n);
            }
        }

        private class ShortNum : INum<short>
        {
            public short Zero
            {
                get { return 0; }
            }

            public short One
            {
                get { return 1; }
            }

            public short Plus(short a, short b)
            {
                return (short)(a + b);
            }

            public short Mult(short a, short b)
            {
                return (short)(a * b);
            }

            public short Negate(short n)
            {
                return (short)-n;
            }

            public short Abs(short n)
            {
                return Math.Abs(n);
            }

            public short Signum(short n)
            {
                return (short)Math.Sign(n);
            }
        }

        private class IntNum : INum<int>
        {
            public int Zero { get { return 0; } }

            public int One { get { return 1; } }

            public int Plus(int a, int b)
            {
                return a + b;
            }

            public int Mult(int a, int b)
            {
                return a * b;
            }

            public int Negate(int n)
            {
                return -n;
            }

            public int Abs(int n)
            {
                return Math.Abs(n);
            }

            public int Signum(int n)
            {
                return Math.Sign(n);
            }
        }

        private class LongNum : INum<long>
        {
            public long Zero
            {
                get { return 0L; }
            }

            public long One
            {
                get { return 1L; }
            }

            public long Plus(long a, long b)
            {
                return a + b;
            }

            public long Mult(long a, long b)
            {
                return a * b;
            }

            public long Negate(long n)
            {
                return -n;
            }

            public long Abs(long n)
            {
                return Math.Abs(n);
            }

            public long Signum(long n)
            {
                return Math.Sign(n);
            }
        }

        private class FloatNum : INum<float>
        {
            public float Zero
            {
                get { return 0.0f; }
            }

            public float One
            {
                get { return 1.0f; }
            }

            public float Plus(float a, float b)
            {
                return a + b;
            }

            public float Mult(float a, float b)
            {
                return a * b;
            }

            public float Negate(float n)
            {
                return -n;
            }

            public float Abs(float n)
            {
                return Math.Abs(n);
            }

            public float Signum(float n)
            {
                return Math.Sign(n);
            }
        }

        private class DoubleNum : INum<double>
        {
            public double Zero
            {
                get { return 0.0; }
            }

            public double One
            {
                get { return 1.0; }
            }

            public double Plus(double a, double b)
            {
                return a + b;
            }

            public double Mult(double a, double b)
            {
                return a * b;
            }

            public double Negate(double n)
            {
                return -n;
            }

            public double Abs(double n)
            {
                return Math.Abs(n);
            }

            public double Signum(double n)
            {
                return Math.Sign(n);
            }
        }

        private class DecimalNum : INum<decimal>
        {
            public decimal Zero
            {
                get { return 0m; }
            }

            public decimal One
            {
                get { return 1m; }
            }

            public decimal Plus(decimal a, decimal b)
            {
                return a + b;
            }

            public decimal Mult(decimal a, decimal b)
            {
                return a * b;
            }

            public decimal Negate(decimal n)
            {
                return -n;
            }

            public decimal Abs(decimal n)
            {
                return Math.Abs(n);
            }

            public decimal Signum(decimal n)
            {
                return Math.Sign(n);
            }
        }

        private class BigIntNum : INum<BigInteger>
        {
            public BigInteger Zero
            {
                get { return BigInteger.Zero; }
            }

            public BigInteger One
            {
                get { return BigInteger.One; }
            }

            public BigInteger Plus(BigInteger a, BigInteger b)
            {
                return a + b;
            }

            public BigInteger Mult(BigInteger a, BigInteger b)
            {
                return a * b;
            }

            public BigInteger Negate(BigInteger n)
            {
                return -n;
            }

            public BigInteger Abs(BigInteger n)
            {
                return n.Sign * n;
            }

            public BigInteger Signum(BigInteger n)
            {
                return n.Sign;
            }
        }
    }
}
