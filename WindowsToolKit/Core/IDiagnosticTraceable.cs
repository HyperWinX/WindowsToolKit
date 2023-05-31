using System.IO;

namespace WindowsToolKit
{
    /// <summary>
    /// Interface exposed by objects that can provide a structured trace of their content.
    /// </summary>
    public interface IDiagnosticTraceable
    {
        /// <summary>
        /// Writes a diagnostic report about the state of the object to a writer.
        /// </summary>
        /// <param name="writer">The writer to send the report to.</param>
        /// <param name="linePrefix">The prefix to place at the start of each line.</param>
        void Dump(TextWriter writer, string linePrefix);
    }
}