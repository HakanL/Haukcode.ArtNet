using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Control;
using Haukcode.ArtNet.Rdm.Packets.DMX;
using Haukcode.ArtNet.Rdm.Packets.Net;
using Haukcode.ArtNet.Rdm.Packets.Parameters;
using Haukcode.ArtNet.Rdm.Packets.Product;
using Haukcode.ArtNet.Rdm.Packets.Sensors;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class RdmTextAndValueTests
{
    private static byte[] Encode(RdmPacket packet)
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));
        return stream.ToArray();
    }

    private static T Read<T>(RdmCommands command, RdmParameters pid, byte[] parameterData)
        where T : RdmPacket
    {
        var header = new RdmHeader { Command = command, ParameterId = pid, ParameterDataLength = parameterData.Length };

        using var content = new MemoryStream(parameterData);
        return Assert.IsInstanceOfType<T>(RdmPacket.ReadPacket(header, new RdmBinaryReader(content)));
    }

    [TestMethod]
    public void Text_EndsAtTheFirstNul()
    {
        // E1.20 10.1: a NULL also acts as a terminator
        Assert.AreEqual("ABC", Read<DeviceLabel.GetReply>(RdmCommands.GetResponse, RdmParameters.DeviceLabel, "ABC\0xyz"u8.ToArray()).Label);
        Assert.AreEqual("ABC", Read<DeviceLabel.Set>(RdmCommands.Set, RdmParameters.DeviceLabel, "ABC\0\0"u8.ToArray()).Label);
        Assert.AreEqual("in", Read<EndpointLabel.GetReply>(RdmCommands.GetResponse, RdmParameters.EndpointLabel, [0, 1, (byte)'i', (byte)'n', 0, 0]).Label);
    }

    [TestMethod]
    [DataRow(ParameterDataTypes.Int8, -1L, 0x000000FFu)]
    [DataRow(ParameterDataTypes.UInt8, 0x12L, 0x00000012u)]
    [DataRow(ParameterDataTypes.Int16, -2L, 0x0000FFFEu)]
    [DataRow(ParameterDataTypes.UInt16, 0xFFFFL, 0x0000FFFFu)]
    [DataRow(ParameterDataTypes.Int32, -1L, 0xFFFFFFFFu)]
    [DataRow(ParameterDataTypes.UInt32, 0xFFFFFFFFL, 0xFFFFFFFFu)]
    [DataRow(ParameterDataTypes.String, 5L, 0u)]
    public void ParameterDescription_ValuesFollowTheDataType(ParameterDataTypes dataType, long value, uint wire)
    {
        Assert.AreEqual(wire, ParameterDescription.EncodeValue(value, dataType));

        // Non-integer types carry zero, which reads back as zero
        Assert.AreEqual(wire == 0 ? 0 : value, ParameterDescription.DecodeValue(wire, dataType));
    }

    [TestMethod]
    public void TcpCommsStatus_ScopeAlwaysEndsWithANul()
    {
        var wire = Encode(new TcpCommsStatus.Set { Scope = new string('s', 70) });

        // Header (22) + 63-byte scope; the last scope byte is the terminator
        Assert.AreEqual((byte)0, wire[22 + 62]);
        Assert.AreEqual((byte)'s', wire[22 + 61]);
    }

    [TestMethod]
    public void EndpointToUniverseSet_RejectsComposite()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => Encode(new EndpointToUniverse.Set { EndpointID = 1, EndpointMode = EndpointToUniverse.UniverseMode.Composite }));
        Assert.ThrowsExactly<InvalidOperationException>(() => Encode(new EndpointToUniverse.Set { EndpointID = 1, UniverseNumber = 64000 }));

        // A reply can be built without setting the mode first
        Assert.AreEqual((ushort)5, new EndpointToUniverse.GetReply { UniverseNumber = 5 }.UniverseNumber);
    }

    [TestMethod]
    public void DmxPersonalityDescription_EmptyDescriptionIsEmpty()
    {
        Assert.AreEqual(string.Empty, Read<DmxPersonalityDescription.GetReply>(RdmCommands.GetResponse, RdmParameters.DmxPersonalityDescription, [1, 0, 4]).Description);
    }

    [TestMethod]
    public void SpecialValues_AreRefused()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => Encode(new SensorValue.Get { SensorNumber = 0xFF }));
        Assert.ThrowsExactly<InvalidOperationException>(() => Encode(new PresetPlayback.Set { Mode = PresetPlayback.PlayMode.Scene, SceneNumber = 0 }));
    }

    [TestMethod]
    public void DeviceInfo_DefaultsToRdmProtocolVersion2()
    {
        Assert.AreEqual(DeviceInfo.RdmProtocolVersion2, new DeviceInfo.GetReply().RdmProtocolVersion);
    }
}
