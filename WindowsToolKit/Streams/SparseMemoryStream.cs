

using System.IO;

namespace WindowsToolKit.Streams
{
    /// <summary>
    /// Provides a sparse equivalent to MemoryStream.
    /// </summary>
    public sealed class SparseMemoryStream : BufferStream
    {
        /// <summary>
        /// Initializes a new instance of the SparseMemoryStream class.
        /// </summary>
        /// <remarks>The created instance permits read and write access.</remarks>
        public SparseMemoryStream()
            : base(new SparseMemoryBuffer(16 * 1024), FileAccess.ReadWrite) { }

        /// <summary>
        /// Initializes a new instance of the SparseMemoryStream class.
        /// </summary>
        /// <param name="buffer">The buffer to use.</param>
        /// <param name="access">The access permitted to clients.</param>
        public SparseMemoryStream(SparseMemoryBuffer buffer, FileAccess access)
            : base(buffer, access) { }
    }
}