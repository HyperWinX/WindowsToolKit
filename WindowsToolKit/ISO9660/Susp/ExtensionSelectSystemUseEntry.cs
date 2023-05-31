namespace WindowsToolKit.ISO9660
{
    internal sealed class ExtensionSelectSystemUseEntry : SystemUseEntry
    {
        public byte SelectedExtension;

        public ExtensionSelectSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 5, 1);

            SelectedExtension = data[offset + 4];
        }
    }
}