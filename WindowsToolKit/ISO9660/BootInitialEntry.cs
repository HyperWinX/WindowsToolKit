using System;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal class BootInitialEntry
    {
        public byte BootIndicator;
        public BootDeviceEmulation BootMediaType;
        public uint ImageStart;
        public ushort LoadSegment;
        public ushort SectorCount;
        public byte SystemType;

        public BootInitialEntry() { }

        public BootInitialEntry(byte[] buffer, int offset)
        {
            BootIndicator = buffer[offset + 0x00];
            BootMediaType = (BootDeviceEmulation)buffer[offset + 0x01];
            LoadSegment = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x02);
            SystemType = buffer[offset + 0x04];
            SectorCount = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x06);
            ImageStart = EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x08);
        }

        internal void WriteTo(byte[] buffer, int offset)
        {
            Array.Clear(buffer, offset, 0x20);
            buffer[offset + 0x00] = BootIndicator;
            buffer[offset + 0x01] = (byte)BootMediaType;
            EndianUtilities.WriteBytesLittleEndian(LoadSegment, buffer, offset + 0x02);
            buffer[offset + 0x04] = SystemType;
            EndianUtilities.WriteBytesLittleEndian(SectorCount, buffer, offset + 0x06);
            EndianUtilities.WriteBytesLittleEndian(ImageStart, buffer, offset + 0x08);
        }
    }
}