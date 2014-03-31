using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        int Signum(T n);

        //T FromBigInt(System.Numerics.BigInteger i);
    }

    public static class NumInstances
    {
        public static readonly INum<int> Int = new IntNum();

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
                if (n < 0) return -1;
                else if (n == 0) return 0;
                else return 1;
            }
        }
    }
}
