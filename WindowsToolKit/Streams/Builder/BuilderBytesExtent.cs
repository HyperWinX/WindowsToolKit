

using System;

namespace WindowsToolKit.Streams
{
    public class BuilderBytesExtent : BuilderExtent
    {
        protected byte[] _data;

        public BuilderBytesExtent(long start, byte[] data)
            : base(start, data.Length)
        {
            _data = data;
        }

        protected BuilderBytesExtent(long start, long length)
            : base(start, length) { }

        public override void Dispose() { }

        public override void PrepareForRead() { }

        public override int Read(long diskOffset, byte[] block, int offset, int count)
        {
            int start = (int)Math.Min(diskOffset - Start, _data.Length);
            int numRead = Math.Min(count, _data.Length - start);

            Array.Copy(_data, start, block, offset, numRead);

            return numRead;
        }

        public override void DisposeReadState() { }
    }
}