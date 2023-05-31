

using System;
using System.IO;

namespace WindowsToolKit.Setup
{
    internal delegate Stream FileOpenDelegate(string fileName, FileMode mode, FileAccess access, FileShare share);

    /// <summary>
    /// Event arguments for opening a file
    /// </summary>
    public class FileOpenEventArgs : EventArgs
    {
        private FileOpenDelegate _opener;

        internal FileOpenEventArgs(string fileName, FileMode mode, FileAccess access, FileShare share, FileOpenDelegate opener)
        {
            FileName = fileName;
            FileMode = mode;
            FileAccess = access;
            FileShare = share;
            _opener = opener;
        }

        /// <summary>
        /// Gets or sets the filename to open
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FileMode"/>
        /// </summary>
        public FileMode FileMode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FileAccess"/>
        /// </summary>
        public FileAccess FileAccess { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FileShare"/>
        /// </summary>
        public FileShare FileShare { get; set; }

        /// <summary>
        /// The resulting stream.
        /// </summary>
        /// <remarks>
        /// If this is set to a non null value, this stream is used instead of opening the supplied <see cref="FileName"/>
        /// </remarks>
        public Stream Result { get; set; }

        /// <summary>
        /// returns the result from the builtin FileLocator
        /// </summary>
        /// <returns></returns>
        public Stream GetFileStream()
        {
            return _opener(FileName, FileMode, FileAccess, FileShare);
        }
    }
}