using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet;

public class ReceiveDataPacket : ReceiveDataBase
{
    public ArtNetPacket Packet { get; set; } = null!;
}
