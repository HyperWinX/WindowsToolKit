

using System;
using System.IO;
using WindowsToolKit.Streams;

namespace WindowsToolKit.Vfs
{
    /// <summary>
    /// Base class for read-only file system implementations.
    /// </summary>
    /// <typeparam name="TDirEntry">The concrete type representing directory entries.</typeparam>
    /// <typeparam name="TFile">The concrete type representing files.</typeparam>
    /// <typeparam name="TDirectory">The concrete type representing directories.</typeparam>
    /// <typeparam name="TContext">The concrete type holding global state.</typeparam>
    public abstract class VfsReadOnlyFileSystem<TDirEntry, TFile, TDirectory, TContext> :
            VfsFileSystem<TDirEntry, TFile, TDirectory, TContext>
        where TDirEntry : VfsDirEntry
        where TFile : IVfsFile
        where TDirectory : class, IVfsDirectory<TDirEntry, TFile>, TFile
        where TContext : VfsContext
    {
        /// <summary>
        /// Initializes a new instance of the VfsReadOnlyFileSystem class.
        /// </summary>
        /// <param name="defaultOptions">The default file system options.</param>
        protected VfsReadOnlyFileSystem(DiscFileSystemOptions defaultOptions)
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