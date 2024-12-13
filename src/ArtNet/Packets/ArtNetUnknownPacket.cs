using Haukcode.ArtNet.IO;
using Haukcode.Network;
using System;

namespace Haukcode.ArtNet.Packets;

public class ArtNetUnknownPacket : ArtNetPacket
{
    public ArtNetUnknownPacket(int opCode)
        : base((ArtNetOpCodes)opCode)
    {
    }

    protected override int DataLength => Data.Length;

    internal static ArtNetUnknownPacket Parse(short opCode, BigEndianBinaryReader reader)
    {
        var target = new ArtNetUnknownPacket(opCode)
        {
            Data = reader.ReadBytes()
        };

        return target;
    }

    public byte[] Data { get; set; } = null!;

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteBytes(Data);
    }
}
