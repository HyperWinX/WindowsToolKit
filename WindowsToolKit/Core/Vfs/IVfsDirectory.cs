

using System.Collections.Generic;

namespace WindowsToolKit.Vfs
{
    /// <summary>
    /// Interface implemented by classes representing a directory.
    /// </summary>
    /// <typeparam name="TDirEntry">Concrete type representing directory entries.</typeparam>
    /// <typeparam name="TFile">Concrete type representing files.</typeparam>
    public interface IVfsDirectory<TDirEntry, TFile> : IVfsFile
        where TDirEntry : VfsDirEntry
        where TFile : IVfsFile
    {
        /// <summary>
        /// Gets all of the directory entries.
        /// </summary>
        ICollection<TDirEntry> AllEntries { get; }

        /// <summary>
        /// Gets a self-reference, if available.
        /// </summary>
        TDirEntry Self { get; }

        /// <summary>
        /// Gets a specific directory entry, by name.
        /// </summary>
        /// <param name="name">The name of the directory entry.</param>
        /// <returns>The directory entry, or <c>null</c> if not found.</returns>
        TDirEntry GetEntryByName(string name);

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="name">The name of the file (relative to this directory).</param>
        /// <returns>The newly created file.</returns>
        TDirEntry CreateNewFile(string name);
    }
}