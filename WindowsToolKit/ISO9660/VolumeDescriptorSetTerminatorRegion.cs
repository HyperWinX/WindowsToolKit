namespace WindowsToolKit.ISO9660
{
    internal class VolumeDescriptorSetTerminatorRegion : VolumeDescriptorDiskRegion
    {
        private readonly VolumeDescriptorSetTerminator _descriptor;

        public VolumeDescriptorSetTerminatorRegion(VolumeDescriptorSetTerminator descriptor, long start)
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