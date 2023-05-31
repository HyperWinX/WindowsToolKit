namespace WindowsToolKit
{
    /// <summary>
    /// Standard Unix-style file type.
    /// </summary>
    public enum UnixFileType
    {
        /// <summary>
        /// No type specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// A FIFO / Named Pipe.
        /// </summary>
        Fifo = 0x1,

        /// <summary>
        /// A character device.
        /// </summary>
        Character = 0x2,

        /// <summary>
        /// A normal directory.
        /// </summary>
        Directory = 0x4,

        /// <summary>
        /// A block device.
        /// </summary>
        Block = 0x6,

        /// <summary>
        /// A regular file.
        /// </summary>
        Regular = 0x8,

        /// <summary>
        /// A soft link.
        /// </summary>
        Link = 0xA,

        /// <summary>
        /// A unix socket.
        /// </summary>
        Socket = 0xC
    }
}