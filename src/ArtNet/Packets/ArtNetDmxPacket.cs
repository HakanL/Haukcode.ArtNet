using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtNetDmxPacket : ArtNetPacket
{
    public ArtNetDmxPacket()
        : base(ArtNetOpCodes.Output)
    {
    }

    public byte Sequence { get; set; }

    public byte Physical { get; set; }

    public short Universe { get; set; }

    public byte[] DmxData { get; set; } = null!;

    protected override int DataLength => 6 + DmxData.Length;

    internal static ArtNetDmxPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtNetDmxPacket
        {
            Sequence = reader.ReadByte(),
            Physical = reader.ReadByte(),
            Universe = reader.ReadInt16Reverse()
        };

        int length = reader.ReadInt16();
        target.DmxData = reader.ReadBytes(length);

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte(Sequence);
        writer.WriteByte(Physical);
        writer.WriteInt16Reverse(Universe);
        writer.WriteInt16((short)DmxData.Length);
        writer.WriteBytes(DmxData);
    }
}
