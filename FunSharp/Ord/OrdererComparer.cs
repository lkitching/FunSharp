using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Ord
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrdererComparer<T> : IComparer<T>
    {
        private readonly Orderer<T> ord;

        public OrdererComparer(Orderer<T> ordFunc)
        {
            Contract.Requires(ordFunc != null);
            this.ord = ordFunc;
        }

        public int Compare(T x, T y)
        {
            Ordering result = this.ord(x, y);
            return OrdererUtils.ToComparisonResult(result);
        }
    }
}
