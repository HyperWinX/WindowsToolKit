using System;

namespace WindowsToolKit.ISO9660
{
    internal sealed class FileTimeSystemUseEntry : SystemUseEntry
    {
        [Flags]
        public enum Timestamps : byte
        {
            None = 0x00,
            Creation = 0x01,
            Modify = 0x02,
            Access = 0x04,
            Attributes = 0x08,
            Backup = 0x10,
            Expiration = 0x20,
            Effective = 0x40
        }

        public DateTime AccessTime;
        public DateTime AttributesTime;
        public DateTime BackupTime;
        public DateTime CreationTime;
        public DateTime EffectiveTime;
        public DateTime ExpirationTime;
        public DateTime ModifyTime;
        public Timestamps TimestampsPresent = Timestamps.None;

        public FileTimeSystemUseEntry(string name, byte length, byte version, byte[] data, int offset)
        {
            CheckAndSetCommonProperties(name, length, version, 5, 1);

            byte flags = data[offset + 4];

            bool longForm = (flags & 0x80) != 0;
            int fieldLen = longForm ? 17 : 7;

            TimestampsPresent = (Timestamps)(flags & 0x7F);

            int pos = offset + 5;

            CreationTime = ReadTimestamp(Timestamps.Creation, data, longForm, ref pos);
            ModifyTime = ReadTimestamp(Timestamps.Modify, data, longForm, ref pos);
            AccessTime = ReadTimestamp(Timestamps.Access, data, longForm, ref pos);
            AttributesTime = ReadTimestamp(Timestamps.Attributes, data, longForm, ref pos);
            BackupTime = ReadTimestamp(Timestamps.Backup, data, longForm, ref pos);
            ExpirationTime = ReadTimestamp(Timestamps.Expiration, data, longForm, ref pos);
            EffectiveTime = ReadTimestamp(Timestamps.Effective, data, longForm, ref pos);
        }

        private DateTime ReadTimestamp(Timestamps timestamp, byte[] data, bool longForm, ref int pos)
        {
            DateTime result = DateTime.MinValue;

            if ((TimestampsPresent & timestamp) != 0)
            {
                if (longForm)
                {
                    result = IsoUtilities.ToDateTimeFromVolumeDescriptorTime(data, pos);
                    pos += 17;
                }
                else
                {
                    result = IsoUtilities.ToUTCDateTimeFromDirectoryTime(data, pos);
                    pos += 7;
                }
            }

            return result;
        }
    }
}