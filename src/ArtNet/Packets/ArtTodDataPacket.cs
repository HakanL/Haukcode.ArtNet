using Haukcode.ArtNet.IO;
using Haukcode.Network;
using Haukcode.Rdm;
using System.Collections.Generic;

namespace Haukcode.ArtNet.Packets;

public class ArtTodDataPacket : ArtNetPacket
{
    public ArtTodDataPacket()
        : base(ArtNetOpCodes.TodData)
    {
        RdmVersion = 1;
        Devices = new List<UId>();
    }

    public ArtTodDataPacket(ArtNetReceiveData data)
        : base(data)
    {

    }

    #region Packet Properties

    public byte RdmVersion { get; set; }

    public byte Port { get; set; }

    public byte BindIndex { get; set; }

    public byte Net { get; set; }

    public byte CommandResponse { get; set; } // 0x00 TodFull 0xFF TodNack

    public byte Universe { get; set; }

    public short UIdTotal { get; set; }

    public byte BlockCount { get; set; }

    public List<UId> Devices { get; set; }

    protected override int DataLength => 16 + Devices.Count * 6;


    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        var rdmReader = new RdmBinaryReader(data.BaseStream);

        base.ReadData(data);

        RdmVersion = data.ReadByte();
        Port = data.ReadByte();
        data.BaseStream.Seek(6, System.IO.SeekOrigin.Current);
        BindIndex = data.ReadByte();
        Net = data.ReadByte();
        CommandResponse = data.ReadByte();
        Universe = data.ReadByte();
        UIdTotal = rdmReader.ReadHiLoInt16();
        BlockCount = data.ReadByte();

        Devices = new List<UId>();
        int count = data.ReadByte();
        for (int n = 0; n < count; n++)
            Devices.Add(rdmReader.ReadUId());
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteByte(RdmVersion);
        writer.WriteByte(Port);
        writer.WriteZeros(6);
        writer.WriteByte(BindIndex);
        writer.WriteByte(Net);
        writer.WriteByte(CommandResponse);
        writer.WriteByte(Universe);
        writer.WriteInt16(UIdTotal);
        writer.WriteByte(BlockCount);
        writer.WriteByte((byte)Devices.Count);

        foreach (UId id in Devices)
            WriteUid(writer, id);
    }
}
