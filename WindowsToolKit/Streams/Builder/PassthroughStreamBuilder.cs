

using System.Collections.Generic;
using System.IO;

namespace WindowsToolKit.Streams
{
    public class PassthroughStreamBuilder : StreamBuilder
    {
        private readonly Stream _stream;

        public PassthroughStreamBuilder(Stream stream)
        {
            _stream = stream;
        }

        protected override List<BuilderExtent> FixExtents(out long totalLength)
        {
            _stream.Position = 0;
            List<BuilderExtent> result = new List<BuilderExtent>();
            result.Add(new BuilderStreamExtent(0, _stream));
            totalLength = _stream.Length;
            return result;
        }
    }
}