using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal sealed class PosixNameSystemUseEntry : SystemUseEntry
    {
        public byte Flags;
        public string NameData;

        public PosixNameSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 5, 1);

            Flags = data[offset + 4];
            NameData = EndianUtilities.BytesToString(data, offset + 5, length - 5);
        }
    }
}