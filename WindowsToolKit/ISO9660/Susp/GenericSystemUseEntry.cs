using System;

namespace WindowsToolKit.ISO9660
{
    internal sealed class GenericSystemUseEntry : SystemUseEntry
    {
        public byte[] Data;

        public GenericSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 4, 0xFF);

            Data = new byte[length - 4];
            Array.Copy(data, offset + 4, Data, 0, length - 4);
        }
    }
}