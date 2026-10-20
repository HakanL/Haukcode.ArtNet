using System.IO;

using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Management;
using Haukcode.ArtNet.Rdm.Packets.Product;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class RdmResponsePacketTests
{
    private static byte[] Encode(RdmPacket packet)
    {
        using var stream = new MemoryStream();
        var writer = new RdmBinaryWriter(stream);
        RdmPacket.WritePacket(packet, writer);
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
        var reply = new DeviceLabel.GetReply { Label = "lamp" };

        var parsed = (DeviceLabel.GetReply)Decode(Encode(reply));

        Assert.AreEqual(RdmResponseTypes.Ack, parsed.ResponseType);
        Assert.AreEqual("lamp", parsed.Label);
    }

    [TestMethod]
    public void TypedReply_ResponseTypeIsReadFromHeader()
    {
        var reply = new DeviceLabel.GetReply { Label = "lamp" };
        reply.ResponseType = RdmResponseTypes.AckOverflow;

        var wire = Encode(reply);
        Assert.AreEqual((byte)RdmResponseTypes.AckOverflow, wire[14]);

        var parsed = (DeviceLabel.GetReply)Decode(wire);
        Assert.AreEqual(RdmResponseTypes.AckOverflow, parsed.ResponseType);
    }

    [TestMethod]
    public void AckTimer_ReportsAckTimerAndRoundTripsDelay()
    {
        var timer = new RdmAckTimer(RdmCommands.GetResponse, RdmParameters.DeviceLabel)
        {
            EstimatedResponseTime = 60000
        };

        var parsed = Decode(Encode(timer));

        Assert.IsInstanceOfType<RdmAckTimer>(parsed);
        var ackTimer = (RdmAckTimer)parsed;
        Assert.AreEqual(RdmResponseTypes.AckTimer, ackTimer.ResponseType);
        Assert.AreEqual((ushort)60000, ackTimer.EstimatedResponseTime);
    }

    [TestMethod]
    public void Nack_ReportsNackReasonAndRoundTripsReason()
    {
        var nack = new RdmNack(RdmCommands.GetResponse, RdmParameters.DeviceLabel)
        {
            Reason = NackReason.WriteProtect
        };

        var parsed = Decode(Encode(nack));

        Assert.IsInstanceOfType<RdmNack>(parsed);
        var parsedNack = (RdmNack)parsed;
        Assert.AreEqual(RdmResponseTypes.NackReason, parsedNack.ResponseType);
        Assert.AreEqual(NackReason.WriteProtect, parsedNack.Reason);
    }

    [TestMethod]
    public void ReadPacket_WithPreparedHeader_PreservesResponseType()
    {
        var header = new RdmHeader
        {
            Command = RdmCommands.GetResponse,
            ParameterId = RdmParameters.DeviceLabel,
            PortOrResponseType = (byte)RdmResponseTypes.AckTimer
        };

        using var content = new MemoryStream([0, 60]);
        var parsed = RdmPacket.ReadPacket(header, new RdmBinaryReader(content));

        Assert.IsInstanceOfType<RdmAckTimer>(parsed);
        Assert.AreEqual(RdmResponseTypes.AckTimer, ((RdmAckTimer)parsed).ResponseType);
    }
}
