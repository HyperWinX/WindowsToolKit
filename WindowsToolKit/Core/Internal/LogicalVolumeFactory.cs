using System.Collections.Generic;

namespace WindowsToolKit.Internal
{
    internal abstract class LogicalVolumeFactory
    {
        public abstract bool HandlesPhysicalVolume(PhysicalVolumeInfo volume);

        public abstract void MapDisks(IEnumerable<VirtualDisk> disks, Dictionary<string, LogicalVolumeInfo> result);
    }
}