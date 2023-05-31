using System;
using System.IO;
using WindowsToolKit.Internal;

namespace WindowsToolKit
{
    internal sealed class DiscFileLocator : FileLocator
    {
        private readonly string _basePath;
        private readonly DiscFileSystem _fileSystem;

        public DiscFileLocator(DiscFileSystem fileSystem, string basePath)
        {
            _fileSystem = fileSystem;
            _basePath = basePath;
        }

        public override bool Exists(string fileName)
        {
            return _fileSystem.FileExists(Utilities.CombinePaths(_basePath, fileName));
        }

        protected override Stream OpenFile(string fileName, FileMode mode, FileAccess access, FileShare share)
        {
            return _fileSystem.OpenFile(Utilities.CombinePaths(_basePath, fileName), mode, access);
        }

        public override FileLocator GetRelativeLocator(string path)
        {
            return new DiscFileLocator(_fileSystem, Utilities.CombinePaths(_basePath, path));
        }

        public override string GetFullPath(string path)
        {
            return Utilities.CombinePaths(_basePath, path);
        }

        public override string GetDirectoryFromPath(string path)
        {
            return Utilities.GetDirectoryFromPath(path);
        }

        public override string GetFileFromPath(string path)
        {
            return Utilities.GetFileFromPath(path);
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            return _fileSystem.GetLastWriteTimeUtc(Utilities.CombinePaths(_basePath, path));
        }

        public override bool HasCommonRoot(FileLocator other)
        {
            DiscFileLocator otherDiscLocator = other as DiscFileLocator;

            if (otherDiscLocator == null)
            {
                return false;
            }

            // Common root if the same file system instance.
            return ReferenceEquals(otherDiscLocator._fileSystem, _fileSystem);
        }

        public override string ResolveRelativePath(string path)
        {
            return Utilities.ResolveRelativePath(_basePath, path);
        }
    }
}