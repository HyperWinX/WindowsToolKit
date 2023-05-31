namespace WindowsToolKit
{
    /// <summary>
    /// Enumeration of different classes of disk.
    /// </summary>
    public enum VirtualDiskClass
    {
        /// <summary>
        /// Unknown (or unspecified) type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Hard disk.
        /// </summary>
        HardDisk = 1,

        /// <summary>
        /// Optical disk, such as CD or DVD.
        /// </summary>
        OpticalDisk = 2,

        /// <summary>
        /// Floppy disk.
        /// </summary>
        FloppyDisk = 3
    }
}