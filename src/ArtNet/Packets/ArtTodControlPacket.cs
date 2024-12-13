using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public enum ArtTodControlCommand
{
    AtcNone = 0,
    AtcFlush = 1
}

public class ArtTodControlPacket : ArtNetPacket
{
    public ArtTodControlPacket()
        : base(ArtNetOpCodes.TodControl)
    {
    }

    public byte Net { get; set; }

    public ArtTodControlCommand Command { get; set; }

    public byte Address { get; set; }

    protected override int DataLength => 12;

    internal static ArtTodControlPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(9);

        var target = new ArtTodControlPacket
        {
            Net = reader.ReadByte(),
            Command = (ArtTodControlCommand)reader.ReadByte(),
            Address = reader.ReadByte()
        };

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteZeros(9);
        writer.WriteByte(Net);
        writer.WriteByte((byte)Command);
        writer.WriteByte(Address);
    }
}
