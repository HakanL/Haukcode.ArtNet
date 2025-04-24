namespace Haukcode.ArtNet.Rdm;

public class InvalidPacketException : InvalidOperationException
{
    public InvalidPacketException(string message)
        : base(message)
    {
    }
}
