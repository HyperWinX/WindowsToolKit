using System;
using System.Collections.Generic;
namespace WindowsToolKit.Streams
{
    public sealed class StreamExtent : IEquatable<StreamExtent>, IComparable<StreamExtent>
    {
        public StreamExtent(long start, long length)
        {
            Start = start;
            Length = length;
        }
        public long Length { get; }
        public long Start { get; }
        public int CompareTo(StreamExtent other)
        {
            if (Start > other.Start)
            {
                return 1;
            }
            if (Start == other.Start)
            {
                return 0;
            }
            return -1;
        }
        public bool Equals(StreamExtent other)
        {
            if (other == null)
            {
                return false;
            }
            return Start == other.Start && Length == other.Length;
        }
        public static IEnumerable<StreamExtent> Union(IEnumerable<StreamExtent> extents, StreamExtent other)
        {
            List<StreamExtent> otherList = new List<StreamExtent>();
            otherList.Add(other);
            return Union(extents, otherList);
        }
        public static IEnumerable<StreamExtent> Union(params IEnumerable<StreamExtent>[] streams)
        {
            long extentStart = long.MaxValue;
            long extentEnd = 0;

            // Initialize enumerations and find first stored byte position
            IEnumerator<StreamExtent>[] enums = new IEnumerator<StreamExtent>[streams.Length];
            bool[] streamsValid = new bool[streams.Length];
            int validStreamsRemaining = 0;
            for (int i = 0; i < streams.Length; ++i)
            {
                enums[i] = streams[i].GetEnumerator();
                streamsValid[i] = enums[i].MoveNext();
                if (streamsValid[i])
                {
                    ++validStreamsRemaining;
                    if (enums[i].Current.Start < extentStart)
                    {
                        extentStart = enums[i].Current.Start;
                        extentEnd = enums[i].Current.Start + enums[i].Current.Length;
                    }
                }
            }

            while (validStreamsRemaining > 0)
            {
                // Find the end of this extent
                bool foundIntersection;
                do
                {
                    foundIntersection = false;
                    validStreamsRemaining = 0;
                    for (int i = 0; i < streams.Length; ++i)
                    {
                        while (streamsValid[i] && enums[i].Current.Start + enums[i].Current.Length <= extentEnd)
                        {
                            streamsValid[i] = enums[i].MoveNext();
                        }

                        if (streamsValid[i])
                        {
                            ++validStreamsRemaining;
                        }

                        if (streamsValid[i] && enums[i].Current.Start <= extentEnd)
                        {
                            extentEnd = enums[i].Current.Start + enums[i].Current.Length;
                            foundIntersection = true;
                            streamsValid[i] = enums[i].MoveNext();
                        }
                    }
                } while (foundIntersection && validStreamsRemaining > 0);

                // Return the discovered extent
                yield return new StreamExtent(extentStart, extentEnd - extentStart);

                // Find the next extent start point
                extentStart = long.MaxValue;
                validStreamsRemaining = 0;
                for (int i = 0; i < streams.Length; ++i)
                {
                    if (streamsValid[i])
                    {
                        ++validStreamsRemaining;
                        if (enums[i].Current.Start < extentStart)
                        {
                            extentStart = enums[i].Current.Start;
                            extentEnd = enums[i].Current.Start + enums[i].Current.Length;
                        }
                    }
                }
            }
        }
        public static IEnumerable<StreamExtent> Intersect(IEnumerable<StreamExtent> extents, StreamExtent other)
        {
            List<StreamExtent> otherList = new List<StreamExtent>(1);
            otherList.Add(other);
            return Intersect(extents, otherList);
        }
        public static IEnumerable<StreamExtent> Intersect(params IEnumerable<StreamExtent>[] streams)
        {
            long extentStart = long.MinValue;
            long extentEnd = long.MaxValue;

            IEnumerator<StreamExtent>[] enums = new IEnumerator<StreamExtent>[streams.Length];
            for (int i = 0; i < streams.Length; ++i)
            {
                enums[i] = streams[i].GetEnumerator();
                if (!enums[i].MoveNext())
                {
                    // Gone past end of one stream (in practice was empty), so no intersections
                    yield break;
                }
            }

            int overlapsFound = 0;
            while (true)
            {
                // We keep cycling round the streams, until we get streams.Length continuous overlaps
                for (int i = 0; i < streams.Length; ++i)
                {
                    // Move stream on past all extents that are earlier than our candidate start point
                    while (enums[i].Current.Length == 0
                           || enums[i].Current.Start + enums[i].Current.Length <= extentStart)
                    {
                        if (!enums[i].MoveNext())
                        {
                            // Gone past end of this stream, no more intersections possible
                            yield break;
                        }
                    }

                    // If this stream has an extent that spans over the candidate start point
                    if (enums[i].Current.Start <= extentStart)
                    {
                        extentEnd = Math.Min(extentEnd, enums[i].Current.Start + enums[i].Current.Length);
                        overlapsFound++;
                    }
                    else
                    {
                        extentStart = enums[i].Current.Start;
                        extentEnd = extentStart + enums[i].Current.Length;
                        overlapsFound = 1;
                    }

                    // We've just done a complete loop of all streams, they overlapped this start position
                    // and we've cut the extent's end down to the shortest run.
                    if (overlapsFound == streams.Length)
                    {
                        yield return new StreamExtent(extentStart, extentEnd - extentStart);
                        extentStart = extentEnd;
                        extentEnd = long.MaxValue;
                        overlapsFound = 0;
                    }
                }
            }
        }
        public static IEnumerable<StreamExtent> Subtract(IEnumerable<StreamExtent> extents, StreamExtent other)
        {
            return Subtract(extents, new[] { other });
        }
        public static IEnumerable<StreamExtent> Subtract(IEnumerable<StreamExtent> a, IEnumerable<StreamExtent> b)
        {
            return Intersect(a, Invert(b));
        }
        public static IEnumerable<StreamExtent> Invert(IEnumerable<StreamExtent> extents)
        {
            StreamExtent last = new StreamExtent(0, 0);
            foreach (StreamExtent extent in extents)
            {
                // Skip over any 'noise'
                if (extent.Length == 0)
                {
                    continue;
                }

                long lastEnd = last.Start + last.Length;
                if (lastEnd < extent.Start)
                {
                    yield return new StreamExtent(lastEnd, extent.Start - lastEnd);
                }

                last = extent;
            }

            long finalEnd = last.Start + last.Length;
            if (finalEnd < long.MaxValue)
            {
                yield return new StreamExtent(finalEnd, long.MaxValue - finalEnd);
            }
        }
        public static IEnumerable<StreamExtent> Offset(IEnumerable<StreamExtent> stream, long delta)
        {
            foreach (StreamExtent extent in stream)
            {
                yield return new StreamExtent(extent.Start + delta, extent.Length);
            }
        }
        public static long BlockCount(IEnumerable<StreamExtent> stream, long blockSize)
        {
            long totalBlocks = 0;
            long lastBlock = -1;

            foreach (StreamExtent extent in stream)
            {
                if (extent.Length > 0)
                {
                    long extentStartBlock = extent.Start / blockSize;
                    long extentNextBlock = MathUtilities.Ceil(extent.Start + extent.Length, blockSize);

                    long extentNumBlocks = extentNextBlock - extentStartBlock;
                    if (extentStartBlock == lastBlock)
                    {
                        extentNumBlocks--;
                    }

                    lastBlock = extentNextBlock - 1;

                    totalBlocks += extentNumBlocks;
                }
            }

            return totalBlocks;
        }
        public static IEnumerable<Range<long, long>> Blocks(IEnumerable<StreamExtent> stream, long blockSize)
        {
            long? rangeStart = null;
            long rangeLength = 0;

            foreach (StreamExtent extent in stream)
            {
                if (extent.Length > 0)
                {
                    long extentStartBlock = extent.Start / blockSize;
                    long extentNextBlock = MathUtilities.Ceil(extent.Start + extent.Length, blockSize);

                    if (rangeStart != null && extentStartBlock > rangeStart + rangeLength)
                    {
                        // This extent is non-contiguous (in terms of blocks), so write out the last range and start new
                        yield return new Range<long, long>((long)rangeStart, rangeLength);
                        rangeStart = extentStartBlock;
                    }
                    else if (rangeStart == null)
                    {
                        // First extent, so start first range
                        rangeStart = extentStartBlock;
                    }

                    // Set the length of the current range, based on the end of this extent
                    rangeLength = extentNextBlock - (long)rangeStart;
                }
            }

            // Final range (if any ranges at all) hasn't been returned yet, so do that now
            if (rangeStart != null)
            {
                yield return new Range<long, long>((long)rangeStart, rangeLength);
            }
        }
        public static bool operator ==(StreamExtent a, StreamExtent b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }
        public static bool operator !=(StreamExtent a, StreamExtent b)
        {
            return !(a == b);
        }
        public static bool operator <(StreamExtent a, StreamExtent b)
        {
            return a.CompareTo(b) < 0;
        }
        public static bool operator >(StreamExtent a, StreamExtent b)
        {
            return a.CompareTo(b) > 0;
        }
        public override string ToString()
        {
            return "[" + Start + ":+" + Length + "]";
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as StreamExtent);
        }
        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ Length.GetHashCode();
        }
    }
}