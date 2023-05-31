namespace WindowsToolKit
{
    /// <summary>
    /// Provides the base class for all file systems that support Unix semantics.
    /// </summary>
    public interface IUnixFileSystem : IFileSystem
    {
        /// <summary>
        /// Retrieves Unix-specific information about a file or directory.
        /// </summary>
        /// <param name="path">Path to the file or directory.</param>
        /// <returns>Information about the owner, group, permissions and type of the
        /// file or directory.</returns>
        UnixFileSystemInfo GetUnixFileInfo(string path);
    }
}