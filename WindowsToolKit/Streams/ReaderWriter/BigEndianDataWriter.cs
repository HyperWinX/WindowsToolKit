

using System;
using System.IO;

namespace WindowsToolKit.Streams
{
    public class BigEndianDataWriter : DataWriter
    {
        public BigEndianDataWriter(Stream stream)
            : base(stream) { }

        public override void Write(ushort value)
        {
            EnsureBuffer();
            EndianUtilities.WriteBytesBigEndian(value, _buffer, 0);
            FlushBuffer(sizeof(UInt16));
        }

        public override void Write(int value)
        {
            EnsureBuffer();
            EndianUtilities.WriteBytesBigEndian(value, _buffer, 0);
            FlushBuffer(sizeof(Int32));
        }

        public override void Write(uint value)
        {
            EnsureBuffer();
            EndianUtilities.WriteBytesBigEndian(value, _buffer, 0);
            FlushBuffer(sizeof(UInt32));
        }

        public override void Write(long value)
        {
            EnsureBuffer();
            EndianUtilities.WriteBytesBigEndian(value, _buffer, 0);
            FlushBuffer(sizeof(Int64));
        }

        public override void Write(ulong value)
        {
            EnsureBuffer();
            EndianUtilities.WriteBytesBigEndian(value, _buffer, 0);
            FlushBuffer(sizeof(UInt64));
        }
    }
}