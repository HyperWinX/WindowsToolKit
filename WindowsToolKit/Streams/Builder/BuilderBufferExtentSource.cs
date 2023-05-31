

namespace WindowsToolKit.Streams
{
    public class BuilderBufferExtentSource : BuilderExtentSource
    {
        private readonly byte[] _buffer;

        public BuilderBufferExtentSource(byte[] buffer)
        {
            _buffer = buffer;
        }

        public override BuilderExtent Fix(long pos)
        {
            return new BuilderBufferExtent(pos, _buffer);
        }
    }
}