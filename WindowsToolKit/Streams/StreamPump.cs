using System;
using System.IO;

namespace WindowsToolKit.Streams
{
    public sealed class StreamPump
    {
        public StreamPump()
        {
            SparseChunkSize = 512;
            BufferSize = (int)(512 * Sizes.OneKiB);
            SparseCopy = true;
        }
        public StreamPump(Stream inStream, Stream outStream, int sparceChunkSize)
        {
            InputStream = inStream;
            OutputStream = outStream;
            SparseChunkSize = sparceChunkSize;
            BufferSize = (int)(512 * Sizes.OneKiB);
            SparseCopy = true;
        }
        public int BufferSize { get; set; }
        public long BytesRead { get; private set; }
        public long BytesWritten { get; private set; }
        public Stream InputStream { get; set; }
        public Stream OutputStream { get; set; }
        public int SparseChunkSize { get; set; }
        public bool SparseCopy { get; set; }
        public event EventHandler<PumpProgressEventArgs> ProgressEvent;
        public void Run()
        {
            if (InputStream == null)
            {
                throw new InvalidOperationException("Input stream is null");
            }

            if (OutputStream == null)
            {
                throw new InvalidOperationException("Output stream is null");
            }

            if (!OutputStream.CanSeek)
            {
                throw new InvalidOperationException("Output stream does not support seek operations");
            }

            if (SparseChunkSize <= 1)
            {
                throw new InvalidOperationException("Chunk size is invalid");
            }

            if (SparseCopy)
            {
                RunSparse();
            }
            else
            {
                RunNonSparse();
            }
        }

        private static bool IsAllZeros(byte[] buffer, int offset, int count)
        {
            for (int j = 0; j < count; j++)
            {
                if (buffer[offset + j] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void RunNonSparse()
        {
            byte[] copyBuffer = new byte[BufferSize];

            InputStream.Position = 0;
            OutputStream.Position = 0;

            int numRead = InputStream.Read(copyBuffer, 0, copyBuffer.Length);
            while (numRead > 0)
            {
                BytesRead += numRead;

                OutputStream.Write(copyBuffer, 0, numRead);
                BytesWritten += numRead;

                RaiseProgressEvent();

                numRead = InputStream.Read(copyBuffer, 0, copyBuffer.Length);
            }
        }

        private void RunSparse()
        {
            SparseStream inStream = InputStream as SparseStream;
            if (inStream == null)
            {
                inStream = SparseStream.FromStream(InputStream, Ownership.None);
            }

            if (BufferSize > SparseChunkSize && BufferSize % SparseChunkSize != 0)
            {
                throw new InvalidOperationException("Buffer size is not a multiple of the sparse chunk size");
            }

            byte[] copyBuffer = new byte[Math.Max(BufferSize, SparseChunkSize)];

            BytesRead = 0;
            BytesWritten = 0;

            foreach (StreamExtent extent in inStream.Extents)
            {
                inStream.Position = extent.Start;

                long extentOffset = 0;
                while (extentOffset < extent.Length)
                {
                    int numRead = (int)Math.Min(copyBuffer.Length, extent.Length - extentOffset);
                    StreamUtilities.ReadExact(inStream, copyBuffer, 0, numRead);
                    BytesRead += numRead;

                    int copyBufferOffset = 0;
                    for (int i = 0; i < numRead; i += SparseChunkSize)
                    {
                        if (IsAllZeros(copyBuffer, i, Math.Min(SparseChunkSize, numRead - i)))
                        {
                            if (copyBufferOffset < i)
                            {
                                OutputStream.Position = extent.Start + extentOffset + copyBufferOffset;
                                OutputStream.Write(copyBuffer, copyBufferOffset, i - copyBufferOffset);
                                BytesWritten += i - copyBufferOffset;
                            }

                            copyBufferOffset = i + SparseChunkSize;
                        }
                    }

                    if (copyBufferOffset < numRead)
                    {
                        OutputStream.Position = extent.Start + extentOffset + copyBufferOffset;
                        OutputStream.Write(copyBuffer, copyBufferOffset, numRead - copyBufferOffset);
                        BytesWritten += numRead - copyBufferOffset;
                    }

                    extentOffset += numRead;

                    RaiseProgressEvent();
                }
            }
            if (OutputStream.Length < inStream.Length)
            {
                inStream.Position = inStream.Length - 1;
                int b = inStream.ReadByte();
                if (b >= 0)
                {
                    OutputStream.Position = inStream.Length - 1;
                    OutputStream.WriteByte((byte)b);
                }
            }
        }
        private void RaiseProgressEvent()
        {
            // Raise the event by using the () operator.
            if (ProgressEvent != null)
            {
                PumpProgressEventArgs args = new PumpProgressEventArgs();
                args.BytesRead = BytesRead;
                args.BytesWritten = BytesWritten;
                args.SourcePosition = InputStream.Position;
                args.DestinationPosition = OutputStream.Position;
                ProgressEvent(this, args);
            }
        }
    }
}