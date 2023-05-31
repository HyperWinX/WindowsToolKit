namespace WindowsToolKit.ISO9660
{
    internal sealed class PaddingSystemUseEntry : SystemUseEntry
    {
        public PaddingSystemUseEntry(string name, byte length, byte version)
        {
            CheckAndSetCommonProperties(name, length, version, 4, 1);
        }
    }
}