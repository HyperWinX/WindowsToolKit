namespace WindowsToolKit
{
    /// <summary>
    /// The supported Floppy Disk logical formats.
    /// </summary>
    public enum FloppyDiskType
    {
        /// <summary>
        /// 720KiB capacity disk.
        /// </summary>
        DoubleDensity = 0,

        /// <summary>
        /// 1440KiB capacity disk.
        /// </summary>
        HighDensity = 1,

        /// <summary>
        /// 2880KiB capacity disk.
        /// </summary>
        Extended = 2
    }
}