

namespace WindowsToolKit.Vfs
{
    /// <summary>
    /// Interface implemented by classes representing a directory.
    /// </summary>
    /// <typeparam name="TDirEntry">Concrete type representing directory entries.</typeparam>
    /// <typeparam name="TFile">Concrete type representing files.</typeparam>
    public interface IVfsSymlink<TDirEntry, TFile> : IVfsFile
        where TDirEntry : VfsDirEntry
        where TFile : IVfsFile
    {
        /// <summary>
        /// Gets the target path for this symlink.
        /// </summary>
        string TargetPath { get; }
    }
}