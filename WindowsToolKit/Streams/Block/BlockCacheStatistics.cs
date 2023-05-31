public sealed class BlockCacheStatistics
{
    public int FreeReadBlocks { get; internal set; }
    public long LargeReadsIn { get; internal set; }
    public long ReadCacheHits { get; internal set; }
    public long ReadCacheMisses { get; internal set; }
    public long TotalReadsIn { get; internal set; }
    public long TotalReadsOut { get; internal set; }
    public long TotalWritesIn { get; internal set; }
    public long UnalignedReadsIn { get; internal set; }
    public long UnalignedWritesIn { get; internal set; }
}