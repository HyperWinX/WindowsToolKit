using System;
using System.Collections.Generic;
using System.IO;
namespace WindowsToolKit.Streams
{
    public sealed class BlockCacheStream : SparseStream
    {
        private bool _atEof;
        private readonly int _blocksInReadBuffer;
        private readonly BlockCache<Block> _cache;
        private readonly Ownership _ownWrapped;
        private long _position;
        private readonly byte[] _readBuffer;
        private readonly BlockCacheSettings _settings;
        private readonly BlockCacheStatistics _stats;
        private SparseStream _wrappedStream;

        public BlockCacheStream(SparseStream toWrap, Ownership ownership) : this(toWrap, ownership, new BlockCacheSettings()) { }
        public BlockCacheStream(SparseStream toWrap, Ownership ownership, BlockCacheSettings settings)
        {
            if (!toWrap.CanRead)
                throw new ArgumentException("The wrapped stream does not support reading", nameof(toWrap));
            if (!toWrap.CanSeek)
                throw new ArgumentException("The wrapped stream does not support seeking", nameof(toWrap));
            _wrappedStream = toWrap;
            _ownWrapped = ownership;
            _settings = new BlockCacheSettings(settings);
            if (_settings.OptimumReadSize % _settings.BlockSize != 0)
                throw new ArgumentException("Invalid settings, OptimumReadSize must be a multiple of BlockSize", nameof(settings));
            _readBuffer = new byte[_settings.OptimumReadSize];
            _blocksInReadBuffer = _settings.OptimumReadSize / _settings.BlockSize;
            int totalBlocks = (int)(_settings.ReadCacheSize / _settings.BlockSize);
            _cache = new BlockCache<Block>(_settings.BlockSize, totalBlocks);
            _stats = new BlockCacheStatistics();
            _stats.FreeReadBlocks = totalBlocks;
        }
        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return _wrappedStream.CanWrite; } }
        public override IEnumerable<StreamExtent> Extents
        {
            get
            {
                CheckDisposed();
                return _wrappedStream.Extents;
            }
        }
        public override long Length
        {
            get
            {
                CheckDisposed();
                return _wrappedStream.Length;
            }
        }
        public override long Position
        {
            get
            {
                CheckDisposed();
                return _position;
            }
            set
            {
                CheckDisposed();
                _position = value;
            }
        }
        public BlockCacheStatistics Statistics
        {
            get
            {
                _stats.FreeReadBlocks = _cache.FreeBlockCount;
                return _stats;
            }
        }
        public override IEnumerable<StreamExtent> GetExtentsInRange(long start, long count)
        {
            CheckDisposed();
            return _wrappedStream.GetExtentsInRange(start, count);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (_position >= Length)
            {
                if (_atEof)
                    throw new IOException("Attempt to read beyond end of stream");
                _atEof = true;
                return 0;
            }
            _stats.TotalReadsIn++;
            if (count > _settings.LargeReadSize)
            {
                _stats.LargeReadsIn++;
                _stats.TotalReadsOut++;
                _wrappedStream.Position = _position;
                int numRead = _wrappedStream.Read(buffer, offset, count);
                _position = _wrappedStream.Position;
                if (_position >= Length)
                    _atEof = true;
                return numRead;
            }
            int totalBytesRead = 0;
            bool servicesFromCache = false;
            bool servicesOutsideCache = false;
            int blockSize = _settings.BlockSize;
            long firstBlock = _position / blockSize;
            int offsetInNextBlock = (int)(_position % blockSize);
            long endBlock = MathUtilities.Ceil(Math.Min(_position + count, Length), blockSize);
            int numBlocks = (int)(endBlock - firstBlock);
            if (offsetInNextBlock != 0)
                _stats.UnalignedReadsIn++;
            int blocksRead = 0;
            while (blocksRead < numBlocks)
            {
                Block block;
                while (blocksRead < numBlocks && _cache.TryGetBlock(firstBlock + blocksRead, out block))
                {
                    int bytesToRead = Math.Min(count - totalBytesRead, block.Available - offsetInNextBlock);
                    Array.Copy(block.Data, offsetInNextBlock, buffer, offset + totalBytesRead, bytesToRead);
                    offsetInNextBlock = 0;
                    totalBytesRead += bytesToRead;
                    _position += bytesToRead;
                    blocksRead++;
                    servicesFromCache = true;
                }
                if (blocksRead < numBlocks && !_cache.ContainsBlock(firstBlock + blocksRead))
                {
                    servicesOutsideCache = true;
                    int blocksToRead = 0;
                    while (blocksRead + blocksToRead < numBlocks
                        && blocksToRead < _blocksInReadBuffer
                        && !_cache.ContainsBlock(firstBlock + blocksRead + blocksToRead))
                        ++blocksToRead;
                    long readPosition = (firstBlock + blocksRead) * blockSize;
                    int bytesRead = (int)Math.Min(blocksToRead * (long)blockSize, Length - readPosition);
                    _stats.TotalReadsOut++;
                    _wrappedStream.Position = readPosition;
                    StreamUtilities.ReadExact(_wrappedStream, _readBuffer, 0, bytesRead);
                    for (int i = 0; i < blocksToRead; ++i)
                    {
                        int copyBytes = Math.Min(blockSize, bytesRead - i * blockSize);
                        block = _cache.GetBlock(firstBlock + blocksRead + i);
                        Array.Copy(_readBuffer, i * blockSize, block.Data, 0, copyBytes);
                        block.Available = copyBytes;
                        if (copyBytes < blockSize)
                            Array.Clear(_readBuffer, i * blockSize + copyBytes, blockSize - copyBytes);
                    }
                    blocksRead += blocksToRead;
                    int bytesToCopy = Math.Min(count - totalBytesRead, bytesRead - offsetInNextBlock);
                    Array.Copy(_readBuffer, offsetInNextBlock, buffer, offset + totalBytesRead, bytesToCopy);
                    totalBytesRead += bytesToCopy;
                    _position += bytesToCopy;
                    offsetInNextBlock = 0;
                }
            }
            if (_position >= Length && totalBytesRead == 0)
                _atEof = true;
            if (servicesFromCache)
                _stats.ReadCacheHits++;
            if (servicesOutsideCache)
                _stats.ReadCacheMisses++;
            return totalBytesRead;
        }
        public override void Flush()
        {
            CheckDisposed();
            _wrappedStream.Flush();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            long effectiveOffset = offset;
            if (origin == SeekOrigin.Current)
                effectiveOffset += _position;
            else if (origin == SeekOrigin.End)
                effectiveOffset += Length;
            _atEof = false;
            if (effectiveOffset < 0)
                throw new IOException("Attempt to move before beginning of disk");
            _position = effectiveOffset;
            return _position;
        }
        public override void SetLength(long value)
        {
            CheckDisposed();
            _wrappedStream.SetLength(value);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            _stats.TotalWritesIn++;

            int blockSize = _settings.BlockSize;
            long firstBlock = _position / blockSize;
            long endBlock = MathUtilities.Ceil(Math.Min(_position + count, Length), blockSize);
            int numBlocks = (int)(endBlock - firstBlock);

            try
            {
                _wrappedStream.Position = _position;
                _wrappedStream.Write(buffer, offset, count);
            }
            catch
            {
                InvalidateBlocks(firstBlock, numBlocks);
                throw;
            }

            int offsetInNextBlock = (int)(_position % blockSize);
            if (offsetInNextBlock != 0)
            {
                _stats.UnalignedWritesIn++;
            }

            int bytesProcessed = 0;
            for (int i = 0; i < numBlocks; ++i)
            {
                int bufferPos = offset + bytesProcessed;
                int bytesThisBlock = Math.Min(count - bytesProcessed, blockSize - offsetInNextBlock);

                Block block;
                if (_cache.TryGetBlock(firstBlock + i, out block))
                {
                    Array.Copy(buffer, bufferPos, block.Data, offsetInNextBlock, bytesThisBlock);
                    block.Available = Math.Max(block.Available, offsetInNextBlock + bytesThisBlock);
                }

                offsetInNextBlock = 0;
                bytesProcessed += bytesThisBlock;
            }

            _position += count;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_wrappedStream != null && _ownWrapped == Ownership.Dispose)
                    _wrappedStream.Dispose();
                _wrappedStream = null;
            }
            base.Dispose(disposing);
        }
        private void CheckDisposed()
        {
            if (_wrappedStream == null)
                throw new ObjectDisposedException("BlockCacheStream");
        }
        private void InvalidateBlocks(long firstBlock, int numBlocks)
        {
            for (long i = firstBlock; i < firstBlock + numBlocks; ++i)
                _cache.ReleaseBlock(i);
        }
    }
}