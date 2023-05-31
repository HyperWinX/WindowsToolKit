

using System.IO;

namespace WindowsToolKit.Vfs
{
    /// <summary>
    /// Base class for logic to detect file systems.
    /// </summary>
    public abstract class VfsFileSystemFactory
    {
        /// <summary>
        /// Detects if a stream contains any known file systems.
        /// </summary>
        /// <param name="stream">The stream to inspect.</param>
        /// <returns>A list of file systems (may be empty).</returns>
        public FileSystemInfo[] Detect(Stream stream)
        {
            return Detect(stream, null);
        }

        /// <summary>
        /// Detects if a volume contains any known file systems.
        /// </summary>
        /// <param name="volume">The volume to inspect.</param>
        /// <returns>A list of file systems (may be empty).</returns>
        public FileSystemInfo[] Detect(VolumeInfo volume)
        {
            using (Stream stream = volume.Open())
            {
                return Detect(stream, volume);
            }
        }

        /// <summary>
        /// The logic for detecting file systems.
        /// </summary>
        /// <param name="stream">The stream to inspect.</param>
        /// <param name="volumeInfo">Optionally, information about the volume.</param>
        /// <returns>A list of file systems detected (may be empty).</returns>
        public abstract FileSystemInfo[] Detect(Stream stream, VolumeInfo volumeInfo);
    }
}