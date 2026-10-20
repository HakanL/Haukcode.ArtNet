using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Fields that Art-Net 4 defines as unsigned must survive values with the top bit set.
/// </summary>
[TestClass]
public class ArtNetUnsignedFieldTests
{
    private static T RoundTrip<T>(T packet)
        where T : ArtNetPacket
    {
        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int length = packet.WriteToBuffer(buffer);

        return (T)ArtNetPacket.Parse(buffer.AsMemory(0, length))!;
    }

    [TestMethod]
    public void Trigger_AllOemCodes_RoundTrips()
    {
        var parsed = RoundTrip(new ArtTriggerPacket { OemCode = 0xFFFF, Key = 1, SubKey = 2, Data = new byte[512] });

        Assert.AreEqual((ushort)0xFFFF, parsed.OemCode);
    }

    [TestMethod]
    public void PollReply_HighValues_RoundTrip()
    {
        var parsed = RoundTrip(new ArtPollReplyPacket
        {
            FirmwareVersion = 0x8001,
            Oem = 0xFFFE,
            EstaCode = 0x8002,
        });

        Assert.AreEqual((ushort)0x8001, parsed.FirmwareVersion);
        Assert.AreEqual((ushort)0xFFFE, parsed.Oem);
        Assert.AreEqual((ushort)0x8002, parsed.EstaCode);
    }

    [TestMethod]
    public void RdmSub_ManufacturerPidAndAllSubDevices_RoundTrip()
    {
        var parsed = RoundTrip(new ArtRdmSubPacket
        {
            DeviceId = new UId(0x1234, 0x56789ABC),
            Command = 0x20,
            ParameterId = 0x8123,
            SubDevice = 0xFFFF,
            SubCount = 0x8000,
            RdmData = [],
        });

        Assert.AreEqual((ushort)0x8123, parsed.ParameterId);
        Assert.AreEqual((ushort)0xFFFF, parsed.SubDevice);
        Assert.AreEqual((ushort)0x8000, parsed.SubCount);
    }

    [TestMethod]
    public void Sync_Aux_ReadsBackInTheSameByteOrder()
    {
        // Senders transmit Aux as zero; a received value is read big-endian
        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int length = new ArtSyncPacket().WriteToBuffer(buffer);
        Assert.AreEqual(0, buffer[12] | buffer[13]);

        buffer[12] = 0x12;
        buffer[13] = 0x34;
        var parsed = (ArtSyncPacket)ArtNetPacket.Parse(buffer.AsMemory(0, length))!;

        Assert.AreEqual((ushort)0x1234, parsed.Aux);
    }
}
