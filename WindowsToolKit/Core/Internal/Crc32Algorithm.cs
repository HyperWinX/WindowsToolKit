namespace WindowsToolKit.Internal
{
    internal enum Crc32Algorithm
    {
        /// <summary>
        /// Used in Ethernet, PKZIP, BZIP2, Gzip, PNG, etc. (aka CRC32).
        /// </summary>
        Common = 0,

        /// <summary>
        /// Used in iSCSI, SCTP, Btrfs, Vhdx. (aka CRC32C).
        /// </summary>
        Castagnoli = 1,

        /// <summary>
        /// Unknown usage. (aka CRC32K).
        /// </summary>
        Koopman = 2,

        /// <summary>
        /// Used in AIXM.  (aka CRC32Q).
        /// </summary>
        Aeronautical = 3
    }
}