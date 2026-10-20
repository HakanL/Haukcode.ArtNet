using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Management;
using Haukcode.ArtNet.Rdm.Packets.Product;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Response type and Port ID are views of the header byte (based on the tests in PR #16).
/// </summary>
[TestClass]
public class RdmResponsePacketTests
{
    // MessageLength (1) + DestinationId (6) + SourceId (6) + TransactionNumber (1)
    private const int PortOrResponseTypeOffset = 14;

    // ... + PortOrResponseType, MessageCount, SubDevice (2), Command, ParameterId (2)
    private const int ParameterDataLengthOffset = 21;

    private static byte[] Encode(RdmPacket packet)
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));
        return stream.ToArray();
    }

    private static RdmPacket Decode(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return RdmPacket.ReadPacket(new RdmBinaryReader(stream));
    }

    [TestMethod]
    public void TypedReply_DefaultsToAck()
    {
        var parsed = (DeviceLabel.GetReply)Decode(Encode(new DeviceLabel.GetReply { Label = "lamp" }));

        Assert.AreEqual(RdmResponseTypes.Ack, parsed.ResponseType);
        Assert.AreEqual("lamp", parsed.Label);
    }

    [TestMethod]
    public void TypedReply_AckOverflow_IsWrittenAndReadFromTheHeader()
    {
        var reply = new DeviceLabel.GetReply { Label = "lamp", ResponseType = RdmResponseTypes.AckOverflow };

        var wire = Encode(reply);
        Assert.AreEqual((byte)RdmResponseTypes.AckOverflow, wire[PortOrResponseTypeOffset]);

        var parsed = (DeviceLabel.GetReply)Decode(wire);
        Assert.AreEqual(RdmResponseTypes.AckOverflow, parsed.ResponseType);
    }

    [TestMethod]
    public void Request_PortId_IsWrittenAndReadFromTheHeader()
    {
        var wire = Encode(new DeviceLabel.Get { PortId = 3 });
        Assert.AreEqual((byte)3, wire[PortOrResponseTypeOffset]);

        var parsed = (DeviceLabel.Get)Decode(wire);
        Assert.AreEqual((byte)3, parsed.PortId);
    }

    [TestMethod]
    public void AckTimer_ReportsAckTimerAndRoundTripsDelay()
    {
        var timer = new RdmAckTimer(RdmCommands.GetResponse, RdmParameters.DeviceLabel) { EstimatedResponseTime = 60000 };

        var parsed = Assert.IsInstanceOfType<RdmAckTimer>(Decode(Encode(timer)));

        Assert.AreEqual(RdmResponseTypes.AckTimer, parsed.ResponseType);
        Assert.AreEqual((ushort)60000, parsed.EstimatedResponseTime);
    }

    [TestMethod]
    public void Nack_ReportsNackReasonAndRoundTripsReason()
    {
        var nack = new RdmNack(RdmCommands.GetResponse, RdmParameters.DeviceLabel) { Reason = NackReason.WriteProtect };

        var wire = Encode(nack);
        Assert.AreEqual((byte)2, wire[ParameterDataLengthOffset]);

        var parsed = Assert.IsInstanceOfType<RdmNack>(Decode(wire));
        Assert.AreEqual(RdmResponseTypes.NackReason, parsed.ResponseType);
        Assert.AreEqual(NackReason.WriteProtect, parsed.Reason);
    }

    [TestMethod]
    public void ReadPacket_WithPreparedHeader_PreservesResponseType()
    {
        var header = new RdmHeader
        {
            Command = RdmCommands.GetResponse,
            ParameterId = RdmParameters.DeviceLabel,
            PortOrResponseType = (byte)RdmResponseTypes.AckTimer,
            ParameterDataLength = 2,
        };

        using var content = new MemoryStream([0, 60]);
        var parsed = Assert.IsInstanceOfType<RdmAckTimer>(RdmPacket.ReadPacket(header, new RdmBinaryReader(content)));

        Assert.AreEqual(RdmResponseTypes.AckTimer, parsed.ResponseType);
        Assert.AreEqual((ushort)60, parsed.EstimatedResponseTime);
    }

    [TestMethod]
    public void ReadPacket_WithPreparedHeader_ReadsVariableLengthBody()
    {
        var header = new RdmHeader
        {
            Command = RdmCommands.GetResponse,
            ParameterId = RdmParameters.DeviceLabel,
            PortOrResponseType = (byte)RdmResponseTypes.Ack,
            ParameterDataLength = 4,
        };

        using var content = new MemoryStream("lamp"u8.ToArray());
        var parsed = Assert.IsInstanceOfType<DeviceLabel.GetReply>(RdmPacket.ReadPacket(header, new RdmBinaryReader(content)));

        Assert.AreEqual("lamp", parsed.Label);
    }
}
