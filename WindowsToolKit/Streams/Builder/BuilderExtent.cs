

using System;
using System.Collections.Generic;

namespace WindowsToolKit.Streams
{
    public abstract class BuilderExtent : IDisposable
    {
        public BuilderExtent(long start, long length)
        {
            Start = start;
            Length = length;
        }

        public long Length { get; }

        public long Start { get; }

        /// <summary>
        /// Gets the parts of the stream that are stored.
        /// </summary>
        /// <remarks>This may be an empty enumeration if all bytes are zero.</remarks>
        public virtual IEnumerable<StreamExtent> StreamExtents
        {
            get { return new[] { new StreamExtent(Start, Length) }; }
        }

        public abstract void Dispose();

        public abstract void PrepareForRead();

        public abstract int Read(long diskOffset, byte[] block, int offset, int count);

        public abstract void DisposeReadState();
    }
}