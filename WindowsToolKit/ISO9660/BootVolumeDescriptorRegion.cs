namespace WindowsToolKit.ISO9660
{
    internal class BootVolumeDescriptorRegion : VolumeDescriptorDiskRegion
    {
        private readonly BootVolumeDescriptor _descriptor;

        public BootVolumeDescriptorRegion(BootVolumeDescriptor descriptor, long start)
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