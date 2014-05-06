namespace FunSharp.Ord
{
    /// <summary>Represents the result of a comparison.</summary>
    public enum Ordering
    {
        /// <summary>Elements are equal.</summary>
        EQ = 0,

        /// <summary>First element is less than the second element.</summary>
        LT = -1,

        /// <summary>First element is greater than the second element.</summary>
        GT = 1
    }
}
