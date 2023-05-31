namespace WindowsToolKit.Streams
{
    public interface IByteArraySerializable
    {
        int Size { get; }
        int ReadFrom(byte[] buffer, int offset);
        void WriteTo(byte[] buffer, int offset);
    }
}