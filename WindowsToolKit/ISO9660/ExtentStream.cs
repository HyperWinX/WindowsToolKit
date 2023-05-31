using System;
using System.IO;

namespace WindowsToolKit.ISO9660
{
    internal class ExtentStream : Stream
    {
        private readonly uint _dataLength;
        private readonly byte _fileUnitSize;
        private readonly byte _interleaveGapSize;

        private readonly Stream _isoStream;
        private long _position;

        private readonly uint _startBlock;

        public ExtentStream(Stream isoStream, uint startBlock, uint dataLength, byte fileUnitSize,
                            byte interleaveGapSize)
        {
            _isoStream = isoStream;
            _startBlock = startBlock;
            _dataLength = dataLength;
            _fileUnitSize = fileUnitSize;
            _interleaveGapSize = interleaveGapSize;

            if (_fileUnitSize != 0 || _interleaveGapSize != 0)
            {
                throw new NotSupportedException("Non-contiguous extents not supported");
            }
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

        public override long Length
        {
            get { return _dataLength; }
        }

        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position > _dataLength)
            {
                return 0;
            }

            int toRead = (int)Math.Min((uint)count, _dataLength - _position);

            _isoStream.Position = _position + _startBlock * (long)IsoUtilities.SectorSize;
            int numRead = _isoStream.Read(buffer, offset, toRead);
            _position += numRead;
            return numRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos = offset;
            if (origin == SeekOrigin.Current)
            {
                newPos += _position;
            }
            else if (origin == SeekOrigin.End)
            {
                newPos += _dataLength;
            }

            _position = newPos;
            return newPos;
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