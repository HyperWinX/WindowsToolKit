

using System.IO;

namespace WindowsToolKit.Vfs
{
    /// <summary>
    /// Class holding information about a file system.
    /// </summary>
    public sealed class VfsFileSystemInfo : FileSystemInfo
    {
        private readonly VfsFileSystemOpener _openDelegate;

        /// <summary>
        /// Initializes a new instance of the VfsFileSystemInfo class.
        /// </summary>
        /// <param name="name">The name of the file system.</param>
        /// <param name="description">A one-line description of the file system.</param>
        /// <param name="openDelegate">A delegate that can open streams as the indicated file system.</param>
        public VfsFileSystemInfo(string name, string description, VfsFileSystemOpener openDelegate)
        {
            Name = name;
            Description = description;
            _openDelegate = openDelegate;
        }

        /// <summary>
        /// Gets a one-line description of the file system.
        /// </summary>
        public override string Description { get; }

        /// <summary>
        /// Gets the name of the file system.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Opens a volume using the file system.
        /// </summary>
        /// <param name="volume">The volume to access.</param>
        /// <param name="parameters">Parameters for the file system.</param>
        /// <returns>A file system instance.</returns>
        public override DiscFileSystem Open(VolumeInfo volume, FileSystemParameters parameters)
        {
            return _openDelegate(volume.Open(), volume, parameters);
        }

        /// <summary>
        /// Opens a stream using the file system.
        /// </summary>
        /// <param name="stream">The stream to access.</param>
        /// <param name="parameters">Parameters for the file system.</param>
        /// <returns>A file system instance.</returns>
        public override DiscFileSystem Open(Stream stream, FileSystemParameters parameters)
        {
            return _openDelegate(stream, null, parameters);
        }
    }
}