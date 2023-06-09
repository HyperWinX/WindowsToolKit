using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsToolKit.ISO9660
{
    /// <summary>
    /// Provides the base class for <see cref="BuildFileInfo"/> and
    /// <see cref="BuildDirectoryInfo"/> objects that will be built into an
    /// ISO image.
    /// </summary>
    /// <remarks>Instances of this class have two names, a <see cref="Name"/>,
    /// which is the full-length Joliet name and a <see cref="ShortName"/>,
    /// which is the strictly compliant ISO 9660 name.</remarks>
    public abstract class BuildDirectoryMember
    {
        internal static readonly Comparer<BuildDirectoryMember> SortedComparison = new DirectorySortedComparison();

        /// <summary>
        /// Initializes a new instance of the BuildDirectoryMember class.
        /// </summary>
        /// <param name="name">The Joliet compliant name of the file or directory.</param>
        /// <param name="shortName">The ISO 9660 compliant name of the file or directory.</param>
        protected BuildDirectoryMember(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
            CreationTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the creation date for the file or directory, in UTC.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets the Joliet compliant name of the file or directory.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parent directory, or <c>null</c> if this is the root directory.
        /// </summary>
        public abstract BuildDirectoryInfo Parent { get; }

        /// <summary>
        /// Gets the ISO 9660 compliant name of the file or directory.
        /// </summary>
        public string ShortName { get; }

        internal string PickName(string nameOverride, Encoding enc)
        {
            if (nameOverride != null)
            {
                return nameOverride;
            }
            return enc == Encoding.ASCII ? ShortName : Name;
        }

        internal abstract long GetDataSize(Encoding enc);

        internal uint GetDirectoryRecordSize(Encoding enc)
        {
            return DirectoryRecord.CalcLength(PickName(null, enc), enc);
        }

        private class DirectorySortedComparison : Comparer<BuildDirectoryMember>
        {
            public override int Compare(BuildDirectoryMember x, BuildDirectoryMember y)
            {
                string[] xParts = x.Name.Split('.', ';');
                string[] yParts = y.Name.Split('.', ';');

                string xPart;
                string yPart;

                for (int i = 0; i < 2; ++i)
                {
                    xPart = xParts.Length > i ? xParts[i] : string.Empty;
                    yPart = yParts.Length > i ? yParts[i] : string.Empty;
                    int val = ComparePart(xPart, yPart, ' ');
                    if (val != 0)
                    {
                        return val;
                    }
                }

                xPart = xParts.Length > 2 ? xParts[2] : string.Empty;
                yPart = yParts.Length > 2 ? yParts[2] : string.Empty;
                return ComparePartBackwards(xPart, yPart, '0');
            }

            private static int ComparePart(string x, string y, char padChar)
            {
                int max = Math.Max(x.Length, y.Length);
                for (int i = 0; i < max; ++i)
                {
                    char xChar = i < x.Length ? x[i] : padChar;
                    char yChar = i < y.Length ? y[i] : padChar;

                    if (xChar != yChar)
                    {
                        return xChar - yChar;
                    }
                }

                return 0;
            }

            private static int ComparePartBackwards(string x, string y, char padChar)
            {
                int max = Math.Max(x.Length, y.Length);

                int xPad = max - x.Length;
                int yPad = max - y.Length;

                for (int i = 0; i < max; ++i)
                {
                    char xChar = i >= xPad ? x[i - xPad] : padChar;
                    char yChar = i >= yPad ? y[i - yPad] : padChar;

                    if (xChar != yChar)
                    {
                        // Note: Version numbers are in DESCENDING order!
                        return yChar - xChar;
                    }
                }

                return 0;
            }
        }
    }
}