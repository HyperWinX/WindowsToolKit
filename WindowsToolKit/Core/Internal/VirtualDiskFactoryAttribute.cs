using System;

namespace WindowsToolKit.Internal
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VirtualDiskFactoryAttribute : Attribute
    {
        public VirtualDiskFactoryAttribute(string type, string fileExtensions)
        {
            Type = type;
            FileExtensions = fileExtensions.Replace(".", string.Empty).Split(',');
        }

        public string[] FileExtensions { get; }

        public string Type { get; }
    }
}