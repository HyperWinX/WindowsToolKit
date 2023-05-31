namespace WindowsToolKit
{
    /// <summary>
    /// Represents a Reparse Point, which can be associated with a file or directory.
    /// </summary>
    public sealed class ReparsePoint
    {
        /// <summary>
        /// Initializes a new instance of the ReparsePoint class.
        /// </summary>
        /// <param name="tag">The defined reparse point tag.</param>
        /// <param name="content">The reparse point's content.</param>
        public ReparsePoint(int tag, byte[] content)
        {
            Tag = tag;
            Content = content;
        }

        /// <summary>
        /// Gets or sets the reparse point's content.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the defined reparse point tag.
        /// </summary>
        public int Tag { get; set; }
    }
}