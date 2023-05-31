

using System;

namespace WindowsToolKit.Streams
{
    public class BuilderBufferExtent : BuilderExtent
    {
        private byte[] _buffer;
        private readonly bool _fixedBuffer;

        public BuilderBufferExtent(long start, long length)
            : base(start, length) { }

        public BuilderBufferExtent(long start, byte[] buffer)
            : base(start, buffer.Length)
        {
            _fixedBuffer = true;
            _buffer = buffer;
        }

        public override void Dispose() { }

        public override void PrepareForRead()
        {
            if (!_fixedBuffer)
            {
                _buffer = GetBuffer();
            }
        }

        public override int Read(long diskOffset, byte[] block, int offset, int count)
        {
            int startOffset = (int)(diskOffset - Start);
            int numBytes = (int)Math.Min(Length - startOffset, count);
            Array.Copy(_buffer, startOffset, block, offset, numBytes);
            return numBytes;
        }

        public override void DisposeReadState()
        {
            if (!_fixedBuffer)
            {
                _buffer = null;
            }
        }

        protected virtual byte[] GetBuffer()
        {
            throw new NotSupportedException("Derived class should implement");
        }
    }
}