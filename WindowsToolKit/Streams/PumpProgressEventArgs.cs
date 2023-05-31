using System;

namespace WindowsToolKit.Streams
{
    public class PumpProgressEventArgs : EventArgs
    {
        public long BytesRead { get; set; }
        public long BytesWritten { get; set; }
        public long DestinationPosition { get; set; }
        public long SourcePosition { get; set; }
    }
}