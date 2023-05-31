using System;
using System.Collections.Generic;
using WindowsToolKit.Streams;

namespace WindowsToolKit
{
    /// <summary>
    /// Represents the base layer, or a differencing layer of a VirtualDisk.
    /// </summary>
    /// <remarks>
    /// <para>VirtualDisks are composed of one or more layers - a base layer
    /// which represents the entire disk (even if not all bytes are actually stored),
    /// and a number of differencing layers that store the disk sectors that are
    /// logically different to the base layer.</para>
    /// <para>Disk Layers may not store all sectors.  Any sectors that are not stored
    /// are logically zero's (for base layers), or holes through to the layer underneath
    /// (all other layers).</para>
    /// </remarks>
    public abstract class VirtualDiskLayer : IDisposable
    {
        /// <summary>
        /// Gets the capacity of the disk (in bytes).
        /// </summary>
        public abstract long Capacity { get; }

        /// <summary>
        /// Gets and sets the logical extents that make up this layer.
        /// </summary>
        public virtual IList<VirtualDiskExtent> Extents
        {
            get { return new List<VirtualDiskExtent>(); }
        }

        /// <summary>
        /// Gets the full path to this disk layer, or empty string.
        /// </summary>
        public virtual string FullPath
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the geometry of the virtual disk layer.
        /// </summary>
        public abstract Geometry Geometry { get; }

        /// <summary>
        /// Gets a value indicating whether the layer only stores meaningful sectors.
        /// </summary>
        public abstract bool IsSparse { get; }

        /// <summary>
        /// Gets a value indicating whether this is a differential disk.
        /// </summary>
        public abstract bool NeedsParent { get; }

        /// <summary>
        /// Gets a <c>FileLocator</c> that can resolve relative paths, or <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Typically used to locate parent disks.
        /// </remarks>
        public abstract FileLocator RelativeFileLocator { get; }

        /// <summary>
        /// Disposes of this instance, freeing underlying resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the VirtualDiskLayer class.
        /// </summary>
        ~VirtualDiskLayer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the content of this layer.
        /// </summary>
        /// <param name="parent">The parent stream (if any).</param>
        /// <param name="ownsParent">Controls ownership of the parent stream.</param>
        /// <returns>The content as a stream.</returns>
        public abstract SparseStream OpenContent(SparseStream parent, Ownership ownsParent);

        /// <summary>
        /// Gets the possible locations of the parent file (if any).
        /// </summary>
        /// <returns>Array of strings, empty if no parent.</returns>
        public abstract string[] GetParentLocations();

        /// <summary>
        /// Disposes of underlying resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if running inside Dispose(), indicating
        /// graceful cleanup of all managed objects should be performed, or <c>false</c>
        /// if running inside destructor.</param>
        protected virtual void Dispose(bool disposing) { }
    }
}