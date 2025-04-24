namespace Haukcode.ArtNet.Internal;

public abstract class ReceiveDataBase
{
    public double TimestampMS { get; set; }

    public IPEndPoint Source { get; set; } = null!;
    
    public IPEndPoint? Destination { get; set; }
}
