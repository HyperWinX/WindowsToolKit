using System;
using System.IO;
using WindowsToolKit.Internal;
using WindowsToolKit.Setup;

namespace WindowsToolKit
{
    public abstract class FileLocator
    {
        public abstract bool Exists(string fileName);

        public Stream Open(string fileName, FileMode mode, FileAccess access, FileShare share)
        {
            var args = new FileOpenEventArgs(fileName, mode, access, share, OpenFile);
            SetupHelper.OnOpeningFile(this, args);
            if (args.Result != null)
                return args.Result;
            return OpenFile(args.FileName, args.FileMode, args.FileAccess, args.FileShare);
        }

        protected abstract Stream OpenFile(string fileName, FileMode mode, FileAccess access, FileShare share);

        public abstract FileLocator GetRelativeLocator(string path);

        public abstract string GetFullPath(string path);

        public abstract string GetDirectoryFromPath(string path);

        public abstract string GetFileFromPath(string path);

        public abstract DateTime GetLastWriteTimeUtc(string path);

        public abstract bool HasCommonRoot(FileLocator other);

        public abstract string ResolveRelativePath(string path);

        internal string MakeRelativePath(FileLocator fileLocator, string path)
        {
            if (!HasCommonRoot(fileLocator))
            {
                return null;
            }

            string ourFullPath = GetFullPath(string.Empty) + @"\";
            string otherFullPath = fileLocator.GetFullPath(path);

            return Utilities.MakeRelativePath(otherFullPath, ourFullPath);
        }
    }
}