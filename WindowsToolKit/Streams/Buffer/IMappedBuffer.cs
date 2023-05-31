

namespace WindowsToolKit.Streams
{
    public interface IMappedBuffer : IBuffer
    {
        long MapPosition(long position);
    }
}