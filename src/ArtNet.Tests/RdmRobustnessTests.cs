using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Configuration;
using Haukcode.ArtNet.Rdm.Packets.Parameters;
using Haukcode.ArtNet.Rdm.Packets.Product;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class RdmRobustnessTests
{
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

    /// <summary>The bytes ArtRdm carries: the message followed by its checksum.</summary>
    private static byte[] WithChecksum(byte[] message)
    {
        ushort sum = (ushort)(0xCC + 0x01 + message.Sum(b => b));
        return [.. message, (byte)(sum >> 8), (byte)sum];
    }

    [TestMethod]
    public void WriteLength_RejectsMoreThan231BytesOfParameterData()
    {
        var reply = new SupportedParameters.GetReply();
        reply.ParameterIds.AddRange(Enumerable.Repeat(RdmParameters.DeviceLabel, 116));

        Assert.ThrowsExactly<InvalidOperationException>(() => Encode(reply));

        reply.ParameterIds.RemoveAt(0);
        Assert.AreEqual((byte)230, Encode(reply)[ParameterDataLengthOffset]);
    }

    [TestMethod]
    public void ShortReply_IsRejected_AndAnEmptyDescriptionReadsAsEmpty()
    {
        // PARAMETER_DESCRIPTION has 20 fixed bytes before the description
        RdmPacket Read(int parameterDataLength)
        {
            var header = new RdmHeader
            {
                Command = RdmCommands.GetResponse,
                ParameterId = RdmParameters.ParameterDescription,
                ParameterDataLength = parameterDataLength,
            };

            using var content = new MemoryStream(new byte[parameterDataLength]);
            return RdmPacket.ReadPacket(header, new RdmBinaryReader(content));
        }

        // The body is bounded by PDL, so a 19-byte reply can't read past its parameter data
        Assert.ThrowsExactly<InvalidPacketException>(() => Read(19));

        var reply = Assert.IsInstanceOfType<ParameterDescription.GetReply>(Read(20));
        Assert.AreEqual(string.Empty, reply.Description);
    }

    [TestMethod]
    public void ReadOverflowPacket_JoinsTheParameterData()
    {
        // SUPPORTED_PARAMETERS split over two messages: ACK_OVERFLOW, then ACK
        byte[] Message(RdmResponseTypes responseType, params RdmParameters[] pids)
        {
            var reply = new SupportedParameters.GetReply { ResponseType = responseType };
            reply.ParameterIds.AddRange(pids);
            return WithChecksum(Encode(reply));
        }

        var packet = RdmPacket.ReadOverflowPacket(
        [
            Message(RdmResponseTypes.AckOverflow, RdmParameters.DeviceLabel, RdmParameters.DeviceInfo),
            Message(RdmResponseTypes.Ack, RdmParameters.SoftwareVersionLabel),
        ]);

        var reply = Assert.IsInstanceOfType<SupportedParameters.GetReply>(packet);
        CollectionAssert.AreEqual(new[] { RdmParameters.DeviceLabel, RdmParameters.DeviceInfo, RdmParameters.SoftwareVersionLabel }, reply.ParameterIds);
    }

    [TestMethod]
    public void DiscoveryMuteNack_IsReadAsNack()
    {
        var nack = new Haukcode.ArtNet.Rdm.Packets.Management.RdmNack(RdmCommands.DiscoveryResponse, RdmParameters.DiscoveryMute)
        {
            Reason = Haukcode.ArtNet.Rdm.Packets.Management.NackReason.FormatError,
        };

        var parsed = Decode(Encode(nack));

        Assert.IsInstanceOfType<Haukcode.ArtNet.Rdm.Packets.Management.RdmNack>(parsed);
    }

    [TestMethod]
    public void Text_IsCappedAt32Characters()
    {
        var wire = Encode(new DeviceLabel.GetReply { Label = new string('x', 40) });

        Assert.AreEqual((byte)32, wire[ParameterDataLengthOffset]);
    }

    [TestMethod]
    public void Language_KeepsTheRawCode()
    {
        var reply = (Language.GetReply)Decode(Encode(new Language.GetReply { LanguageCode = "de" }));
        Assert.AreEqual("de", reply.LanguageCode);
        Assert.AreEqual("de", reply.Language?.TwoLetterISOLanguageName);

        // Unknown codes parse instead of throwing CultureNotFoundException
        var unknown = (Language.GetReply)Decode(Encode(new Language.GetReply { LanguageCode = "q!" }));
        Assert.AreEqual("q!", unknown.LanguageCode);

        var capabilities = (LanguageCapabilities.GetReply)Decode(Encode(new LanguageCapabilities.GetReply { LanguageCodes = ["en", "de", "q!"] }));
        CollectionAssert.AreEqual(new[] { "en", "de", "q!" }, capabilities.LanguageCodes);
    }

    [TestMethod]
    public void RealTimeClock_ToleratesInvalidAndLeapSecondValues()
    {
        var unset = (RealTimeClock.GetReply)Decode(Encode(new RealTimeClock.GetReply { Clock = new(2026, 0, 0, 0, 0, 0) }));
        Assert.AreEqual((byte)0, unset.Clock.Month);
        Assert.IsNull(unset.ClockTime);

        var leap = (RealTimeClock.GetReply)Decode(Encode(new RealTimeClock.GetReply { Clock = new(2026, 12, 31, 23, 59, 60) }));
        Assert.AreEqual((byte)60, leap.Clock.Second);
        Assert.AreEqual(new DateTime(2026, 12, 31, 23, 59, 59), leap.ClockTime);

        var time = new DateTime(2026, 10, 20, 11, 12, 13);
        Assert.AreEqual(time, ((RealTimeClock.Set)Decode(Encode(new RealTimeClock.Set { ClockTime = time }))).ClockTime);
    }

    [TestMethod]
    public void Checksum_IsReadAndValidated()
    {
        var data = WithChecksum(Encode(new DeviceLabel.GetReply { Label = "lamp" }));

        Assert.IsTrue(RdmPacket.IsChecksumValid(data));

        var reply = Assert.IsInstanceOfType<DeviceLabel.GetReply>(RdmPacket.ReadPacket(data));
        Assert.AreEqual("lamp", reply.Label);
        Assert.AreEqual((ushort)((data[^2] << 8) | data[^1]), reply.Checksum);

        var corrupted = (byte[])data.Clone();
        corrupted[^3] ^= 0x01;
        Assert.IsFalse(RdmPacket.IsChecksumValid(corrupted));
        Assert.ThrowsExactly<InvalidPacketException>(() => RdmPacket.ReadPacket(corrupted));

        // Missing checksum
        Assert.IsFalse(RdmPacket.IsChecksumValid(data[..^2]));
    }
}
