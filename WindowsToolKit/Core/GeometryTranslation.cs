namespace WindowsToolKit
{
    /// <summary>
    /// Enumeration of standard BIOS disk geometry translation methods.
    /// </summary>
    public enum GeometryTranslation
    {
        /// <summary>
        /// Apply no translation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Automatic, based on the physical geometry select the most appropriate translation.
        /// </summary>
        Auto = 1,

        /// <summary>
        /// LBA assisted translation, based on just the disk capacity.
        /// </summary>
        Lba = 2,

        /// <summary>
        /// Bit-shifting translation, based on the physical geometry of the disk.
        /// </summary>
        Large = 3
    }
}