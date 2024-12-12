using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtRdmSubPacket : ArtNetPacket
{
    public ArtRdmSubPacket()
        : base(ArtNetOpCodes.RdmSub)
    {
        RdmVersion = 1;
    }

    public ArtRdmSubPacket(ArtNetReceiveData data)
        : base(data)
    {
    }

    #region Packet Properties

    public byte RdmVersion { get; set; }

    public UId DeviceId { get; set; }

    public RdmCommands Command { get; set; }

    public RdmParameters ParameterId { get; set; }

    public short SubDevice { get; set; }

    public short SubCount { get; set; }

    public byte[] RdmData { get; set; } = null!;

    protected override int DataLength => 20 + RdmData.Length;


    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        var reader = new RdmBinaryReader(data.BaseStream);
        
        base.ReadData(data);            

        RdmVersion = data.ReadByte();
        data.BaseStream.Seek(1, System.IO.SeekOrigin.Current);
        DeviceId = reader.ReadUId();
        data.BaseStream.Seek(1, System.IO.SeekOrigin.Current);
        Command = (RdmCommands) data.ReadByte();
        ParameterId = (RdmParameters) reader.ReadHiLoInt16();
        SubDevice = reader.ReadHiLoInt16();
        SubCount = reader.ReadHiLoInt16();
        data.BaseStream.Seek(4, System.IO.SeekOrigin.Current);
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteByte(RdmVersion);
        writer.WriteByte(0x00);
        WriteUid(writer, DeviceId);
        writer.WriteByte(0x00);
        writer.WriteByte((byte)Command);
        writer.WriteInt16((short)ParameterId);
        writer.WriteInt16(SubDevice);
        writer.WriteInt16(SubCount);
        writer.WriteZeros(4);
        writer.WriteBytes(RdmData);
    }


}
