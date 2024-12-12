using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtRdmPacket : ArtNetPacket
{
    public ArtRdmPacket()
        : base(ArtNetOpCodes.Rdm)
    {
        RdmVersion = 1;
        SubStartCode = 1;
    }

    public ArtRdmPacket(ArtNetReceiveData data)
        : base(data)
    {
        
    }

    #region Packet Properties

    public byte RdmVersion { get; set; }

    public byte Net { get; set; }

    public byte Command { get; set; }

    public byte Address { get; set; }

    public byte SubStartCode { get; set; }

    public byte[] RdmData { get; set; } = null!;

    protected override int DataLength => 13 + RdmData.Length;
    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        RdmVersion = data.ReadByte();
        data.BaseStream.Seek(8, System.IO.SeekOrigin.Current);
        Net = data.ReadByte();
        Command = data.ReadByte();
        Address = data.ReadByte();
        SubStartCode = data.ReadByte();
        RdmData = data.ReadBytes((int) (data.BaseStream.Length - data.BaseStream.Position));
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteByte(RdmVersion);
        writer.WriteZeros(8);
        writer.WriteByte(Net);
        writer.WriteByte(Command);
        writer.WriteByte(Address);
        writer.WriteByte(SubStartCode);
        writer.WriteBytes(RdmData);
    }
	

}
