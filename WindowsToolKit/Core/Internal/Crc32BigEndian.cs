namespace WindowsToolKit.Internal
{
    /// <summary>
    /// Calculates CRC32 of buffers.
    /// </summary>
    internal sealed class Crc32BigEndian : Crc32
    {
        private static readonly uint[][] Tables;

        static Crc32BigEndian()
        {
            Tables = new uint[4][];

            Tables[(int)Crc32Algorithm.Common] = CalcTable(0x04C11DB7);
            Tables[(int)Crc32Algorithm.Castagnoli] = CalcTable(0x1EDC6F41);
            Tables[(int)Crc32Algorithm.Koopman] = CalcTable(0x741B8CD7);
            Tables[(int)Crc32Algorithm.Aeronautical] = CalcTable(0x814141AB);
        }

        public Crc32BigEndian(Crc32Algorithm algorithm)
            : base(Tables[(int)algorithm]) { }

        public static uint Compute(Crc32Algorithm algorithm, byte[] buffer, int offset, int count)
        {
            return Process(Tables[(int)algorithm], 0xFFFFFFFF, buffer, offset, count) ^ 0xFFFFFFFF;
        }

        public override void Process(byte[] buffer, int offset, int count)
        {
            _value = Process(Table, _value, buffer, offset, count);
        }

        private static uint[] CalcTable(uint polynomial)
        {
            uint[] table = new uint[256];

            for (uint i = 0; i < 256; ++i)
            {
                uint crc = i << 24;

                for (int j = 8; j > 0; --j)
                {
                    if ((crc & 0x80000000) != 0)
                    {
                        crc = (crc << 1) ^ polynomial;
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }

                table[i] = crc;
            }

            return table;
        }

        private static uint Process(uint[] table, uint accumulator, byte[] buffer, int offset, int count)
        {
            uint value = accumulator;

            for (int i = 0; i < count; ++i)
            {
                byte b = buffer[offset + i];
                value = table[(value >> 24) ^ b] ^ (value << 8);
            }

            return value;
        }
    }
}