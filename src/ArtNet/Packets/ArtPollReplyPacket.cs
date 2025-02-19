using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;
using Haukcode.Network;

namespace Haukcode.ArtNet.Packets;

public class ArtPollReplyPacket : ArtNetPacket
{
    public ArtPollReplyPacket()
        : base(ArtNetOpCodes.PollReply)
    {
    }

    private byte[] ipAddress = new byte[4];

    public byte[] IpAddress
    {
        get { return ipAddress; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The IP address must be an array of 4 bytes.");

            ipAddress = value;
        }
    }

    private byte[] respUID = new byte[6];

    public byte[] RespUID
    {
        get { return respUID; }
        set
        {
            if (value.Length != 6)
                throw new ArgumentException("The Responder Default UID must be an array of 6 bytes.");

            respUID = value;
        }
    }

    private short port = ArtNetClient.DefaultPort;

    public short Port
    {
        get { return port; }
        set { port = value; }
    }

    private short firmwareVersion = 0;

    public short FirmwareVersion
    {
        get { return firmwareVersion; }
        set { firmwareVersion = value; }
    }

    private short subSwitch = 0;

    public short SubSwitch
    {
        get { return subSwitch; }
        set { subSwitch = value; }
    }

    private short oem = 0xff;

    public short Oem
    {
        get { return oem; }
        set { oem = value; }
    }

    private byte ubeaVersion = 0;

    public byte UbeaVersion
    {
        get { return ubeaVersion; }
        set { ubeaVersion = value; }
    }

    private byte status = 0;

    public byte Status
    {
        get { return status; }
        set { status = value; }
    }

    private short estaCode = 0;

    public short EstaCode
    {
        get { return estaCode; }
        set { estaCode = value; }
    }

    private string shortName = string.Empty;

    public string ShortName
    {
        get { return shortName; }
        set { shortName = value; }
    }

    private string longName = string.Empty;

    public string LongName
    {
        get { return longName; }
        set { longName = value; }
    }

    private string nodeReport = string.Empty;

    public string NodeReport
    {
        get { return nodeReport; }
        set { nodeReport = value; }
    }

    private short portCount = 0;

    public short PortCount
    {
        get { return portCount; }
        set { portCount = value; }
    }

    private byte[] portTypes = new byte[4];

    public byte[] PortTypes
    {
        get { return portTypes; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The port types must be an array of 4 bytes.");

            portTypes = value;
        }
    }

    private byte[] goodInput = new byte[4];

    public byte[] GoodInput
    {
        get { return goodInput; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good input must be an array of 4 bytes.");

            goodInput = value;
        }
    }

    private byte[] _goodOutputA = new byte[4];

    public byte[] GoodOutputA
    {
        get { return _goodOutputA; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good output must be an array of 4 bytes.");

            _goodOutputA = value;
        }
    }

    private byte[] _goodOutputB = new byte[4];

    public byte[] GoodOutputB
    {
        get { return _goodOutputB; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good output must be an array of 4 bytes.");

            _goodOutputB = value;
        }
    }

    private byte[] swIn = new byte[4];

    public byte[] SwIn
    {
        get { return swIn; }
        set { swIn = value; }
    }

    private byte[] swOut = new byte[4];

    public byte[] SwOut
    {
        get { return swOut; }
        set { swOut = value; }
    }

    private byte _acnPriority = 0;

    public byte AcnPriority
    {
        get { return _acnPriority; }
        set { _acnPriority = value; }
    }

    private byte swMacro = 0;

    public byte SwMacro
    {
        get { return swMacro; }
        set { swMacro = value; }
    }

    private byte swRemote = 0;

    public byte SwRemote
    {
        get { return swRemote; }
        set { swRemote = value; }
    }

    private byte style = 0;

    public byte Style
    {
        get { return style; }
        set { style = value; }
    }

    private byte[] macAddress = new byte[6];

    public byte[] MacAddress
    {
        get { return macAddress; }
        set
        {
            if (value.Length != 6)
                throw new ArgumentException("The mac address must be an array of 6 bytes.");

            macAddress = value;
        }
    }

    private byte[] bindIpAddress = new byte[4];

    public byte[] BindIpAddress
    {
        get { return bindIpAddress; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The bind IP address must be an array of 4 bytes.");

            bindIpAddress = value;
        }
    }

    private byte bindIndex = 0;

    public byte BindIndex
    {
        get { return bindIndex; }
        set { bindIndex = value; }
    }

    private byte status2 = 0;

    public byte Status2
    {
        get { return status2; }
        set { status2 = value; }
    }

    private byte status3 = 0;

    public byte Status3
    {
        get { return status3; }
        set { status3 = value; }
    }

    protected override int DataLength => 229;

    /// <summary>
    /// Interprets the universe address to ensure compatibility with ArtNet I, II and III devices.
    /// </summary>
    /// <param name="outPorts">Whether to get the address for in or out ports.</param>
    /// <param name="portIndex">The port index to obtain the universe for.</param>
    /// <returns>The 15 Bit universe address</returns>
    public int UniverseAddress(bool outPorts, int portIndex)
    {
        int universe;

        if (SubSwitch > 0)
        {
            universe = (SubSwitch & 0x7F00);
            universe += (SubSwitch & 0x0F) << 4;
            universe += (outPorts ? SwOut[portIndex] : SwIn[portIndex]) & 0xF;
        }
        else
        {
            universe = (outPorts ? SwOut[portIndex] : SwIn[portIndex]);
        }

        return universe;
    }

    internal static ArtPollReplyPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtPollReplyPacket
        {
            IpAddress = reader.ReadBytes(4),
            Port = reader.ReadInt16Reverse(),
            FirmwareVersion = reader.ReadInt16(),
            SubSwitch = reader.ReadInt16(),
            Oem = reader.ReadInt16(),
            UbeaVersion = reader.ReadByte(),
            Status = reader.ReadByte(),
            EstaCode = reader.ReadInt16Reverse(),
            ShortName = reader.ReadString(18),
            LongName = reader.ReadString(64),
            NodeReport = reader.ReadString(64),
            PortCount = reader.ReadInt16(),
            PortTypes = reader.ReadBytes(4),
            GoodInput = reader.ReadBytes(4),
            GoodOutputA = reader.ReadBytes(4),
            SwIn = reader.ReadBytes(4),
            SwOut = reader.ReadBytes(4),
            AcnPriority = reader.ReadByte(),
            SwMacro = reader.ReadByte(),
            SwRemote = reader.ReadByte()
        };

        reader.SkipBytes(3);
        target.Style = reader.ReadByte();
        target.MacAddress = reader.ReadBytes(6);
        target.BindIpAddress = reader.ReadBytes(4);
        target.BindIndex = reader.ReadByte();
        target.Status2 = reader.ReadByte();
        target.GoodOutputB = reader.ReadBytes(4);
        target.Status3 = reader.ReadByte();
        target.RespUID = reader.ReadBytes(6);

        reader.SkipBytes(15);

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteBytes(IpAddress);
        writer.WriteInt16Reverse(Port);
        writer.WriteInt16(FirmwareVersion);
        writer.WriteInt16(SubSwitch);
        writer.WriteInt16(Oem);
        writer.WriteByte(UbeaVersion);
        writer.WriteByte((byte)Status);
        writer.WriteInt16Reverse(EstaCode);
        writer.WriteString(ShortName, 18);
        writer.WriteString(LongName, 64);
        writer.WriteString(NodeReport, 64);
        writer.WriteInt16(PortCount);
        writer.WriteBytes(PortTypes);
        writer.WriteBytes(GoodInput);
        writer.WriteBytes(GoodOutputA);
        writer.WriteBytes(SwIn);
        writer.WriteBytes(SwOut);
        writer.WriteByte(AcnPriority);
        writer.WriteByte(SwMacro);
        writer.WriteByte(SwRemote);
        writer.WriteZeros(3);
        writer.WriteByte(Style);
        writer.WriteBytes(MacAddress);
        writer.WriteBytes(BindIpAddress);
        writer.WriteByte(BindIndex);
        writer.WriteByte(Status2);
        writer.WriteBytes(GoodOutputB);
        writer.WriteByte(Status3);
        writer.WriteBytes(respUID);
        writer.WriteZeros(15);
    }
}
