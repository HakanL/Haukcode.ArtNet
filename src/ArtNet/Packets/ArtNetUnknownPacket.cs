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

    public ArtNetUnknownPacket(ArtNetReceiveData data)
        : base(data)
    {
    }

    public byte[] Data { get; set; } = null!;

    protected override int DataLength => Data.Length;

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        Data = data.ReadBytes(DataLength);
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteBytes(Data);
    }
}
