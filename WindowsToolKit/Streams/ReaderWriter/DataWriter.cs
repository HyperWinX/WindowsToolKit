

using System;
using System.IO;

namespace WindowsToolKit.Streams
{
    public abstract class DataWriter
    {
        private const int _bufferSize = sizeof(UInt64);

        protected readonly Stream _stream;

        protected byte[] _buffer;

        public DataWriter(Stream stream)
        {
            _stream = stream;
        }

        public abstract void Write(ushort value);

        public abstract void Write(int value);

        public abstract void Write(uint value);

        public abstract void Write(long value);

        public abstract void Write(ulong value);

        public virtual void WriteBytes(byte[] value, int offset, int count)
        {
            _stream.Write(value, offset, count);
        }

        public virtual void WriteBytes(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
        }

        public virtual void Flush()
        {
            _stream.Flush();
        }

        protected void EnsureBuffer()
        {
            if (_buffer == null)
            {
                _buffer = new byte[_bufferSize];
            }
        }

        protected void FlushBuffer(int count)
        {
            _stream.Write(_buffer, 0, count);
        }
    }
}