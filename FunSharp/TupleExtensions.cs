using System;

namespace FunSharp
{
    /// <summary>Extension methods for tuples.</summary>
    public static class TupleExtensions
    {
        public static Tuple<FR, S> SelectFirst<F, S, FR>(this Tuple<F, S> tup, Func<F, FR> mapFst)
        {
            return Tuple.Create(mapFst(tup.Item1), tup.Item2);
        }

        public static Tuple<F, SR> SelectSecond<F, S, SR>(this Tuple<F, S> tup, Func<S, SR> mapSnd)
        {
            return Tuple.Create(tup.Item1, mapSnd(tup.Item2));
        }

        public static Tuple<FR, SR> SelectBoth<F, S, FR, SR>(this Tuple<F, S> tup, Func<F, FR> mapFirst, Func<S, SR> mapSnd)
        {
            return Tuple.Create(mapFirst(tup.Item1), mapSnd(tup.Item2));
        }
    }
}
