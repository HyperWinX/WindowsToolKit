

namespace WindowsToolKit.Partitions
{
    /// <summary>
    /// Enumeration of partition-table technology neutral partition types.
    /// </summary>
    public enum WellKnownPartitionType
    {
        /// <summary>
        /// Windows FAT-based partition.
        /// </summary>
        WindowsFat = 0,

        /// <summary>
        /// Windows NTFS-based partition.
        /// </summary>
        WindowsNtfs = 1,

        /// <summary>
        /// Linux native file system.
        /// </summary>
        Linux = 2,

        /// <summary>
        /// Linux swap.
        /// </summary>
        LinuxSwap = 3,

        /// <summary>
        /// Linux Logical Volume Manager (LVM).
        /// </summary>
        LinuxLvm = 4
    }
}