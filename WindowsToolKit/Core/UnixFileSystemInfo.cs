namespace WindowsToolKit
{
    /// <summary>
    /// Information about a file or directory common to most Unix systems.
    /// </summary>
    public sealed class UnixFileSystemInfo
    {
        /// <summary>
        /// Gets or sets the device id of the referenced device (for character and block devices).
        /// </summary>
        public long DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the file's type.
        /// </summary>
        public UnixFileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the group that owns this file or directory.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the file's serial number (unique within file system).
        /// </summary>
        public long Inode { get; set; }

        /// <summary>
        /// Gets or sets the number of hard links to this file.
        /// </summary>
        public int LinkCount { get; set; }

        /// <summary>
        /// Gets or sets the file permissions (aka flags) for this file or directory.
        /// </summary>
        public UnixFilePermissions Permissions { get; set; }

        /// <summary>
        /// Gets or sets the user that owns this file or directory.
        /// </summary>
        public int UserId { get; set; }
    }
}