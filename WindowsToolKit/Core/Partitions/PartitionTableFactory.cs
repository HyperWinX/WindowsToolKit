

using System.IO;

namespace WindowsToolKit.Partitions
{
    internal abstract class PartitionTableFactory
    {
        public abstract bool DetectIsPartitioned(Stream s);

        public abstract PartitionTable DetectPartitionTable(VirtualDisk disk);
    }
}