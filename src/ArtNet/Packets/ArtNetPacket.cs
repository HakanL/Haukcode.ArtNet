using Haukcode.ArtNet.IO;
using Haukcode.Network;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;

namespace Haukcode.ArtNet.Packets;

public abstract class ArtNetPacket
{
    public const int MAX_PACKET_SIZE = 572;

    public ArtNetPacket(ArtNetOpCodes opCode)
    {
        OpCode = opCode;
    }

    public int PacketLength => (OpCode == ArtNetOpCodes.PollReply ? 10 : 12) + DataLength;

    protected abstract int DataLength { get; }

    public static ArtNetPacket Parse(ReadOnlyMemory<byte> inputBuffer)
    {
        var reader = new BigEndianBinaryReader(inputBuffer);

        string protocol = reader.ReadString(8);
        var opCode = (ArtNetOpCodes)reader.ReadInt16Reverse();

        short version = 14;

        // For some reason the poll packet header does not include the version
        if (opCode != ArtNetOpCodes.PollReply)
            version = reader.ReadInt16();

        var target = Create(opCode, reader);

        Debug.Assert(target.PacketLength == reader.BytesRead);

        return target;
    }

    public int WriteToBuffer(Memory<byte> buffer)
    {
        var writer = new Network.BigEndianBinaryWriter(buffer);

        writer.WriteString(Protocol, 8);
        writer.WriteUInt16Reverse((ushort)OpCode);

        // For some reason the poll reply packet header does not include the version
        if (OpCode != ArtNetOpCodes.PollReply)
            writer.WriteInt16(Version);

        WriteData(writer);

        Debug.Assert(writer.BytesWritten == PacketLength);

        return writer.BytesWritten;
    }

    protected static void WriteUid(BigEndianBinaryWriter writer, Rdm.UId value)
    {
        writer.WriteUInt16(value.ManufacturerId);
        writer.WriteUInt32(value.DeviceId);
    }

    protected static Rdm.UId ReadUId(BigEndianBinaryReader reader)
    {
        return new Rdm.UId(reader.ReadUInt16(), reader.ReadUInt32());
    }

    private string protocol = "Art-Net";

    public string Protocol
    {
        get { return protocol; }
        protected set
        {
            if (value.Length > 8)
                protocol = value[..8];
            else
                protocol = value;
        }
    }

    public short Version { get; protected set; } = 14;

    public virtual ArtNetOpCodes OpCode { get; protected set; } = ArtNetOpCodes.None;

    protected abstract void WriteData(BigEndianBinaryWriter writer);

    internal static ArtNetPacket Create(ArtNetOpCodes opCode, BigEndianBinaryReader reader)
    {
        switch (opCode)
        {
            case ArtNetOpCodes.Poll:
                return ArtPollPacket.Parse(reader);

            case ArtNetOpCodes.PollReply:
                return ArtPollReplyPacket.Parse(reader);

            case ArtNetOpCodes.Output:
                return ArtNetDmxPacket.Parse(reader);

            case ArtNetOpCodes.Sync:
                return ArtSyncPacket.Parse(reader);

            case ArtNetOpCodes.TodRequest:
                return ArtTodRequestPacket.Parse(reader);

            case ArtNetOpCodes.TodData:
                return ArtTodDataPacket.Parse(reader);

            case ArtNetOpCodes.TodControl:
                return ArtTodControlPacket.Parse(reader);

            case ArtNetOpCodes.Rdm:
                return ArtRdmPacket.Parse(reader);

            case ArtNetOpCodes.RdmSub:
                return ArtRdmSubPacket.Parse(reader);

            case ArtNetOpCodes.Trigger:
                return ArtTriggerPacket.Parse(reader);

            case ArtNetOpCodes.IpProg:
                return ArtIpProgPacket.Parse(reader);

            case ArtNetOpCodes.IpProgReply:
                return ArtIpProgReplyPacket.Parse(reader);

            case ArtNetOpCodes.Address:
                return ArtAddressPacket.Parse(reader);

            case ArtNetOpCodes.Input:
                return ArtInputPacket.Parse(reader);

            default:
                return ArtNetUnknownPacket.Parse((short)opCode, reader);
        }
    }
}
