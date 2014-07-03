using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunSharp.Typeclasses
{
    public interface IEnum<T>
    {
        T Succ(T e);
        T Pred(T e);
        T ToEnum(int i);
        int FromEnum(T e);

        IEnumerable<T> EnumFrom(T e);
        IEnumerable<T> EnumFromTo(T first, T last);
        IEnumerable<T> EnumFromThenTo(T first, T second, T max);
    }
}
