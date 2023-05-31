using System.Collections.Generic;

namespace WindowsToolKit
{
    /// <summary>
    /// Common parameters for virtual disks.
    /// </summary>
    /// <remarks>Not all attributes make sense for all kinds of disks, so some
    /// may be null.  Modifying instances of this class does not modify the
    /// disk itself.</remarks>
    public sealed class VirtualDiskParameters
    {
        /// <summary>
        /// Gets or sets the type of disk adapter.
        /// </summary>
        public GenericDiskAdapterType AdapterType { get; set; }

        /// <summary>
        /// Gets or sets the logical (aka BIOS) geometry of the disk.
        /// </summary>
        public Geometry BiosGeometry { get; set; }

        /// <summary>
        /// Gets or sets the disk capacity.
        /// </summary>
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the type of disk (optical, hard disk, etc).
        /// </summary>
        public VirtualDiskClass DiskType { get; set; }

        /// <summary>
        /// Gets a dictionary of extended parameters, that varies by disk type.
        /// </summary>
        public Dictionary<string, string> ExtendedParameters { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the physical (aka IDE) geometry of the disk.
        /// </summary>
        public Geometry Geometry { get; set; }
    }
}