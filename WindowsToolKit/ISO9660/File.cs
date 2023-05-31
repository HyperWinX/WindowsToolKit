using System;
using System.IO;
using WindowsToolKit.Streams;
using WindowsToolKit.Vfs;

namespace WindowsToolKit.ISO9660
{
    internal class File : IVfsFile
    {
        protected IsoContext _context;
        protected ReaderDirEntry _dirEntry;

        public File(IsoContext context, ReaderDirEntry dirEntry)
        {
            _context = context;
            _dirEntry = dirEntry;
        }

        public virtual byte[] SystemUseData
        {
            get { return _dirEntry.Record.SystemUseData; }
        }

        public UnixFileSystemInfo UnixFileInfo
        {
            get
            {
                if (!_context.SuspDetected || string.IsNullOrEmpty(_context.RockRidgeIdentifier))
                {
                    throw new InvalidOperationException("No RockRidge file information available");
                }

                SuspRecords suspRecords = new SuspRecords(_context, SystemUseData, 0);

                PosixFileInfoSystemUseEntry pfi =
                    suspRecords.GetEntry<PosixFileInfoSystemUseEntry>(_context.RockRidgeIdentifier, "PX");
                if (pfi != null)
                {
                    return new UnixFileSystemInfo
                    {
                        FileType = (UnixFileType)((pfi.FileMode >> 12) & 0xff),
                        Permissions = (UnixFilePermissions)(pfi.FileMode & 0xfff),
                        UserId = (int)pfi.UserId,
                        GroupId = (int)pfi.GroupId,
                        Inode = pfi.Inode,
                        LinkCount = (int)pfi.NumLinks
                    };
                }

                throw new InvalidOperationException("No RockRidge file information available for this file");
            }
        }

        public DateTime LastAccessTimeUtc
        {
            get { return _dirEntry.LastAccessTimeUtc; }

            set { throw new NotSupportedException(); }
        }

        public DateTime LastWriteTimeUtc
        {
            get { return _dirEntry.LastWriteTimeUtc; }

            set { throw new NotSupportedException(); }
        }

        public DateTime CreationTimeUtc
        {
            get { return _dirEntry.CreationTimeUtc; }

            set { throw new NotSupportedException(); }
        }

        public FileAttributes FileAttributes
        {
            get { return _dirEntry.FileAttributes; }

            set { throw new NotSupportedException(); }
        }

        public long FileLength
        {
            get { return _dirEntry.Record.DataLength; }
        }

        public IBuffer FileContent
        {
            get
            {
                ExtentStream es = new ExtentStream(_context.DataStream, _dirEntry.Record.LocationOfExtent,
                    _dirEntry.Record.DataLength, _dirEntry.Record.FileUnitSize, _dirEntry.Record.InterleaveGapSize);
                return new StreamBuffer(es, Ownership.Dispose);
            }
        }
    }
}