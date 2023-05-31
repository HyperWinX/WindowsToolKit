using System;
using System.Collections.Generic;
using System.IO;
namespace WindowsToolKit.Streams
{
    public abstract class SparseStream : Stream
    {
        public abstract IEnumerable<StreamExtent> Extents { get; }
        public static SparseStream FromStream(Stream stream, Ownership takeOwnership)
        {
            return new SparseWrapperStream(stream, takeOwnership, null);
        }
        public static SparseStream FromStream(Stream stream, Ownership takeOwnership, IEnumerable<StreamExtent> extents)
        {
            return new SparseWrapperStream(stream, takeOwnership, extents);
        }
        public static void Pump(Stream inStream, Stream outStream)
        {
            Pump(inStream, outStream, Sizes.Sector);
        }
        public static void Pump(Stream inStream, Stream outStream, int chunkSize)
        {
            StreamPump pump = new StreamPump(inStream, outStream, chunkSize);
            pump.Run();
        }
        public static SparseStream ReadOnly(SparseStream toWrap, Ownership ownership)
        {
            return new SparseReadOnlyWrapperStream(toWrap, ownership);
        }
        public virtual void Clear(int count)
        {
            Write(new byte[count], 0, count);
        }
        public virtual IEnumerable<StreamExtent> GetExtentsInRange(long start, long count)
        {
            return StreamExtent.Intersect(Extents, new[] { new StreamExtent(start, count) });
        }
        private class SparseReadOnlyWrapperStream : SparseStream
        {
            private readonly Ownership _ownsWrapped;
            private SparseStream _wrapped;
            public SparseReadOnlyWrapperStream(SparseStream wrapped, Ownership ownsWrapped)
            {
                _wrapped = wrapped;
                _ownsWrapped = ownsWrapped;
            }
            public override bool CanRead
            {
                get
                {
                    return _wrapped.CanRead;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return _wrapped.CanSeek;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }
            public override IEnumerable<StreamExtent> Extents
            {
                get
                {
                    return _wrapped.Extents;
                }
            }
            public override long Length
            {
                get
                {
                    return _wrapped.Length;
                }
            }
            public override long Position
            {
                get
                {
                    return _wrapped.Position;
                }
                set
                {
                    _wrapped.Position = value;
                }
            }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count)
            {
                return _wrapped.Read(buffer, offset, count);
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                return _wrapped.Seek(offset, origin);
            }
            public override void SetLength(long value)
            {
                throw new InvalidOperationException("Attempt to change length of read-only stream");
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new InvalidOperationException("Attempt to write to read-only stream");
            }
            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && _ownsWrapped == Ownership.Dispose && _wrapped != null)
                    {
                        _wrapped.Dispose();
                        _wrapped = null;
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
        private class SparseWrapperStream : SparseStream
        {
            private readonly List<StreamExtent> _extents;
            private readonly Ownership _ownsWrapped;
            private Stream _wrapped;
            public SparseWrapperStream(Stream wrapped, Ownership ownsWrapped, IEnumerable<StreamExtent> extents)
            {
                _wrapped = wrapped;
                _ownsWrapped = ownsWrapped;
                if (extents != null)
                    _extents = new List<StreamExtent>(extents);
            }
            public override bool CanRead
            {
                get { return _wrapped.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _wrapped.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _wrapped.CanWrite; }
            }
            public override IEnumerable<StreamExtent> Extents
            {
                get
                {
                    if (_extents != null)
                    {
                        return _extents;
                    }
                    SparseStream wrappedAsSparse = _wrapped as SparseStream;
                    if (wrappedAsSparse != null)
                    {
                        return wrappedAsSparse.Extents;
                    }
                    return new[] { new StreamExtent(0, _wrapped.Length) };
                }
            }

            public override long Length
            {
                get { return _wrapped.Length; }
            }

            public override long Position
            {
                get { return _wrapped.Position; }

                set { _wrapped.Position = value; }
            }

            public override void Flush()
            {
                _wrapped.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _wrapped.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _wrapped.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _wrapped.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (_extents != null)
                {
                    throw new InvalidOperationException("Attempt to write to stream with explicit extents");
                }

                _wrapped.Write(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && _ownsWrapped == Ownership.Dispose && _wrapped != null)
                    {
                        _wrapped.Dispose();
                        _wrapped = null;
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
    }
}