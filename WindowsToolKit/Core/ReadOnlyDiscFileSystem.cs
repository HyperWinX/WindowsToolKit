using System;
using System.IO;
using WindowsToolKit.Streams;

namespace WindowsToolKit
{
    /// <summary>
    /// Base class for file systems that are by their nature read-only, causes NotSupportedException to be thrown
    /// from all methods that are always invalid.
    /// </summary>
    public abstract class ReadOnlyDiscFileSystem : DiscFileSystem
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyDiscFileSystem class.
        /// </summary>
        protected ReadOnlyDiscFileSystem() { }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyDiscFileSystem class.
        /// </summary>
        /// <param name="defaultOptions">The options instance to use for this file system instance.</param>
        protected ReadOnlyDiscFileSystem(DiscFileSystemOptions defaultOptions)
            : base(defaultOptions) { }

        /// <summary>
        /// Indicates whether the file system is read-only or read-write.
        /// </summary>
        /// <returns>Always false.</returns>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Copies a file - not supported on read-only file systems.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="destinationFile">The destination file.</param>
        /// <param name="overwrite">Whether to permit over-writing of an existing file.</param>
        public override void CopyFile(string sourceFile, string destinationFile, bool overwrite)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The path of the new directory.</param>
        public override void CreateDirectory(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Deletes a directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        public override void DeleteDirectory(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Deletes a file - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        public override void DeleteFile(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Moves a directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="sourceDirectoryName">The directory to move.</param>
        /// <param name="destinationDirectoryName">The target directory name.</param>
        public override void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Moves a file - not supported on read-only file systems.
        /// </summary>
        /// <param name="sourceName">The file to move.</param>
        /// <param name="destinationName">The target file name.</param>
        /// <param name="overwrite">Whether to allow an existing file to be overwritten.</param>
        public override void MoveFile(string sourceName, string destinationName, bool overwrite)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="path">The full path of the file to open.</param>
        /// <param name="mode">The file mode for the created stream.</param>
        /// <returns>The new stream.</returns>
        public override SparseStream OpenFile(string path, FileMode mode)
        {
            return OpenFile(path, mode, FileAccess.Read);
        }

        /// <summary>
        /// Sets the attributes of a file or directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The file or directory to change.</param>
        /// <param name="newValue">The new attributes of the file or directory.</param>
        public override void SetAttributes(string path, FileAttributes newValue)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the creation time (in UTC) of a file or directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The path of the file or directory.</param>
        /// <param name="newTime">The new time to set.</param>
        public override void SetCreationTimeUtc(string path, DateTime newTime)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the last access time (in UTC) of a file or directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The path of the file or directory.</param>
        /// <param name="newTime">The new time to set.</param>
        public override void SetLastAccessTimeUtc(string path, DateTime newTime)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the last modification time (in UTC) of a file or directory - not supported on read-only file systems.
        /// </summary>
        /// <param name="path">The path of the file or directory.</param>
        /// <param name="newTime">The new time to set.</param>
        public override void SetLastWriteTimeUtc(string path, DateTime newTime)
        {
            throw new NotSupportedException();
        }
    }
}