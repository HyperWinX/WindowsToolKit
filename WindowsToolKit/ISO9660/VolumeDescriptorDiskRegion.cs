using System;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal abstract class VolumeDescriptorDiskRegion : BuilderExtent
    {
        private byte[] _readCache;

        public VolumeDescriptorDiskRegion(long start)
            : base(start, IsoUtilities.SectorSize) { }

        public override void Dispose() { }

        public override void PrepareForRead()
        {
            _readCache = GetBlockData();
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

        protected abstract byte[] GetBlockData();
    }
}