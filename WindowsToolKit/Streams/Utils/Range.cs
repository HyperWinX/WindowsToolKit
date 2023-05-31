using System;
using System.Collections.Generic;
namespace WindowsToolKit.Streams
{
    public class Range<TOffset, TCount> : IEquatable<Range<TOffset, TCount>>
        where TOffset : IEquatable<TOffset>
        where TCount : IEquatable<TCount>
    {
        public Range(TOffset offset, TCount count)
        {
            Offset = offset;
            Count = count;
        }
        public TCount Count { get; }
        public TOffset Offset { get; }
        public static IEnumerable<Range<T, T>> Chunked<T>(IEnumerable<Range<T, T>> ranges, T chunkSize)
            where T : struct, IEquatable<T>, IComparable<T>
        {
            T? chunkStart = Numbers<T>.Zero;
            T chunkLength = Numbers<T>.Zero;

            foreach (Range<T, T> range in ranges)
            {
                if (Numbers<T>.NotEqual(range.Count, Numbers<T>.Zero))
                {
                    T rangeStart = Numbers<T>.RoundDown(range.Offset, chunkSize);
                    T rangeNext = Numbers<T>.RoundUp(Numbers<T>.Add(range.Offset, range.Count), chunkSize);

                    if (chunkStart.HasValue &&
                        Numbers<T>.GreaterThan(rangeStart, Numbers<T>.Add(chunkStart.Value, chunkLength)))
                    {
                        // This extent is non-contiguous (in terms of blocks), so write out the last range and start new
                        yield return new Range<T, T>(chunkStart.Value, chunkLength);
                        chunkStart = rangeStart;
                    }
                    else if (!chunkStart.HasValue)
                    {
                        // First extent, so start first range
                        chunkStart = rangeStart;
                    }

                    // Set the length of the current range, based on the end of this extent
                    chunkLength = Numbers<T>.Subtract(rangeNext, chunkStart.Value);
                }
            }

            // Final range (if any ranges at all) hasn't been returned yet, so do that now
            if (chunkStart.HasValue)
            {
                yield return new Range<T, T>(chunkStart.Value, chunkLength);
            }
        }

        public bool Equals(Range<TOffset, TCount> other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "[" + Offset + ":+" + Count + "]";
        }
    }
}