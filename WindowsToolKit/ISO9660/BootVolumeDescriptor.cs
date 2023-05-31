using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal class BootVolumeDescriptor : BaseVolumeDescriptor
    {
        public const string ElToritoSystemIdentifier = "EL TORITO SPECIFICATION";

        public BootVolumeDescriptor(uint catalogSector)
            : base(VolumeDescriptorType.Boot, 1)
        {
            CatalogSector = catalogSector;
        }

        public BootVolumeDescriptor(byte[] src, int offset)
            : base(src, offset)
        {
            SystemId = EndianUtilities.BytesToString(src, offset + 0x7, 0x20).TrimEnd('\0');
            CatalogSector = EndianUtilities.ToUInt32LittleEndian(src, offset + 0x47);
        }

        public uint CatalogSector { get; }

        public string SystemId { get; }

        internal override void WriteTo(byte[] buffer, int offset)
        {
            base.WriteTo(buffer, offset);

            EndianUtilities.StringToBytes(ElToritoSystemIdentifier, buffer, offset + 7, 0x20);
            EndianUtilities.WriteBytesLittleEndian(CatalogSector, buffer, offset + 0x47);
        }
    }
}