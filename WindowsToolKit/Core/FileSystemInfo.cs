using System.IO;

namespace WindowsToolKit
{
    /// <summary>
    /// Base class holding information about a file system.
    /// </summary>
    /// <remarks>
    /// File system implementations derive from this class, to provide information about the file system.
    /// </remarks>
    public abstract class FileSystemInfo
    {
        /// <summary>
        /// Gets a one-line description of the file system.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the name of the file system.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Opens a volume using the file system.
        /// </summary>
        /// <param name="volume">The volume to access.</param>
        /// <returns>A file system instance.</returns>
        public DiscFileSystem Open(VolumeInfo volume)
        {
            return Open(volume, null);
        }

        /// <summary>
        /// Opens a stream using the file system.
        /// </summary>
        /// <param name="stream">The stream to access.</param>
        /// <returns>A file system instance.</returns>
        public DiscFileSystem Open(Stream stream)
        {
            return Open(stream, null);
        }

        /// <summary>
        /// Opens a volume using the file system.
        /// </summary>
        /// <param name="volume">The volume to access.</param>
        /// <param name="parameters">Parameters for the file system.</param>
        /// <returns>A file system instance.</returns>
        public abstract DiscFileSystem Open(VolumeInfo volume, FileSystemParameters parameters);

        /// <summary>
        /// Opens a stream using the file system.
        /// </summary>
        /// <param name="stream">The stream to access.</param>
        /// <param name="parameters">Parameters for the file system.</param>
        /// <returns>A file system instance.</returns>
        public abstract DiscFileSystem Open(Stream stream, FileSystemParameters parameters);

        /// <summary>
        /// Gets the name of the file system.
        /// </summary>
        /// <returns>The file system name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}