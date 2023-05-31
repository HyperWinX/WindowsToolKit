using System;

namespace WindowsToolKit
{
    /// <summary>
    /// Common file system options.
    /// </summary>
    /// <remarks>Not all options are honoured by all file systems.</remarks>
    public class DiscFileSystemOptions
    {
        /// <summary>
        /// Gets or sets the random number generator the file system should use.
        /// </summary>
        /// <remarks>This option is normally <c>null</c>, which is fine for most purposes.
        /// Use this option when you need to finely control the filesystem for
        /// reproducibility of behaviour (for example in a test harness).</remarks>
        public Random RandomNumberGenerator { get; set; }
    }
}