using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtTodRequestPacket : ArtNetPacket
{
    public ArtTodRequestPacket()
        : base(ArtNetOpCodes.TodRequest)
    {
        RequestedUniverses = new List<byte>();
    }

    public ArtTodRequestPacket(ArtNetReceiveData data)
        : base(data)
    {
    }

    #region Packet Properties

    public byte Net { get; set; }

    public byte Command { get; set; }

    public List<byte> RequestedUniverses { get; set; }

    protected override int DataLength => 12 + RequestedUniverses.Count;


    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        data.BaseStream.Seek(9, System.IO.SeekOrigin.Current);
        Net = data.ReadByte();
        Command = data.ReadByte();
        int count = data.ReadByte();
        RequestedUniverses = new List<byte>(data.ReadBytes(count));
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteZeros(9);
        writer.WriteByte(Net);
        writer.WriteByte(Command);
        writer.WriteByte((byte)RequestedUniverses.Count);
        writer.WriteBytes(RequestedUniverses.ToArray());
    }


}
