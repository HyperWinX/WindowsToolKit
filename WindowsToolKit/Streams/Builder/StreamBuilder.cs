

using System.Collections.Generic;
using System.IO;

namespace WindowsToolKit.Streams
{
    /// <summary>
    /// Base class for objects that can dynamically construct a stream.
    /// </summary>
    public abstract class StreamBuilder
    {
        /// <summary>
        /// Builds a new stream.
        /// </summary>
        /// <returns>The stream created by the StreamBuilder instance.</returns>
        public virtual SparseStream Build()
        {
            long totalLength;
            List<BuilderExtent> extents = FixExtents(out totalLength);
            return new BuiltStream(totalLength, extents);
        }

        /// <summary>
        /// Writes the stream contents to an existing stream.
        /// </summary>
        /// <param name="output">The stream to write to.</param>
        public void Build(Stream output)
        {
            using (Stream src = Build())
            {
                byte[] buffer = new byte[64 * 1024];
                int numRead = src.Read(buffer, 0, buffer.Length);
                while (numRead != 0)
                {
                    output.Write(buffer, 0, numRead);
                    numRead = src.Read(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Writes the stream contents to a file.
        /// </summary>
        /// <param name="outputFile">The file to write to.</param>
        public void Build(string outputFile)
        {
            using (FileStream destStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                Build(destStream);
            }
        }

        protected abstract List<BuilderExtent> FixExtents(out long totalLength);
    }
}