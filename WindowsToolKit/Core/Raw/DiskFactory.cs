

using System;
using System.IO;
using WindowsToolKit.Internal;
using WindowsToolKit.Streams;

namespace WindowsToolKit.Raw
{
    [VirtualDiskFactory("RAW", ".img,.ima,.vfd,.flp,.bif")]
    internal sealed class DiskFactory : VirtualDiskFactory
    {
        public override string[] Variants
        {
            get { return new string[] { }; }
        }

        public override VirtualDiskTypeInfo GetDiskTypeInformation(string variant)
        {
            return MakeDiskTypeInfo();
        }

        public override DiskImageBuilder GetImageBuilder(string variant)
        {
            throw new NotSupportedException();
        }

        public override VirtualDisk CreateDisk(FileLocator locator, string variant, string path,
                                               VirtualDiskParameters diskParameters)
        {
            return Disk.Initialize(locator.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None),
                Ownership.Dispose, diskParameters.Capacity, diskParameters.Geometry);
        }

        public override VirtualDisk OpenDisk(string path, FileAccess access)
        {
            return new Disk(path, access);
        }

        public override VirtualDisk OpenDisk(FileLocator locator, string path, FileAccess access)
        {
            FileShare share = access == FileAccess.Read ? FileShare.Read : FileShare.None;
            return new Disk(locator.Open(path, FileMode.Open, access, share), Ownership.Dispose);
        }

        public override VirtualDiskLayer OpenDiskLayer(FileLocator locator, string path, FileAccess access)
        {
            return null;
        }

        internal static VirtualDiskTypeInfo MakeDiskTypeInfo()
        {
            return new VirtualDiskTypeInfo
            {
                Name = "RAW",
                Variant = string.Empty,
                CanBeHardDisk = true,
                DeterministicGeometry = true,
                PreservesBiosGeometry = false,
                CalcGeometry = c => Geometry.FromCapacity(c)
            };
        }
    }
}