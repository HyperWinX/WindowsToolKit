using System.IO;

namespace WindowsToolKit
{
    /// <summary>
    /// Base class for objects that validate file system integrity.
    /// </summary>
    /// <remarks>Instances of this class do not offer the ability to fix/correct
    /// file system issues, just to perform a limited number of checks on
    /// integrity of the file system.</remarks>
    public abstract class DiscFileSystemChecker
    {
        /// <summary>
        /// Checks the integrity of a file system held in a stream.
        /// </summary>
        /// <param name="reportOutput">A report on issues found.</param>
        /// <param name="levels">The amount of detail to report.</param>
        /// <returns><c>true</c> if the file system appears valid, else <c>false</c>.</returns>
        public abstract bool Check(TextWriter reportOutput, ReportLevels levels);
    }
}