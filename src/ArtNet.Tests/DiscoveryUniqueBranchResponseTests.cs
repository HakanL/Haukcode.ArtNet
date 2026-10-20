using Haukcode.ArtNet.Rdm.Packets.Discovery;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class DiscoveryUniqueBranchResponseTests
{
    [TestMethod]
    public void Encode_MatchesTheSpecLayout()
    {
        // UID 1234:56789ABC with no preamble: separator, each UID byte as (b | 0xAA, b | 0x55),
        // then the 16-bit sum of those 12 bytes encoded the same way
        var encoded = DiscoveryUniqueBranchResponse.Encode(new UId(0x1234, 0x56789ABC), preambleLength: 0);

        byte[] expected =
        [
            0xAA,
            0xBA, 0x57, 0xBE, 0x75, 0xFE, 0x57, 0xFA, 0x7D, 0xBA, 0xDF, 0xBE, 0xFD,
            // Checksum 0x0864
            0xAA, 0x5D, 0xEE, 0x75,
        ];

        CollectionAssert.AreEqual(expected, encoded);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(3)]
    [DataRow(7)]
    public void Decode_RoundTrips(int preambleLength)
    {
        var uid = new UId(0xFFFF, 0xFFFFFFFE);

        Assert.IsTrue(DiscoveryUniqueBranchResponse.TryDecode(DiscoveryUniqueBranchResponse.Encode(uid, preambleLength), out var decoded));
        Assert.AreEqual(uid, decoded);
    }

    [TestMethod]
    public void Decode_RejectsCollidingResponses()
    {
        // Two responders on the same line: the receiver sees roughly the bitwise OR of both. (Not
        // every collision is caught, e.g. UIDs differing only in the lowest bits can OR into a
        // valid response, which is why discovery verifies a found UID with DISC_MUTE.)
        var a = DiscoveryUniqueBranchResponse.Encode(new UId(0x1234, 0x56789ABC));
        var b = DiscoveryUniqueBranchResponse.Encode(new UId(0x4321, 0x0FEDCBA9));
        var collided = a.Zip(b, (x, y) => (byte)(x | y)).ToArray();

        Assert.IsFalse(DiscoveryUniqueBranchResponse.TryDecode(collided, out _));
    }

    [TestMethod]
    public void Decode_RejectsMalformedData()
    {
        var valid = DiscoveryUniqueBranchResponse.Encode(new UId(0x1234, 0x56789ABC), preambleLength: 2);

        // Truncated
        Assert.IsFalse(DiscoveryUniqueBranchResponse.TryDecode(valid.AsSpan(0, valid.Length - 1), out _));

        // Missing separator
        var noSeparator = (byte[])valid.Clone();
        noSeparator[2] = 0x00;
        Assert.IsFalse(DiscoveryUniqueBranchResponse.TryDecode(noSeparator, out _));

        // Preamble longer than 7 bytes
        Assert.IsFalse(DiscoveryUniqueBranchResponse.TryDecode([0xFE, 0xFE, .. DiscoveryUniqueBranchResponse.Encode(new UId(1, 1))], out _));

        // A flipped checksum bit
        var badChecksum = (byte[])valid.Clone();
        badChecksum[^1] ^= 0x02;
        Assert.IsFalse(DiscoveryUniqueBranchResponse.TryDecode(badChecksum, out _));
    }
}
