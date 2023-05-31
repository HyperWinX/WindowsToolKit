
using System;
using System.IO;
using System.Text;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal abstract class SystemUseEntry
    {
        public string Name;
        public byte Version;
        public static SystemUseEntry Parse(byte[] data, int offset, Encoding encoding, SuspExtension extension,
                                           out byte length)
        {
            if (data[offset] == 0)
            {
                length = 0;

                return null;
            }

            string name = EndianUtilities.BytesToString(data, offset, 2);
            length = data[offset + 2];
            byte version = data[offset + 3];

            switch (name)
            {
                case "CE":
                    return new ContinuationSystemUseEntry(name, length, version, data, offset);

                case "PD":
                    return new PaddingSystemUseEntry(name, length, version);

                case "SP":
                    return new SharingProtocolSystemUseEntry(name, length, version, data, offset);

                case "ST":
                    // Termination entry. There's no point in storing or validating this one.
                    // Return null to indicate to the caller that SUSP parsing is terminated.
                    return null;

                case "ER":
                    return new ExtensionSystemUseEntry(name, length, version, data, offset, encoding);

                case "ES":
                    return new ExtensionSelectSystemUseEntry(name, length, version, data, offset);

                case "AA":
                case "AB":
                case "AS":
                    // Placeholder support for Apple and Amiga extension records.
                    return new GenericSystemUseEntry(name, length, version, data, offset);

                default:
                    if (extension == null)
                    {
                        return new GenericSystemUseEntry(name, length, version, data, offset);
                    }

                    return extension.Parse(name, length, version, data, offset, encoding);
            }
        }

        protected void CheckAndSetCommonProperties(string name, byte length, byte version, byte minLength, byte maxVersion)
        {
            if (length < minLength)
            {
                throw new InvalidDataException("Invalid SUSP " + Name + " entry - too short, only " + length + " bytes");
            }

            if (version > maxVersion || version == 0)
            {
                throw new NotSupportedException("Unknown SUSP " + Name + " entry version: " + version);
            }

            Name = name;
            Version = version;
        }
    }
}