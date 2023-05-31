using System.Collections.Generic;
using System.IO;

namespace WindowsToolKit.Internal
{
    public abstract class VirtualDiskFactory
    {
        public abstract string[] Variants { get; }

        public abstract VirtualDiskTypeInfo GetDiskTypeInformation(string variant);

        public abstract DiskImageBuilder GetImageBuilder(string variant);

        public abstract VirtualDisk CreateDisk(FileLocator locator, string variant, string path,
                                               VirtualDiskParameters diskParameters);

        public abstract VirtualDisk OpenDisk(string path, FileAccess access);

        public abstract VirtualDisk OpenDisk(FileLocator locator, string path, FileAccess access);

        public virtual VirtualDisk OpenDisk(FileLocator locator, string path, string extraInfo,
                                            Dictionary<string, string> parameters, FileAccess access)
        {
            return OpenDisk(locator, path, access);
        }

        public VirtualDisk OpenDisk(DiscFileSystem fileSystem, string path, FileAccess access)
        {
            return OpenDisk(new DiscFileLocator(fileSystem, @"\"), path, access);
        }

        public abstract VirtualDiskLayer OpenDiskLayer(FileLocator locator, string path, FileAccess access);
    }
}