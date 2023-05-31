

using System;
using System.IO;

namespace WindowsToolKit.Streams
{
    /// <summary>
    /// Class for reading little-endian data from a stream.
    /// </summary>
    public class LittleEndianDataReader : DataReader
    {
        public LittleEndianDataReader(Stream stream)
            : base(stream) { }

        public override ushort ReadUInt16()
        {
            ReadToBuffer(sizeof(UInt16));
            return EndianUtilities.ToUInt16LittleEndian(_buffer, 0);
        }

        public override int ReadInt32()
        {
            ReadToBuffer(sizeof(Int32));
            return EndianUtilities.ToInt32LittleEndian(_buffer, 0);
        }

        public override uint ReadUInt32()
        {
            ReadToBuffer(sizeof(UInt32));
            return EndianUtilities.ToUInt32LittleEndian(_buffer, 0);
        }

        public override long ReadInt64()
        {
            ReadToBuffer(sizeof(Int64));
            return EndianUtilities.ToInt64LittleEndian(_buffer, 0);
        }

        public override ulong ReadUInt64()
        {
            ReadToBuffer(sizeof(UInt64));
            return EndianUtilities.ToUInt64LittleEndian(_buffer, 0);
        }
    }
}