using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class ArtNetDmxPacketParseTests
{
    private static byte[] Encode(short universe, byte sequence, byte[] dmx)
    {
        var packet = new ArtNetDmxPacket { Universe = universe, Sequence = sequence, Physical = 1, DmxData = dmx };
        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int length = packet.WriteToBuffer(buffer);

        return buffer[..length];
    }

    [TestMethod]
    public void ScratchParse_FillsTheSameInstance_AndMatchesTheAllocatingParse()
    {
        var scratch = new ArtNetDmxPacket();
        var first = Encode(5, 9, [1, 2, 3, 4]);
        var second = Encode(700, 10, [255, 0]);

        var a = ArtNetPacket.Parse(first, scratch);
        Assert.AreSame(scratch, a);
        Assert.AreEqual((short)5, scratch.Universe);
        Assert.AreEqual((byte)9, scratch.Sequence);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, scratch.DmxData.ToArray());

        var b = ArtNetPacket.Parse(second, scratch);
        Assert.AreSame(scratch, b);
        Assert.AreEqual((short)700, scratch.Universe);
        Assert.AreEqual((byte)10, scratch.Sequence);
        CollectionAssert.AreEqual(new byte[] { 255, 0 }, scratch.DmxData.ToArray());

        var allocated = (ArtNetDmxPacket)ArtNetPacket.Parse(second)!;
        Assert.AreNotSame(scratch, allocated);
        Assert.AreEqual(scratch.Universe, allocated.Universe);
        Assert.AreEqual(scratch.Sequence, allocated.Sequence);
        CollectionAssert.AreEqual(allocated.DmxData.ToArray(), scratch.DmxData.ToArray());
    }

    [TestMethod]
    public void ScratchParse_LeavesOtherOpcodesAllocated()
    {
        var scratch = new ArtNetDmxPacket();
        var poll = new ArtPollPacket();
        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int length = poll.WriteToBuffer(buffer);

        var parsed = ArtNetPacket.Parse(buffer[..length], scratch);
        Assert.IsInstanceOfType<ArtPollPacket>(parsed);
        Assert.AreNotSame(scratch, parsed);
    }
}
