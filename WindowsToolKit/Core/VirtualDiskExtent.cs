using System;
using WindowsToolKit.Streams;

namespace WindowsToolKit
{
    /// <summary>
    /// Base class represented a stored extent of a virtual disk.
    /// </summary>
    /// <remarks>
    /// Some file formats can divide a logical disk layer into multiple extents, stored in
    /// different files.  This class represents those extents.  Normally, all virtual disks
    /// have at least one extent.
    /// </remarks>
    public abstract class VirtualDiskExtent : IDisposable
    {
        /// <summary>
        /// Gets the capacity of the extent (in bytes).
        /// </summary>
        public abstract long Capacity { get; }

        /// <summary>
        /// Gets a value indicating whether the extent only stores meaningful sectors.
        /// </summary>
        public abstract bool IsSparse { get; }

        /// <summary>
        /// Gets the size of the extent (in bytes) on underlying storage.
        /// </summary>
        public abstract long StoredSize { get; }

        /// <summary>
        /// Disposes of this instance, freeing underlying resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the content of this extent.
        /// </summary>
        /// <param name="parent">The parent stream (if any).</param>
        /// <param name="ownsParent">Controls ownership of the parent stream.</param>
        /// <returns>The content as a stream.</returns>
        public abstract MappedStream OpenContent(SparseStream parent, Ownership ownsParent);

        /// <summary>
        /// Disposes of underlying resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if running inside Dispose(), indicating
        /// graceful cleanup of all managed objects should be performed, or <c>false</c>
        /// if running inside destructor.</param>
        protected virtual void Dispose(bool disposing) { }
    }
}