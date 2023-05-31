namespace WindowsToolKit.ISO9660
{
    internal class VolumeDescriptorSetTerminator : BaseVolumeDescriptor
    {
        public VolumeDescriptorSetTerminator()
            : base(VolumeDescriptorType.SetTerminator, 1) { }
    }
}