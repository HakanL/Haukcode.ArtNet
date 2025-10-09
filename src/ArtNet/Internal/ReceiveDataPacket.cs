using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Internal;

public class ReceiveDataPacket
{
    public double TimestampMS { get; set; }

    public IPEndPoint Source { get; set; } = null!;

    public IPEndPoint? Destination { get; set; }

    public ArtNetPacket Packet { get; set; } = null!;
}
