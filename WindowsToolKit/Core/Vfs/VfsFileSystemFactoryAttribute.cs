

using System;

namespace WindowsToolKit.Vfs
{
    /// <summary>
    /// Attribute identifying file system factory classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VfsFileSystemFactoryAttribute : Attribute { }
}