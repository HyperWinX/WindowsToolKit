namespace WindowsToolKit.Streams
{
    public class Block
    {
        public int Available { get; set; }
        public byte[] Data { get; set; }
        public long Position { get; set; }
        public bool Equals(Block other)
        {
            return Position == other.Position;
        }
    }
}