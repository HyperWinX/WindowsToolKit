namespace WindowsToolKit.ISO9660
{
    internal class PrimaryVolumeDescriptorRegion : VolumeDescriptorDiskRegion
    {
        private readonly PrimaryVolumeDescriptor _descriptor;

        public PrimaryVolumeDescriptorRegion(PrimaryVolumeDescriptor descriptor, long start)
            : base(start)
        {
            _descriptor = descriptor;
        }

        protected override byte[] GetBlockData()
        {
            byte[] buffer = new byte[IsoUtilities.SectorSize];
            _descriptor.WriteTo(buffer, 0);
            return buffer;
        }
    }
}