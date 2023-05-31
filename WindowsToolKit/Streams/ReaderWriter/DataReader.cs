

using System;
using System.IO;

namespace WindowsToolKit.Streams
{
    /// <summary>
    /// Base class for reading binary data from a stream.
    /// </summary>
    public abstract class DataReader
    {
        private const int _bufferSize = sizeof(UInt64);

        protected readonly Stream _stream;

        protected byte[] _buffer;

        public DataReader(Stream stream)
        {
            _stream = stream;
        }

        public long Length
        {
            get { return _stream.Length; }
        }

        public long Position
        {
            get { return _stream.Position; }
        }

        public void Skip(int bytes)
        {
            ReadBytes(bytes);
        }

        public abstract ushort ReadUInt16();

        public abstract int ReadInt32();

        public abstract uint ReadUInt32();

        public abstract long ReadInt64();

        public abstract ulong ReadUInt64();

        public virtual byte[] ReadBytes(int count)
        {
            return StreamUtilities.ReadExact(_stream, count);
        }

        protected void ReadToBuffer(int count)
        {
            if (_buffer == null)
            {
                _buffer = new byte[_bufferSize];
            }

            StreamUtilities.ReadExact(_stream, _buffer, 0, count);
        }
    }
}