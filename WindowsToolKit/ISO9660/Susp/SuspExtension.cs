using System.Text;

namespace WindowsToolKit.ISO9660
{
    internal abstract class SuspExtension
    {
        public abstract string Identifier { get; }
        public abstract SystemUseEntry Parse(string name, byte length, byte version, byte[] data, int offset, Encoding encoding);
    }
}