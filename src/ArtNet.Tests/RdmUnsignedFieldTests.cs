using System.IO;

using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets;
using Haukcode.ArtNet.Rdm.Packets.Net;
using Haukcode.ArtNet.Rdm.Packets.Product;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Fields that E1.20 / E1.37-7 define as unsigned must survive values with the top bit set.
/// </summary>
[TestClass]
public class RdmUnsignedFieldTests
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
    public void BinaryIO_RoundTripsUnsignedValues()
    {
        using var stream = new MemoryStream();
        var writer = new RdmBinaryWriter(stream);
        writer.WriteUInt16((ushort)0xFFFE);
        writer.WriteUInt32(0xFFFFFFF0u);

        stream.Position = 0;
        var reader = new RdmBinaryReader(stream);

        Assert.AreEqual((ushort)0xFFFE, reader.ReadUInt16());
        Assert.AreEqual(0xFFFFFFF0u, reader.ReadUInt32());
    }

    [TestMethod]
    public void Header_AllSubDevices_RoundTrips()
    {
        var reply = new DeviceInfo.GetReply();
        reply.Header.SubDevice = 0xFFFF;

        var parsed = Decode(Encode(reply));

        Assert.AreEqual((ushort)0xFFFF, parsed.Header.SubDevice);
    }

    [TestMethod]
    public void Header_ManufacturerPid_IsRecognized()
    {
        var packet = new RdmRawPacket(RdmCommands.GetResponse, (RdmParameters)0x8123) { Data = [1, 2] };

        var parsed = Decode(Encode(packet));

        Assert.AreEqual((RdmParameters)0x8123, parsed.Header.ParameterId);
        Assert.IsTrue(parsed.Header.ParameterId.IsManufacturerPID());
    }

    [TestMethod]
    public void DeviceInfo_RoundTripsHighValues()
    {
        var reply = new DeviceInfo.GetReply
        {
            RdmProtocolVersion = 0x0100,
            DeviceModelId = 0x8001,
            ProductCategory = (ProductCategories)0x8002,
            SoftwareVersionId = 0x80000003,
            DmxFootprint = 512,
            DmxStartAddress = 1,
            SubDeviceCount = 0x8004,
        };

        var parsed = (DeviceInfo.GetReply)Decode(Encode(reply));

        Assert.AreEqual((ushort)0x8001, parsed.DeviceModelId);
        Assert.AreEqual((ProductCategories)0x8002, parsed.ProductCategory);
        Assert.AreEqual(0x80000003u, parsed.SoftwareVersionId);
        Assert.AreEqual((ushort)0x8004, parsed.SubDeviceCount);
    }

    [TestMethod]
    public void EndpointList_RoundTripsHighValues()
    {
        var reply = new EndpointList.Reply
        {
            ListChangeNumber = 0x80000001,
            EndpointIDs = [0x0001, 0xF000],
        };

        var parsed = (EndpointList.Reply)Decode(Encode(reply));

        Assert.AreEqual(0x80000001u, parsed.ListChangeNumber);
        CollectionAssert.AreEqual(new ushort[] { 0x0001, 0xF000 }, parsed.EndpointIDs);
    }

    [TestMethod]
    public void EndpointToUniverse_CompositeIsDetected()
    {
        var reply = new EndpointToUniverse.GetReply
        {
            EndpointID = 0x8000,
            EndpointMode = EndpointToUniverse.UniverseMode.Composite,
        };

        var parsed = (EndpointToUniverse.GetReply)Decode(Encode(reply));

        Assert.AreEqual((ushort)0x8000, parsed.EndpointID);
        Assert.AreEqual(EndpointToUniverse.UniverseMode.Composite, parsed.EndpointMode);
    }

    [TestMethod]
    public void EndpointToUniverse_HighUniverseRoundTrips()
    {
        var reply = new EndpointToUniverse.GetReply
        {
            EndpointMode = EndpointToUniverse.UniverseMode.Standard,
            UniverseNumber = 63999,
        };

        var parsed = (EndpointToUniverse.GetReply)Decode(Encode(reply));

        Assert.AreEqual(EndpointToUniverse.UniverseMode.Standard, parsed.EndpointMode);
        Assert.AreEqual((ushort)63999, parsed.UniverseNumber);
    }
}
