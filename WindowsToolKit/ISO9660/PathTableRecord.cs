using System.Text;
using WindowsToolKit.Internal;

namespace WindowsToolKit.ISO9660
{
    internal struct PathTableRecord
    {
        ////public byte ExtendedAttributeRecordLength;
        public uint LocationOfExtent;
        public ushort ParentDirectoryNumber;
        public string DirectoryIdentifier;

        ////public static int ReadFrom(byte[] src, int offset, bool byteSwap, Encoding enc, out PathTableRecord record)
        ////{
        ////    byte directoryIdentifierLength = src[offset + 0];
        ////    record.ExtendedAttributeRecordLength = src[offset + 1];
        ////    record.LocationOfExtent = EndianUtilities.ToUInt32LittleEndian(src, offset + 2);
        ////    record.ParentDirectoryNumber = EndianUtilities.ToUInt16LittleEndian(src, offset + 6);
        ////    record.DirectoryIdentifier = IsoUtilities.ReadChars(src, offset + 8, directoryIdentifierLength, enc);
        ////
        ////    if (byteSwap)
        ////    {
        ////        record.LocationOfExtent = Utilities.BitSwap(record.LocationOfExtent);
        ////        record.ParentDirectoryNumber = Utilities.BitSwap(record.ParentDirectoryNumber);
        ////    }
        ////
        ////    return directoryIdentifierLength + 8 + (((directoryIdentifierLength & 1) == 1) ? 1 : 0);
        ////}

        internal int Write(bool byteSwap, Encoding enc, byte[] buffer, int offset)
        {
            int nameBytes = enc.GetByteCount(DirectoryIdentifier);

            buffer[offset + 0] = (byte)nameBytes;
            buffer[offset + 1] = 0; // ExtendedAttributeRecordLength;
            IsoUtilities.ToBytesFromUInt32(buffer, offset + 2,
                byteSwap ? Utilities.BitSwap(LocationOfExtent) : LocationOfExtent);
            IsoUtilities.ToBytesFromUInt16(buffer, offset + 6,
                byteSwap ? Utilities.BitSwap(ParentDirectoryNumber) : ParentDirectoryNumber);
            IsoUtilities.WriteString(buffer, offset + 8, nameBytes, false, DirectoryIdentifier, enc);
            if ((nameBytes & 1) == 1)
            {
                buffer[offset + 8 + nameBytes] = 0;
            }

            return 8 + nameBytes + ((nameBytes & 0x1) == 1 ? 1 : 0);
        }
    }
}