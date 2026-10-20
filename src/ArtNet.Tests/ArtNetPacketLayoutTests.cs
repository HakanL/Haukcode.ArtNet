using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Packet sizes and field positions per the Art-Net 4 spec.
/// </summary>
[TestClass]
public class ArtNetPacketLayoutTests
{
    private static byte[] Write(ArtNetPacket packet)
    {
        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        return buffer[..packet.WriteToBuffer(buffer)];
    }

    private static T RoundTrip<T>(T packet, int expectedLength)
        where T : ArtNetPacket
    {
        var wire = Write(packet);
        Assert.AreEqual(expectedLength, wire.Length, $"{typeof(T).Name} length");

        return Assert.IsInstanceOfType<T>(ArtNetPacket.Parse(wire));
    }

    [TestMethod]
    public void IpProgReply_Is34Bytes_WithGatewayAfterTheSpareByte()
    {
        var wire = Write(new ArtIpProgReplyPacket { DefaultGateway = [10, 0, 0, 1], DhcpEnabled = true });

        Assert.AreEqual(34, wire.Length);
        // Header (12), filler (4), IP (4), mask (4), port (2), status at 26, spare at 27, gateway at 28
        Assert.AreEqual((byte)(1 << 6), wire[26]);
        CollectionAssert.AreEqual(new byte[] { 10, 0, 0, 1 }, wire[28..32]);

        var parsed = (ArtIpProgReplyPacket)ArtNetPacket.Parse(wire)!;
        CollectionAssert.AreEqual(new byte[] { 10, 0, 0, 1 }, parsed.DefaultGateway);
        Assert.IsTrue(parsed.DhcpEnabled);
    }

    [TestMethod]
    public void IpProg_Is34Bytes()
    {
        var parsed = RoundTrip(new ArtIpProgPacket { DefaultGateway = [192, 168, 1, 1] }, 34);

        CollectionAssert.AreEqual(new byte[] { 192, 168, 1, 1 }, parsed.DefaultGateway);
    }

    [TestMethod]
    public void ArtAddressCopy_KeepsTheNet()
    {
        var reply = new ArtPollReplyPacket { NetSwitch = 3, SubSwitch = 2 };

        var address = new ArtAddressPacket(reply);

        Assert.AreEqual((byte)(3 | 0x80), address.NetSwitch);
        Assert.AreEqual((byte)(2 | 0x80), address.SubSwitch);
    }

    [TestMethod]
    public void PollReply_UniverseAddress_IsNetSubNetUniverse()
    {
        var reply = new ArtPollReplyPacket { NetSwitch = 0x7F, SubSwitch = 0x0F, SwOut = [0, 0, 0, 0x0F] };

        Assert.AreEqual(0x7FFF, reply.UniverseAddress(outPorts: true, 3));
        Assert.ThrowsExactly<ArgumentException>(() => reply.SwIn = new byte[3]);
    }

    [TestMethod]
    public void UId_CompareTo_OrdersByManufacturerThenDevice()
    {
        var a = new UId(1, 5);
        var b = new UId(2, 3);

        Assert.IsLessThan(0, a.CompareTo(b));
        Assert.IsGreaterThan(0, b.CompareTo(a));
        Assert.AreEqual(0, a.CompareTo(new UId(1, 5)));
        Assert.IsLessThan(0, new UId(1, 0x7FFFFFFF).CompareTo(new UId(1, 0xFFFFFFFF)));
    }

    [TestMethod]
    public void Poll_KeepsDiagPriority()
    {
        Assert.AreEqual((byte)0x40, RoundTrip(new ArtPollPacket { TalkToMe = 2, DiagPriority = 0x40 }, 14).DiagPriority);
    }

    [TestMethod]
    public void TodRequest_HasAFixed32ByteAddressArray()
    {
        var parsed = RoundTrip(new ArtTodRequestPacket { Net = 1, RequestedUniverses = [4, 5] }, 56);

        CollectionAssert.AreEqual(new byte[] { 4, 5 }, parsed.RequestedUniverses);
        Assert.ThrowsExactly<InvalidOperationException>(() => Write(new ArtTodRequestPacket { RequestedUniverses = [.. new byte[33]] }));
    }

    [TestMethod]
    public void Dmx_PadsToAnEvenLength_AndRejectsMoreThan512()
    {
        var wire = Write(new ArtNetDmxPacket { Universe = 1, DmxData = new byte[] { 1, 2, 3 } });

        Assert.AreEqual(18 + 4, wire.Length);
        Assert.AreEqual((byte)4, wire[17]);

        Assert.ThrowsExactly<InvalidOperationException>(() => Write(new ArtNetDmxPacket { DmxData = new byte[513] }));
    }

    [TestMethod]
    public void Dmx_RejectsALengthLongerThanTheDatagram()
    {
        var wire = Write(new ArtNetDmxPacket { Universe = 1, DmxData = new byte[] { 1, 2 } });
        wire[16] = 0x80;

        Assert.ThrowsExactly<InvalidDataException>(() => ArtNetPacket.Parse(wire));
    }

    [TestMethod]
    public void Trigger_IsPaddedTo512_AndParsesAShortPacket()
    {
        var wire = Write(new ArtTriggerPacket { OemCode = 0xFFFF, Key = 1, Data = [1, 2, 3] });
        Assert.AreEqual(12 + 6 + 512, wire.Length);

        var shortPacket = (ArtTriggerPacket)ArtNetPacket.Parse(wire[..20])!;
        Assert.AreEqual((ushort)0xFFFF, shortPacket.OemCode);
        CollectionAssert.AreEqual(new byte[] { 1, 2 }, shortPacket.Data);
    }

    [TestMethod]
    public void TodData_RejectsMoreThan200Devices()
    {
        var packet = new ArtTodDataPacket { Devices = [.. Enumerable.Range(0, 201).Select(i => new UId(1, (uint)i))] };

        Assert.ThrowsExactly<InvalidOperationException>(() => Write(packet));
    }

    [TestMethod]
    public void Parse_IgnoresDatagramsThatAreNotArtNet()
    {
        var wire = Write(new ArtPollPacket());

        var notArtNet = (byte[])wire.Clone();
        notArtNet[0] = (byte)'X';

        Assert.IsNull(ArtNetPacket.Parse(notArtNet));
        Assert.IsNull(ArtNetPacket.Parse(wire[..9]));
    }

    [TestMethod]
    public void PollReply_ShortNameIsNullTerminated_AndNotCutMidCharacter()
    {
        var wire = Write(new ArtPollReplyPacket { ShortName = new string('x', 16) + "é" });

        // ShortName is 18 bytes at offset 26: 16 x's, then "é" (2 bytes) doesn't fit before the NUL
        var field = wire[26..44];
        Assert.AreEqual((byte)0, field[^1]);
        Assert.AreEqual((byte)0, field[16]);

        var parsed = (ArtPollReplyPacket)ArtNetPacket.Parse(wire)!;
        Assert.AreEqual(new string('x', 16), parsed.ShortName);
    }

    [TestMethod]
    public void ArtAddressCopy_EncodesOnlyTheLowNibbleOfSwInSwOut()
    {
        var address = new ArtAddressPacket(new ArtPollReplyPacket { SwIn = [0xF1, 0, 0, 0], SwOut = [0xA2, 0, 0, 0] });

        Assert.AreEqual((byte)0x81, address.SwIn[0]);
        Assert.AreEqual((byte)0x82, address.SwOut[0]);
    }

    [TestMethod]
    public void ArtRdm_RoundTripsFifoAvailAndFifoMax()
    {
        var parsed = RoundTrip(new ArtRdmPacket { FifoAvail = 3, FifoMax = 8, Net = 1, Address = 0x31, SubStartCode = 1, RdmData = [1, 2] }, 12 + 13 + 2);

        Assert.AreEqual((byte)3, parsed.FifoAvail);
        Assert.AreEqual((byte)8, parsed.FifoMax);
        Assert.AreEqual((byte)1, parsed.Net);
        Assert.AreEqual((byte)0x31, parsed.Address);
    }

    [TestMethod]
    public void ArtRdmSub_RejectsSubCountZero()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => Write(new ArtRdmSubPacket { DeviceId = new UId(1, 1), SubCount = 0, RdmData = [] }));
    }
}
