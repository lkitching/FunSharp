using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Typeclasses
{
    public interface IEnum<T>
    {
        T Succ(T e);
        T Pred(T e);
        T ToEnum(int i);
        int FromEnum(T e);

        IEnumerable<T> EnumFrom(T e);
        IEnumerable<T> EnumFromTo(T e);
        IEnumerable<T> EnumFromThenTo(T e);
    }
}
