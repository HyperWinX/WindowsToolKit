using System.Collections.Generic;

namespace WindowsToolKit
{
    /// <summary>
    /// Class that identifies the role of each cluster in a file system.
    /// </summary>
    public sealed class ClusterMap
    {
        private readonly object[] _clusterToFileId;
        private readonly ClusterRoles[] _clusterToRole;
        private readonly Dictionary<object, string[]> _fileIdToPaths;

        internal ClusterMap(ClusterRoles[] clusterToRole, object[] clusterToFileId,
                            Dictionary<object, string[]> fileIdToPaths)
        {
            _clusterToRole = clusterToRole;
            _clusterToFileId = clusterToFileId;
            _fileIdToPaths = fileIdToPaths;
        }

        /// <summary>
        /// Gets the role of a cluster within the file system.
        /// </summary>
        /// <param name="cluster">The cluster to inspect.</param>
        /// <returns>The clusters role (or roles).</returns>
        public ClusterRoles GetRole(long cluster)
        {
            if (_clusterToRole == null || _clusterToRole.Length < cluster)
            {
                return ClusterRoles.None;
            }
            return _clusterToRole[cluster];
        }

        /// <summary>
        /// Converts a cluster to a list of file names.
        /// </summary>
        /// <param name="cluster">The cluster to inspect.</param>
        /// <returns>A list of paths that map to the cluster.</returns>
        /// <remarks>A list is returned because on file systems with the notion of
        /// hard links, a cluster may correspond to multiple directory entries.</remarks>
        public string[] ClusterToPaths(long cluster)
        {
            if ((GetRole(cluster) & (ClusterRoles.DataFile | ClusterRoles.SystemFile)) != 0)
            {
                object fileId = _clusterToFileId[cluster];
                return _fileIdToPaths[fileId];
            }
            return new string[0];
        }
    }
}