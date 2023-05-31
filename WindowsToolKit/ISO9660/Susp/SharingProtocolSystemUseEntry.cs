using System.IO;

namespace WindowsToolKit.ISO9660
{
    internal sealed class SharingProtocolSystemUseEntry : SystemUseEntry
    {
        public byte SystemAreaSkip;

        public SharingProtocolSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 7, 1);

            if (data[offset + 4] != 0xBE || data[offset + 5] != 0xEF)
            {
                throw new InvalidDataException("Invalid SUSP SP entry - invalid checksum bytes");
            }

            SystemAreaSkip = data[offset + 6];
        }
    }
}