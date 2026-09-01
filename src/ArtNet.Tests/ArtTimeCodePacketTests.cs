using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class ArtTimeCodePacketTests
{
    // Art-Net\0 + OpTimeCode 0x9700 LE + ProtVer 14 + filler/stream/time/type
    private static readonly byte[] SampleFilm24 =
    [
        (byte)'A', (byte)'r', (byte)'t', (byte)'-', (byte)'N', (byte)'e', (byte)'t', 0x00,
        0x00, 0x97,
        0x00, 0x0E,
        0x00,
        0x00,
        0x06, 0x21, 0x01, 0x00,
        0x00
    ];

    [TestMethod]
    public void PacketLength_Is19Bytes()
    {
        var packet = new ArtTimeCodePacket();

        Assert.AreEqual(19, packet.PacketLength);
    }

    [TestMethod]
    public void Parse_SampleLayout_MatchesSpecOffsets()
    {
        var parsed = ArtNetPacket.Parse(SampleFilm24) as ArtTimeCodePacket;

        Assert.IsNotNull(parsed);
        Assert.AreEqual(ArtNetOpCodes.TimeCode, parsed.OpCode);
        Assert.AreEqual(0, parsed.Filler1);
        Assert.AreEqual(0, parsed.StreamId);
        Assert.AreEqual(0x06, parsed.Frames);
        Assert.AreEqual(0x21, parsed.Seconds);
        Assert.AreEqual(0x01, parsed.Minutes);
        Assert.AreEqual(0x00, parsed.Hours);
        Assert.AreEqual(ArtTimeCodeTypes.Film, parsed.Type);
    }

    [TestMethod]
    [DataRow(ArtTimeCodeTypes.Film)]
    [DataRow(ArtTimeCodeTypes.Ebu)]
    [DataRow(ArtTimeCodeTypes.DropFrame)]
    [DataRow(ArtTimeCodeTypes.Smpte)]
    public void RoundTrip_PreservesFields(ArtTimeCodeTypes type)
    {
        var original = new ArtTimeCodePacket
        {
            StreamId = 3,
            Frames = 17,
            Seconds = 44,
            Minutes = 12,
            Hours = 1,
            Type = type
        };

        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int written = original.WriteToBuffer(buffer);

        Assert.AreEqual(19, written);

        var parsed = ArtNetPacket.Parse(buffer.AsMemory(0, written)) as ArtTimeCodePacket;

        Assert.IsNotNull(parsed);
        Assert.AreEqual(original.StreamId, parsed.StreamId);
        Assert.AreEqual(original.Frames, parsed.Frames);
        Assert.AreEqual(original.Seconds, parsed.Seconds);
        Assert.AreEqual(original.Minutes, parsed.Minutes);
        Assert.AreEqual(original.Hours, parsed.Hours);
        Assert.AreEqual(original.Type, parsed.Type);
        Assert.AreEqual(0, parsed.Filler1);
    }

    [TestMethod]
    public void Parse_PaddedDatagram_StillReadsTime()
    {
        var padded = new byte[SampleFilm24.Length + 4];
        SampleFilm24.CopyTo(padded, 0);

        var parsed = ArtNetPacket.Parse(padded) as ArtTimeCodePacket;

        Assert.IsNotNull(parsed);
        Assert.AreEqual(1, parsed.Minutes);
        Assert.AreEqual(ArtTimeCodeTypes.Film, parsed.Type);
    }

    [TestMethod]
    public void Parse_DoesNotFallThroughToUnknownPacket()
    {
        var parsed = ArtNetPacket.Parse(SampleFilm24);

        Assert.IsInstanceOfType<ArtTimeCodePacket>(parsed);
        Assert.IsNotInstanceOfType<ArtNetUnknownPacket>(parsed);
    }
}
