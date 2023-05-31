using System.Collections.Generic;
using System.IO;
using WindowsToolKit.Vfs;

namespace WindowsToolKit.ISO9660
{
    internal class IsoContext : VfsContext
    {
        public Stream DataStream { get; set; }

        public string RockRidgeIdentifier { get; set; }

        public bool SuspDetected { get; set; }

        public List<SuspExtension> SuspExtensions { get; set; }

        public int SuspSkipBytes { get; set; }
        public CommonVolumeDescriptor VolumeDescriptor { get; set; }
    }
}