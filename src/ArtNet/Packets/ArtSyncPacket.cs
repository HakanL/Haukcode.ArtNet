using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtSyncPacket : ArtNetPacket
{
    public ArtSyncPacket()
        : base(ArtNetOpCodes.Sync)
    {
    }

    public short Aux { get; set; }

    protected override int DataLength => 2;

    internal static ArtSyncPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtSyncPacket
        {
            Aux = reader.ReadInt16Reverse()
        };

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteInt16(Aux);
    }
}
