

using System;
using WindowsToolKit.Streams;

namespace WindowsToolKit.Partitions
{
    internal class BiosPartitionRecord : IComparable<BiosPartitionRecord>
    {
        private readonly uint _lbaOffset;

        public BiosPartitionRecord() { }

        public BiosPartitionRecord(byte[] data, int offset, uint lbaOffset, int index)
        {
            _lbaOffset = lbaOffset;

            Status = data[offset];
            StartHead = data[offset + 1];
            StartSector = (byte)(data[offset + 2] & 0x3F);
            StartCylinder = (ushort)(data[offset + 3] | ((data[offset + 2] & 0xC0) << 2));
            PartitionType = data[offset + 4];
            EndHead = data[offset + 5];
            EndSector = (byte)(data[offset + 6] & 0x3F);
            EndCylinder = (ushort)(data[offset + 7] | ((data[offset + 6] & 0xC0) << 2));
            LBAStart = EndianUtilities.ToUInt32LittleEndian(data, offset + 8);
            LBALength = EndianUtilities.ToUInt32LittleEndian(data, offset + 12);
            Index = index;
        }

        public ushort EndCylinder { get; set; }

        public byte EndHead { get; set; }

        public byte EndSector { get; set; }

        public string FriendlyPartitionType
        {
            get { return BiosPartitionTypes.ToString(PartitionType); }
        }

        public int Index { get; }

        public bool IsValid
        {
            get { return EndHead != 0 || EndSector != 0 || EndCylinder != 0 || LBALength != 0; }
        }

        public uint LBALength { get; set; }

        public uint LBAStart { get; set; }

        public uint LBAStartAbsolute
        {
            get { return LBAStart + _lbaOffset; }
        }

        public byte PartitionType { get; set; }

        public ushort StartCylinder { get; set; }

        public byte StartHead { get; set; }

        public byte StartSector { get; set; }

        public byte Status { get; set; }

        public int CompareTo(BiosPartitionRecord other)
        {
            return LBAStartAbsolute.CompareTo(other.LBAStartAbsolute);
        }

        internal void WriteTo(byte[] buffer, int offset)
        {
            buffer[offset] = Status;
            buffer[offset + 1] = StartHead;
            buffer[offset + 2] = (byte)((StartSector & 0x3F) | ((StartCylinder >> 2) & 0xC0));
            buffer[offset + 3] = (byte)StartCylinder;
            buffer[offset + 4] = PartitionType;
            buffer[offset + 5] = EndHead;
            buffer[offset + 6] = (byte)((EndSector & 0x3F) | ((EndCylinder >> 2) & 0xC0));
            buffer[offset + 7] = (byte)EndCylinder;
            EndianUtilities.WriteBytesLittleEndian(LBAStart, buffer, offset + 8);
            EndianUtilities.WriteBytesLittleEndian(LBALength, buffer, offset + 12);
        }
    }
}