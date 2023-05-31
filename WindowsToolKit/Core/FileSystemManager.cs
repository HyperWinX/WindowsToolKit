using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WindowsToolKit.CoreCompat;
using WindowsToolKit.Vfs;

namespace WindowsToolKit
{
    /// <summary>
    /// FileSystemManager determines which file systems are present on a volume.
    /// </summary>
    /// <remarks>
    /// The static detection methods detect default file systems.  To plug in additional
    /// file systems, create an instance of this class and call RegisterFileSystems.
    /// </remarks>
    public static class FileSystemManager
    {
        private static readonly List<VfsFileSystemFactory> _factories;

        /// <summary>
        /// Initializes a new instance of the FileSystemManager class.
        /// </summary>
        static FileSystemManager()
        {
            _factories = new List<VfsFileSystemFactory>();
        }

        /// <summary>
        /// Registers new file systems with an instance of this class.
        /// </summary>
        /// <param name="factory">The detector for the new file systems.</param>
        public static void RegisterFileSystems(VfsFileSystemFactory factory)
        {
            _factories.Add(factory);
        }

        /// <summary>
        /// Registers new file systems detected in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to inspect.</param>
        /// <remarks>
        /// To be detected, the <c>VfsFileSystemFactory</c> instances must be marked with the
        /// <c>VfsFileSystemFactoryAttribute</c>> attribute.
        /// </remarks>
        public static void RegisterFileSystems(Assembly assembly)
        {
            _factories.AddRange(DetectFactories(assembly));
        }

        /// <summary>
        /// Detect which file systems are present on a volume.
        /// </summary>
        /// <param name="volume">The volume to inspect.</param>
        /// <returns>The list of file systems detected.</returns>
        public static FileSystemInfo[] DetectFileSystems(VolumeInfo volume)
        {
            using (Stream s = volume.Open())
            {
                return DoDetect(s, volume);
            }
        }

        /// <summary>
        /// Detect which file systems are present in a stream.
        /// </summary>
        /// <param name="stream">The stream to inspect.</param>
        /// <returns>The list of file systems detected.</returns>
        public static FileSystemInfo[] DetectFileSystems(Stream stream)
        {
            return DoDetect(stream, null);
        }

        private static IEnumerable<VfsFileSystemFactory> DetectFactories(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                Attribute attrib = ReflectionHelper.GetCustomAttribute(type, typeof(VfsFileSystemFactoryAttribute), false);
                if (attrib == null)
                    continue;

                yield return (VfsFileSystemFactory)Activator.CreateInstance(type);
            }
        }

        private static FileSystemInfo[] DoDetect(Stream stream, VolumeInfo volume)
        {
            BufferedStream detectStream = new BufferedStream(stream);
            List<FileSystemInfo> detected = new List<FileSystemInfo>();

            foreach (VfsFileSystemFactory factory in _factories)
            {
                detected.AddRange(factory.Detect(detectStream, volume));
            }

            return detected.ToArray();
        }
    }
}