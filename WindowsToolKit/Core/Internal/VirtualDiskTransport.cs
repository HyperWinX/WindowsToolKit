using System;
using System.IO;

namespace WindowsToolKit.Internal
{
    internal abstract class VirtualDiskTransport : IDisposable
    {
        public abstract bool IsRawDisk { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Connect(Uri uri, string username, string password);

        public abstract VirtualDisk OpenDisk(FileAccess access);

        public abstract FileLocator GetFileLocator();

        public abstract string GetFileName();

        public abstract string GetExtraInfo();

        protected virtual void Dispose(bool disposing) { }
    }
}