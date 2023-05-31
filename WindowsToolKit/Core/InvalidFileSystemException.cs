using System;
using System.IO;
using System.Runtime.Serialization;

namespace WindowsToolKit
{
    /// <summary>
    /// Exception thrown when some invalid file system data is found, indicating probably corruption.
    /// </summary>
    [Serializable]
    public class InvalidFileSystemException : IOException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidFileSystemException class.
        /// </summary>
        public InvalidFileSystemException() { }

        /// <summary>
        /// Initializes a new instance of the InvalidFileSystemException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidFileSystemException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the InvalidFileSystemException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidFileSystemException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the InvalidFileSystemException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidFileSystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}