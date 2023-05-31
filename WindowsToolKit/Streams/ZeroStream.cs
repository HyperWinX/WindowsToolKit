

using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsToolKit.Streams
{
    /// <summary>
    /// A stream that returns Zero's.
    /// </summary>
    public class ZeroStream : MappedStream
    {
        private bool _atEof;
        private readonly long _length;
        private long _position;

        public ZeroStream(long length)
        {
            _length = length;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override IEnumerable<StreamExtent> Extents
        {
            // The stream is entirely sparse
            get { return new List<StreamExtent>(0); }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _position; }

            set
            {
                _position = value;
                _atEof = false;
            }
        }

        public override IEnumerable<StreamExtent> MapContent(long start, long length)
        {
            return new StreamExtent[0];
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position > _length)
            {
                _atEof = true;
                throw new IOException("Attempt to read beyond end of stream");
            }

            if (_position == _length)
            {
                if (_atEof)
                {
                    throw new IOException("Attempt to read beyond end of stream");
                }
                _atEof = true;
                return 0;
            }

            int numToClear = (int)Math.Min(count, _length - _position);
            Array.Clear(buffer, offset, numToClear);
            _position += numToClear;

            return numToClear;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long effectiveOffset = offset;
            if (origin == SeekOrigin.Current)
            {
                effectiveOffset += _position;
            }
            else if (origin == SeekOrigin.End)
            {
                effectiveOffset += _length;
            }

            _atEof = false;

            if (effectiveOffset < 0)
            {
                throw new IOException("Attempt to move before beginning of stream");
            }
            _position = effectiveOffset;
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}