using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// ArtDataRequest/Reply, ArtDiagData, ArtCommand, ArtNzs, ArtVlc and ArtFirmwareMaster/Reply (#30),
/// with sizes from the Art-Net 4 packet definitions.
/// </summary>
[TestClass]
public class ArtNetMorePacketsTests
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
    public void DataRequestAndReply_RoundTrip()
    {
        var request = RoundTrip(new ArtDataRequestPacket { EstaManufacturer = 0x6A6B, Oem = 0x2BD3, Request = ArtDataRequestCodes.UrlProduct }, 12 + 28);
        Assert.AreEqual(ArtDataRequestCodes.UrlProduct, request.Request);
        Assert.AreEqual((ushort)0x6A6B, request.EstaManufacturer);

        var reply = RoundTrip(new ArtDataReplyPacket { Request = ArtDataRequestCodes.UrlProduct, PayloadText = "https://dmxking.com" }, 12 + 8 + 20);
        Assert.AreEqual("https://dmxking.com", reply.PayloadText);
        Assert.AreEqual((byte)0, reply.Payload[^1], "string payloads are NUL-terminated");
    }

    [TestMethod]
    public void DiagData_RoundTrips()
    {
        var parsed = RoundTrip(new ArtDiagDataPacket { Priority = ArtDiagPriorities.High, LogicalPort = 2, Text = "DMX input lost" }, 12 + 6 + 15);

        Assert.AreEqual(ArtDiagPriorities.High, parsed.Priority);
        Assert.AreEqual((byte)2, parsed.LogicalPort);
        Assert.AreEqual("DMX input lost", parsed.Text);
    }

    [TestMethod]
    public void Command_RoundTripsAndSplitsCommands()
    {
        var parsed = RoundTrip(new ArtCommandPacket { EstaManufacturer = 0xFFFF, Text = "SwoutText=Playback&SwinText=Record&" }, 12 + 4 + 36);

        Assert.AreEqual("SwoutText=Playback&SwinText=Record&", parsed.Text);
        CollectionAssert.AreEqual(
            new[] { new KeyValuePair<string, string>("SwoutText", "Playback"), new KeyValuePair<string, string>("SwinText", "Record") },
            parsed.Commands.ToArray());
    }

    [TestMethod]
    public void Nzs_RoundTrips_AndRejectsZeroAndRdmStartCodes()
    {
        var parsed = RoundTrip(new ArtNzsPacket { Sequence = 5, StartCode = 0x17, Universe = 0x0131, Data = new byte[] { 1, 2, 3 } }, 12 + 6 + 3);
        Assert.AreEqual((byte)0x17, parsed.StartCode);
        Assert.AreEqual((ushort)0x0131, parsed.Universe);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, parsed.Data.ToArray());

        Assert.ThrowsExactly<InvalidOperationException>(() => Write(new ArtNzsPacket { StartCode = 0, Data = new byte[1] }));
        Assert.ThrowsExactly<InvalidOperationException>(() => Write(new ArtNzsPacket { StartCode = 0xCC, Data = new byte[1] }));
    }

    [TestMethod]
    public void Vlc_RoundTripsAsArtVlcWithTheHeaderInPlace()
    {
        var vlc = new ArtVlcPacket
        {
            Universe = 49,
            Flags = ArtVlcFlags.Beacon,
            Transaction = 0xFFFF,
            SlotAddress = 7,
            Depth = 50,
            Frequency = 1000,
            PayloadLanguage = ArtVlcPayloadLanguages.BeaconText,
            BeaconRepeat = 10,
            Payload = "hello"u8.ToArray(),
        };

        var wire = Write(vlc);
        Assert.AreEqual(12 + 6 + 22 + 5, wire.Length);
        Assert.AreEqual(ArtVlcPacket.VlcStartCode, wire[13]);

        // VLC data starts at 18: magic, flags, then PayCount at 26-27 and PayCheck at 28-29
        CollectionAssert.AreEqual(new byte[] { 0x41, 0x4C, 0x45 }, wire[18..21]);
        Assert.AreEqual(5, (wire[26] << 8) | wire[27]);
        Assert.AreEqual(ArtVlcPacket.PayloadChecksum("hello"u8), (ushort)((wire[28] << 8) | wire[29]));

        var parsed = Assert.IsInstanceOfType<ArtVlcPacket>(ArtNetPacket.Parse(wire));
        Assert.AreEqual(ArtVlcFlags.Beacon, parsed.Flags);
        Assert.AreEqual((ushort)0xFFFF, parsed.Transaction);
        Assert.AreEqual((ushort)7, parsed.SlotAddress);
        Assert.AreEqual((byte)50, parsed.Depth);
        Assert.AreEqual((ushort)1000, parsed.Frequency);
        Assert.AreEqual(ArtVlcPayloadLanguages.BeaconText, parsed.PayloadLanguage);
        Assert.AreEqual((ushort)10, parsed.BeaconRepeat);
        CollectionAssert.AreEqual("hello"u8.ToArray(), parsed.Payload);
    }

    [TestMethod]
    public void Firmware_RoundTrips()
    {
        var master = RoundTrip(new ArtFirmwareMasterPacket { Type = ArtFirmwareTypes.FirmFirst, BlockId = 0, FirmwareLength = 0x00008212, Data = [0xAB, 0xCD] }, 12 + 28 + 1024);
        Assert.AreEqual(0x00008212u, master.FirmwareLength);
        Assert.AreEqual(ArtFirmwareMasterPacket.BlockSize, master.Data.Length);
        Assert.AreEqual((byte)0xCD, master.Data[1]);

        Assert.AreEqual(ArtFirmwareReplyTypes.AllGood, RoundTrip(new ArtFirmwareReplyPacket { Type = ArtFirmwareReplyTypes.AllGood }, 12 + 24).Type);
    }
}
