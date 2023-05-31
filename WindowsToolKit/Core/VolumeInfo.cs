#if !NETSTANDARD
using System;
#endif
using WindowsToolKit.Streams;

namespace WindowsToolKit
{
    /// <summary>
    /// Base class that holds information about a disk volume.
    /// </summary>
    public abstract class VolumeInfo
#if !NETSTANDARD
        : MarshalByRefObject
#endif
    {
        internal VolumeInfo() { }

        /// <summary>
        /// Gets the one-byte BIOS type for this volume, which indicates the content.
        /// </summary>
        public abstract byte BiosType { get; }

        /// <summary>
        /// Gets the size of the volume, in bytes.
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// Gets the stable volume identity.
        /// </summary>
        /// <remarks>The stability of the identity depends the disk structure.
        /// In some cases the identity may include a simple index, when no other information
        /// is available.  Best practice is to add disks to the Volume Manager in a stable 
        /// order, if the stability of this identity is paramount.</remarks>
        public abstract string Identity { get; }

        /// <summary>
        /// Gets the disk geometry of the underlying storage medium, if any (may be null).
        /// </summary>
        public abstract Geometry PhysicalGeometry { get; }

        /// <summary>
        /// Gets the disk geometry of the underlying storage medium (as used in BIOS calls), may be null.
        /// </summary>
        public abstract Geometry BiosGeometry { get; }

        /// <summary>
        /// Gets the offset of this volume in the underlying storage medium, if any (may be Zero).
        /// </summary>
        public abstract long PhysicalStartSector { get; }

        /// <summary>
        /// Opens the volume, providing access to it's contents.
        /// </summary>
        /// <returns>Stream that can access the volume's contents.</returns>
        public abstract SparseStream Open();
    }
}