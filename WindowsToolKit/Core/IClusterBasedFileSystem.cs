using WindowsToolKit.Streams;

namespace WindowsToolKit
{
    /// <summary>
    /// Base class for all file systems based on a cluster model.
    /// </summary>
    public interface IClusterBasedFileSystem : IFileSystem
    {
        /// <summary>
        /// Gets the size (in bytes) of each cluster.
        /// </summary>
        long ClusterSize { get; }

        /// <summary>
        /// Gets the total number of clusters managed by the file system.
        /// </summary>
        long TotalClusters { get; }

        /// <summary>
        /// Converts a cluster (index) into an absolute byte position in the underlying stream.
        /// </summary>
        /// <param name="cluster">The cluster to convert.</param>
        /// <returns>The corresponding absolute byte position.</returns>
        long ClusterToOffset(long cluster);

        /// <summary>
        /// Converts an absolute byte position in the underlying stream to a cluster (index).
        /// </summary>
        /// <param name="offset">The byte position to convert.</param>
        /// <returns>The cluster containing the specified byte.</returns>
        long OffsetToCluster(long offset);

        /// <summary>
        /// Converts a file name to the list of clusters occupied by the file's data.
        /// </summary>
        /// <param name="path">The path to inspect.</param>
        /// <returns>The clusters.</returns>
        /// <remarks>Note that in some file systems, small files may not have dedicated
        /// clusters.  Only dedicated clusters will be returned.</remarks>
        Range<long, long>[] PathToClusters(string path);

        /// <summary>
        /// Converts a file name to the extents containing its data.
        /// </summary>
        /// <param name="path">The path to inspect.</param>
        /// <returns>The file extents, as absolute byte positions in the underlying stream.</returns>
        /// <remarks>Use this method with caution - not all file systems will store all bytes
        /// directly in extents.  Files may be compressed, sparse or encrypted.  This method
        /// merely indicates where file data is stored, not what's stored.</remarks>
        StreamExtent[] PathToExtents(string path);

        /// <summary>
        /// Gets an object that can convert between clusters and files.
        /// </summary>
        /// <returns>The cluster map.</returns>
        ClusterMap BuildClusterMap();
    }
}