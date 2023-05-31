using System;

namespace WindowsToolKit.Internal
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class VirtualDiskTransportAttribute : Attribute
    {
        public VirtualDiskTransportAttribute(string scheme)
        {
            Scheme = scheme;
        }

        public string Scheme { get; }
    }
}