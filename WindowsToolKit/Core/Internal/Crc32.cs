namespace WindowsToolKit.Internal
{
    internal abstract class Crc32
    {
        protected readonly uint[] Table;
        protected uint _value;

        protected Crc32(uint[] table)
        {
            Table = table;
            _value = 0xFFFFFFFF;
        }

        public uint Value
        {
            get { return _value ^ 0xFFFFFFFF; }
        }

        public abstract void Process(byte[] buffer, int offset, int count);
    }
}