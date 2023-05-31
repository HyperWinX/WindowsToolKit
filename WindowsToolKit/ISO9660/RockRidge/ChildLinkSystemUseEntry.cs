namespace WindowsToolKit.ISO9660
{
    internal sealed class ChildLinkSystemUseEntry : SystemUseEntry
    {
        public uint ChildDirLocation;
        public ChildLinkSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 12, 1);
            ChildDirLocation = IsoUtilities.ToUInt32FromBoth(data, offset + 4);
        }
    }
}