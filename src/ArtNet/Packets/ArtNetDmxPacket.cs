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

    public ArtNetDmxPacket(ArtNetReceiveData data)
        : base(data)
    {
    }

    #region Packet Properties

    private byte sequence = 0;

    public byte Sequence
    {
        get { return sequence; }
        set { sequence = value; }
    }

    private byte physical = 0;

    public byte Physical
    {
        get { return physical; }
        set { physical = value; }
    }

    private short universe = 0;

    public short Universe
    {
        get { return universe; }
        set { universe = value; }
    }

    public short Length
    {
        get
        {
            if (dmxData == null)
                return 0;
            return (short)dmxData.Length;
        }
    }

    private byte[] dmxData = null!;

    public byte[] DmxData
    {
        get { return dmxData; }
        set { dmxData = value; }
    }

    protected override int DataLength => 6 + DmxData.Length;

    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        Sequence = data.ReadByte();
        Physical = data.ReadByte();
        Universe = data.ReadLoHiInt16();
        int length = data.ReadHiLoInt16();
        DmxData = data.ReadBytes(length);
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteByte(Sequence);
        writer.WriteByte(Physical);
        writer.WriteInt16Reverse(Universe);
        writer.WriteInt16(Length);
        writer.WriteBytes(DmxData);
    }
}
