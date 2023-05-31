namespace WindowsToolKit.ISO9660
{
    internal sealed class ContinuationSystemUseEntry : SystemUseEntry
    {
        public uint Block;
        public uint BlockOffset;
        public uint Length;

        public ContinuationSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 28, 1);

            Block = IsoUtilities.ToUInt32FromBoth(data, offset + 4);
            BlockOffset = IsoUtilities.ToUInt32FromBoth(data, offset + 12);
            Length = IsoUtilities.ToUInt32FromBoth(data, offset + 20);
        }
    }
}