using System.Text;

namespace WindowsToolKit.ISO9660
{
    internal sealed class GenericSuspExtension : SuspExtension
    {
        public GenericSuspExtension(string identifier)
        {
            Identifier = identifier;
        }

        public override string Identifier { get; }

        public override SystemUseEntry Parse(string name, byte length, byte version, byte[] data, int offset, Encoding encoding)
        {
            return new GenericSystemUseEntry(name, length, version, data, offset);
        }
    }
}