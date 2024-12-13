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

    public byte RdmVersion { get; set; }

    public UId DeviceId { get; set; }

    public RdmCommands Command { get; set; }

    public RdmParameters ParameterId { get; set; }

    public short SubDevice { get; set; }

    public short SubCount { get; set; }

    public byte[] RdmData { get; set; } = null!;

    protected override int DataLength => 20 + RdmData.Length;

    internal static ArtRdmSubPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtRdmSubPacket
        {
            RdmVersion = reader.ReadByte()
        };

        reader.SkipBytes(1);
        target.DeviceId = ReadUId(reader);
        reader.SkipBytes(1);
        target.Command = (RdmCommands)reader.ReadByte();
        target.ParameterId = (RdmParameters)reader.ReadInt16();
        target.SubDevice = reader.ReadInt16();
        target.SubCount = reader.ReadInt16();
        reader.SkipBytes(4);

        target.RdmData = reader.ReadBytes();

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
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
