

using System.IO;

namespace WindowsToolKit.Streams
{
    public class BuilderStreamExtent : BuilderExtent
    {
        private readonly Ownership _ownership;
        private Stream _source;

        public BuilderStreamExtent(long start, Stream source)
            : this(start, source, Ownership.None) { }

        public BuilderStreamExtent(long start, Stream source, Ownership ownership)
            : base(start, source.Length)
        {
            _source = source;
            _ownership = ownership;
        }

        public override void Dispose()
        {
            if (_source != null && _ownership == Ownership.Dispose)
            {
                _source.Dispose();
                _source = null;
            }
        }

        public override void PrepareForRead() { }

        public override int Read(long diskOffset, byte[] block, int offset, int count)
        {
            _source.Position = diskOffset - Start;
            return _source.Read(block, offset, count);
        }

        public override void DisposeReadState() { }
    }
}