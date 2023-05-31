using System;
using System.Collections.Generic;
using System.Text;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal class DirectoryExtent : BuilderExtent
    {
        private readonly BuildDirectoryInfo _dirInfo;
        private readonly Encoding _enc;
        private readonly Dictionary<BuildDirectoryMember, uint> _locationTable;

        private byte[] _readCache;

        public DirectoryExtent(BuildDirectoryInfo dirInfo, Dictionary<BuildDirectoryMember, uint> locationTable,
                               Encoding enc, long start)
            : base(start, dirInfo.GetDataSize(enc))
        {
            _dirInfo = dirInfo;
            _locationTable = locationTable;
            _enc = enc;
        }

        public override void Dispose() { }

        public override void PrepareForRead()
        {
            _readCache = new byte[Length];
            _dirInfo.Write(_readCache, 0, _locationTable, _enc);
        }

        public override int Read(long diskOffset, byte[] buffer, int offset, int count)
        {
            long relPos = diskOffset - Start;

            int numRead = (int)Math.Min(count, _readCache.Length - relPos);

            Array.Copy(_readCache, (int)relPos, buffer, offset, numRead);

            return numRead;
        }

        public override void DisposeReadState()
        {
            _readCache = null;
        }
    }
}