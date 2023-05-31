using System.Text;

namespace WindowsToolKit.ISO9660
{
    internal sealed class RockRidgeExtension : SuspExtension
    {
        public RockRidgeExtension(string identifier)
        {
            Identifier = identifier;
        }

        public override string Identifier { get; }

        public override SystemUseEntry Parse(string name, byte length, byte version, byte[] data, int offset, Encoding encoding)
        {
            switch (name)
            {
                case "PX":
                    return new PosixFileInfoSystemUseEntry(name, length, version, data, offset);

                case "NM":
                    return new PosixNameSystemUseEntry(name, length, version, data, offset);

                case "CL":
                    return new ChildLinkSystemUseEntry(name, length, version, data, offset);

                case "TF":
                    return new FileTimeSystemUseEntry(name, length, version, data, offset);

                default:
                    return new GenericSystemUseEntry(name, length, version, data, offset);
            }
        }
    }
}