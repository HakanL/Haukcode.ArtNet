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

    public ArtTodControlPacket(ArtNetReceiveData data)
        : base(data)
    {
    }

    #region Packet Properties

    public byte Net { get; set; }

    public ArtTodControlCommand Command { get; set; }

    public byte Address { get; set; }

    protected override int DataLength => 12;


    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        data.BaseStream.Seek(9, System.IO.SeekOrigin.Current);
        Net = data.ReadByte();
        Command = (ArtTodControlCommand)data.ReadByte();
        Address = data.ReadByte();
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteZeros(9);
        writer.WriteByte(Net);
        writer.WriteByte((byte)Command);
        writer.WriteByte(Address);
    }


}
