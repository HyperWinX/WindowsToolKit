using System.Text;

namespace WindowsToolKit
{
    /// <summary>
    /// Class with generic file system parameters.
    /// </summary>
    /// <remarks>Note - not all parameters apply to all types of file system.</remarks>
    public sealed class FileSystemParameters
    {
        /// <summary>
        /// Gets or sets the character encoding for file names, or <c>null</c> for default.
        /// </summary>
        /// <remarks>Some file systems, such as FAT, don't specify a particular character set for
        /// file names.  This parameter determines the character set that will be used for such
        /// file systems.</remarks>
        public Encoding FileNameEncoding { get; set; }

        /// <summary>
        /// Gets or sets the algorithm to convert file system time to UTC.
        /// </summary>
        /// <remarks>Some file system, such as FAT, don't have a defined way to convert from file system
        /// time (local time where the file system is authored) to UTC time.  This parameter determines
        /// the algorithm to use.</remarks>
        public TimeConverter TimeConverter { get; set; }
    }
}