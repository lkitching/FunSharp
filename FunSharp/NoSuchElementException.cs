using System;

namespace FunSharp
{
    /// <summary>Indicates no matching element exists in a collection.</summary>
    public class NoSuchElementException : Exception
    {
        /// <summary>Creates a new instance of this class.</summary>
        public NoSuchElementException() { }

        /// <summary>Creates a new instance of this class with the given message.</summary>
        /// <param name="message">Message for the exception.</param>
        public NoSuchElementException(string message) : base(message) { }

        /// <summary>Creates a new instance of this class with the given message and inner exception.</summary>
        /// <param name="message">The message for this exception.</param>
        /// <param name="inner">The cause of this exception.</param>
        public NoSuchElementException(string message, Exception inner) : base(message, inner) { }
    }
}
