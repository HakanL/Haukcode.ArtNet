using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtPollPacket : ArtNetPacket
{
    public ArtPollPacket()
        : base(ArtNetOpCodes.Poll)
    {
    }

    public ArtPollPacket(ArtNetReceiveData data)
        : base(data)
    {

    }

    #region Packet Properties

    private byte talkToMe = 0;

    public byte TalkToMe
    {
        get { return talkToMe; }
        set { talkToMe = value; }
    }

    protected override int DataLength => 2;

    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        TalkToMe = data.ReadByte();
        data.ReadByte();
    }

    public override void WriteData(BigEndianBinaryWriter writer)
    {
        base.WriteData(writer);

        writer.WriteByte(TalkToMe);
        writer.WriteByte((byte)0);
    }

}
