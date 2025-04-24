using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Internal;

public class ReceiveDataPacket : ReceiveDataBase
{
    public ArtNetPacket Packet { get; set; } = null!;
}
