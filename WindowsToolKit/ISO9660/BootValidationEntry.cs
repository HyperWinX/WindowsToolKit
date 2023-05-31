using System;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal class BootValidationEntry
    {
        private readonly byte[] _data;
        public byte HeaderId;
        public string ManfId;
        public byte PlatformId;

        public BootValidationEntry()
        {
            HeaderId = 1;
            PlatformId = 0;
            ManfId = ".Net WindowsToolKit";
        }

        public BootValidationEntry(byte[] src, int offset)
        {
            _data = new byte[32];
            Array.Copy(src, offset, _data, 0, 32);

            HeaderId = _data[0];
            PlatformId = _data[1];
            ManfId = EndianUtilities.BytesToString(_data, 4, 24).TrimEnd('\0').TrimEnd(' ');
        }

        public bool ChecksumValid
        {
            get
            {
                ushort total = 0;
                for (int i = 0; i < 16; ++i)
                {
                    total += EndianUtilities.ToUInt16LittleEndian(_data, i * 2);
                }

                return total == 0;
            }
        }

        internal void WriteTo(byte[] buffer, int offset)
        {
            Array.Clear(buffer, offset, 0x20);
            buffer[offset + 0x00] = HeaderId;
            buffer[offset + 0x01] = PlatformId;
            EndianUtilities.StringToBytes(ManfId, buffer, offset + 0x04, 24);
            buffer[offset + 0x1E] = 0x55;
            buffer[offset + 0x1F] = 0xAA;
            EndianUtilities.WriteBytesLittleEndian(CalcChecksum(buffer, offset), buffer, offset + 0x1C);
        }

        private static ushort CalcChecksum(byte[] buffer, int offset)
        {
            ushort total = 0;
            for (int i = 0; i < 16; ++i)
            {
                total += EndianUtilities.ToUInt16LittleEndian(buffer, offset + i * 2);
            }

            return (ushort)(0 - total);
        }
    }
}