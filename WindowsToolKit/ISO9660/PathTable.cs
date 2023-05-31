using System;
using System.Collections.Generic;
using System.Text;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal class PathTable : BuilderExtent
    {
        private readonly bool _byteSwap;
        private readonly List<BuildDirectoryInfo> _dirs;
        private readonly Encoding _enc;
        private readonly Dictionary<BuildDirectoryMember, uint> _locations;

        private byte[] _readCache;

        public PathTable(bool byteSwap, Encoding enc, List<BuildDirectoryInfo> dirs,
                         Dictionary<BuildDirectoryMember, uint> locations, long start)
            : base(start, CalcLength(enc, dirs))
        {
            _byteSwap = byteSwap;
            _enc = enc;
            _dirs = dirs;
            _locations = locations;
        }

        public override void Dispose() { }

        public override void PrepareForRead()
        {
            _readCache = new byte[Length];
            int pos = 0;

            List<BuildDirectoryInfo> sortedList = new List<BuildDirectoryInfo>(_dirs);
            sortedList.Sort(BuildDirectoryInfo.PathTableSortComparison);

            Dictionary<BuildDirectoryInfo, ushort> dirNumbers = new Dictionary<BuildDirectoryInfo, ushort>(_dirs.Count);
            ushort i = 1;
            foreach (BuildDirectoryInfo di in sortedList)
            {
                dirNumbers[di] = i++;
                PathTableRecord ptr = new PathTableRecord();
                ptr.DirectoryIdentifier = di.PickName(null, _enc);
                ptr.LocationOfExtent = _locations[di];
                ptr.ParentDirectoryNumber = dirNumbers[di.Parent];

                pos += ptr.Write(_byteSwap, _enc, _readCache, pos);
            }
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

        private static uint CalcLength(Encoding enc, List<BuildDirectoryInfo> dirs)
        {
            uint length = 0;
            foreach (BuildDirectoryInfo di in dirs)
            {
                length += di.GetPathTableEntrySize(enc);
            }

            return length;
        }
    }
}