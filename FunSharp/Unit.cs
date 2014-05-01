using System;

namespace FunSharp
{
    /// <summary>Represents a type with one value.</summary>
    public class Unit : IEquatable<Unit>
    {
        private static readonly Unit instance = new Unit();

        private Unit() { }

        /// <summary>Unit instances are considered equal. Returns true if the argument is non-null.</summary>
        /// <param name="other">The unit instance to compare against.</param>
        /// <returns>True if <paramref name="other"/> is non-null, otherwise false.</returns>
        public bool Equals(Unit other)
        {
            return other != null;
        }

        /// <summary>Whether this instance is equal to a given object.</summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is Unit;
        }

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>Hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return 1;
        }

        /// <summary>Gets the string representation of this object.</summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            return "()";
        }

        /// <summary>Gets the unit instance.</summary>
        public static Unit Instance
        {
            get { return instance; }
        }
    }
}
