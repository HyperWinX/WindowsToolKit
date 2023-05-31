using System;
using System.IO;

namespace WindowsToolKit
{
    /// <summary>
    /// Common information for Windows files.
    /// </summary>
    public class WindowsFileInformation
    {
        /// <summary>
        /// Gets or sets the last time the file was changed.
        /// </summary>
        public DateTime ChangeTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the file.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the file attributes.
        /// </summary>
        public FileAttributes FileAttributes { get; set; }

        /// <summary>
        /// Gets or sets the last access time of the file.
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets the modification time of the file.
        /// </summary>
        public DateTime LastWriteTime { get; set; }
    }
}