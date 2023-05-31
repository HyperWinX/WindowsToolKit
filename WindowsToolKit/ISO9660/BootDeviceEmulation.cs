namespace WindowsToolKit.ISO9660
{
    /// <summary>
    /// Enumeration of boot device emulation modes.
    /// </summary>
    public enum BootDeviceEmulation : byte
    {
        /// <summary>
        /// No emulation, the boot image is just loaded and executed.
        /// </summary>
        NoEmulation = 0x0,

        /// <summary>
        /// Emulates 1.2MB diskette image as drive A.
        /// </summary>
        Diskette1200KiB = 0x1,

        /// <summary>
        /// Emulates 1.44MB diskette image as drive A.
        /// </summary>
        Diskette1440KiB = 0x2,

        /// <summary>
        /// Emulates 2.88MB diskette image as drive A.
        /// </summary>
        Diskette2880KiB = 0x3,

        /// <summary>
        /// Emulates hard disk image as drive C.
        /// </summary>
        HardDisk = 0x4
    }
}