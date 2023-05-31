using System.IO;
using System.Text;

namespace WindowsToolKit.ISO9660
{
    internal sealed class ExtensionSystemUseEntry : SystemUseEntry
    {
        public string ExtensionDescriptor;
        public string ExtensionIdentifier;
        public string ExtensionSource;
        public byte ExtensionVersion;

        public ExtensionSystemUseEntry(string name, byte length, byte version, byte[] data, int offset, Encoding encoding)
        {
            CheckAndSetCommonProperties(name, length, version, 8, 1);

            int lenId = data[offset + 4];
            int lenDescriptor = data[offset + 5];
            int lenSource = data[offset + 6];

            ExtensionVersion = data[offset + 7];

            if (length < 8 + lenId + lenDescriptor + lenSource)
            {
                throw new InvalidDataException("Invalid SUSP ER entry - too short, only " + length + " bytes - expected: " +
                                               (8 + lenId + lenDescriptor + lenSource));
            }

            ExtensionIdentifier = IsoUtilities.ReadChars(data, offset + 8, lenId, encoding);
            ExtensionDescriptor = IsoUtilities.ReadChars(data, offset + 8 + lenId, lenDescriptor, encoding);
            ExtensionSource = IsoUtilities.ReadChars(data, offset + 8 + lenId + lenDescriptor, lenSource, encoding);
        }
    }
}