using System;
using System.Text;

namespace WindowsToolKit.ISO9660
{
    internal class BaseVolumeDescriptor
    {
        public const string Iso9660StandardIdentifier = "CD001";

        public readonly string StandardIdentifier;
        public readonly VolumeDescriptorType VolumeDescriptorType;
        public readonly byte VolumeDescriptorVersion;

        public BaseVolumeDescriptor(VolumeDescriptorType type, byte version)
        {
            VolumeDescriptorType = type;
            StandardIdentifier = "CD001";
            VolumeDescriptorVersion = version;
        }

        public BaseVolumeDescriptor(byte[] src, int offset)
        {
            VolumeDescriptorType = (VolumeDescriptorType)src[offset + 0];
            StandardIdentifier = Encoding.ASCII.GetString(src, offset + 1, 5);
            VolumeDescriptorVersion = src[offset + 6];
        }

        internal virtual void WriteTo(byte[] buffer, int offset)
        {
            Array.Clear(buffer, offset, IsoUtilities.SectorSize);
            buffer[offset] = (byte)VolumeDescriptorType;
            IsoUtilities.WriteAChars(buffer, offset + 1, 5, StandardIdentifier);
            buffer[offset + 6] = VolumeDescriptorVersion;
        }
    }
}