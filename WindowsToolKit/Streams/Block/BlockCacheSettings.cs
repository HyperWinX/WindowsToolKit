namespace WindowsToolKit.Streams
{
    public sealed class BlockCacheSettings
    {
        public BlockCacheSettings()
        {
            BlockSize = (int)(4 * Sizes.OneKiB);
            ReadCacheSize = 4 * Sizes.OneMiB;
            LargeReadSize = 64 * Sizes.OneKiB;
            OptimumReadSize = (int)(64 * Sizes.OneKiB);
        }
        internal BlockCacheSettings(BlockCacheSettings settings)
        {
            BlockSize = settings.BlockSize;
            ReadCacheSize = settings.ReadCacheSize;
            LargeReadSize = settings.LargeReadSize;
            OptimumReadSize = settings.OptimumReadSize;
        }
        public int BlockSize { get; set; }
        public long LargeReadSize { get; set; }
        public int OptimumReadSize { get; set; }
        public long ReadCacheSize { get; set; }
    }
}