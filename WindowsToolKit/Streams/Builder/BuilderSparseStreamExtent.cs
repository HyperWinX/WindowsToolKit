

using System.Collections.Generic;

namespace WindowsToolKit.Streams
{
    public class BuilderSparseStreamExtent : BuilderExtent
    {
        private readonly Ownership _ownership;
        private SparseStream _stream;

        public BuilderSparseStreamExtent(long start, SparseStream stream)
            : this(start, stream, Ownership.None) { }

        public BuilderSparseStreamExtent(long start, SparseStream stream, Ownership ownership)
            : base(start, stream.Length)
        {
            _stream = stream;
            _ownership = ownership;
        }

        public override IEnumerable<StreamExtent> StreamExtents
        {
            get { return StreamExtent.Offset(_stream.Extents, Start); }
        }

        public override void Dispose()
        {
            if (_stream != null && _ownership == Ownership.Dispose)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        public override void PrepareForRead() { }

        public override int Read(long diskOffset, byte[] block, int offset, int count)
        {
            _stream.Position = diskOffset - Start;
            return _stream.Read(block, offset, count);
        }

        public override void DisposeReadState() { }
    }
}