

using System.IO;

namespace WindowsToolKit.Partitions
{
    [PartitionTableFactory]
    internal sealed class DefaultPartitionTableFactory : PartitionTableFactory
    {
        public override bool DetectIsPartitioned(Stream s)
        {
            return BiosPartitionTable.IsValid(s);
        }

        public override PartitionTable DetectPartitionTable(VirtualDisk disk)
        {
            if (BiosPartitionTable.IsValid(disk.Content))
            {
                BiosPartitionTable table = new BiosPartitionTable(disk);
                if (table.Count == 1 && table[0].BiosType == BiosPartitionTypes.GptProtective)
                {
                    return new GuidPartitionTable(disk);
                }
                return table;
            }
            return null;
        }
    }
}