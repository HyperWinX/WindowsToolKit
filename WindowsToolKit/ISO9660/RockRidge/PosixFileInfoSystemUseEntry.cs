namespace WindowsToolKit.ISO9660
{
    internal sealed class PosixFileInfoSystemUseEntry : SystemUseEntry
    {
        public uint FileMode;
        public uint GroupId;
        public uint Inode;
        public uint NumLinks;
        public uint UserId;

        public PosixFileInfoSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 36, 1);

            FileMode = IsoUtilities.ToUInt32FromBoth(data, offset + 4);
            NumLinks = IsoUtilities.ToUInt32FromBoth(data, offset + 12);
            UserId = IsoUtilities.ToUInt32FromBoth(data, offset + 20);
            GroupId = IsoUtilities.ToUInt32FromBoth(data, offset + 28);
            Inode = 0;
            if (length >= 44)
            {
                Inode = IsoUtilities.ToUInt32FromBoth(data, offset + 36);
            }
        }
    }
}