using System.Net;

using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Management;
using Haukcode.ArtNet.Rdm.Packets.Net;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Gateway PIDs and layouts per published ANSI E1.37-7-2019 and E1.33-2019.
/// </summary>
[TestClass]
public class RdmGatewayPidTests
{
    private const int ParameterDataLengthOffset = 21;

    private static T RoundTrip<T>(T packet, int expectedPdl)
        where T : RdmPacket
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));
        var wire = stream.ToArray();

        Assert.AreEqual((byte)expectedPdl, wire[ParameterDataLengthOffset], $"{typeof(T).FullName} PDL");

        using var input = new MemoryStream(wire);
        return Assert.IsInstanceOfType<T>(RdmPacket.ReadPacket(new RdmBinaryReader(input)));
    }

    [TestMethod]
    [DataRow(RdmParameters.EndpointList, 0x0900)]
    [DataRow(RdmParameters.EndpointListChange, 0x0901)]
    [DataRow(RdmParameters.EndpointIdentify, 0x0902)]
    [DataRow(RdmParameters.EndpointToUniverse, 0x0903)]
    [DataRow(RdmParameters.EndpointMode, 0x0904)]
    [DataRow(RdmParameters.EndpointLabel, 0x0905)]
    [DataRow(RdmParameters.RdmTrafficEnable, 0x0906)]
    [DataRow(RdmParameters.DiscoveryState, 0x0907)]
    [DataRow(RdmParameters.BackgroundDiscovery, 0x0908)]
    [DataRow(RdmParameters.EndpointTiming, 0x0909)]
    [DataRow(RdmParameters.EndpointTimingDescription, 0x090A)]
    [DataRow(RdmParameters.EndpointDevices, 0x090B)]
    [DataRow(RdmParameters.EndpointDeviceListChange, 0x090C)]
    [DataRow(RdmParameters.BindingControlFields, 0x090D)]
    [DataRow(RdmParameters.BackgroundQueuedStatusPolicy, 0x090E)]
    [DataRow(RdmParameters.BackgroundQueuedStatusPolicyDescription, 0x090F)]
    [DataRow(RdmParameters.TcpCommsStatus, 0x0802)]
    public void PidValues_MatchThePublishedStandards(RdmParameters pid, int expected)
    {
        Assert.AreEqual(expected, (int)pid);
    }

    [TestMethod]
    public void EndpointSetReplies_CarryTheEndpointId()
    {
        Assert.AreEqual((ushort)7, RoundTrip(new EndpointIdentify.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new EndpointToUniverse.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new EndpointMode.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new EndpointLabel.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new RdmTrafficEnable.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new DiscoveryState.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new BackgroundDiscovery.SetReply { EndpointID = 7 }, 2).EndpointID);
        Assert.AreEqual((ushort)7, RoundTrip(new EndpointTiming.SetReply { EndpointID = 7 }, 2).EndpointID);
    }

    [TestMethod]
    public void EndpointList_RecordsAreIdAndType()
    {
        var reply = new EndpointList.Reply
        {
            ListChangeNumber = 5,
            Endpoints = [new(1, EndpointList.EndpointTypes.Physical), new(2, EndpointList.EndpointTypes.Virtual)],
        };

        CollectionAssert.AreEqual(reply.Endpoints, RoundTrip(reply, 4 + 2 * 3).Endpoints);
    }

    [TestMethod]
    public void EndpointToUniverse_GetReplyIsEndpointAndUniverse()
    {
        var reply = RoundTrip(new EndpointToUniverse.GetReply { EndpointID = 3, EndpointMode = EndpointToUniverse.UniverseMode.Standard, UniverseNumber = 1234 }, 4);

        Assert.AreEqual((ushort)1234, reply.UniverseNumber);
    }

    [TestMethod]
    public void DiscoveryState_ValuesMatchTableA2()
    {
        Assert.AreEqual(0x01, (int)DiscoveryState.DiscoveryStates.Incremental);
        Assert.AreEqual(0x02, (int)DiscoveryState.DiscoveryStates.Full);
    }

    [TestMethod]
    public void BindingControlFields_EndpointThenUid()
    {
        var uid = new UId(0x1234, 0x56789ABC);

        var get = RoundTrip(new BindingControlFields.Get { EndpointID = 9, Id = uid }, 8);
        Assert.AreEqual((ushort)9, get.EndpointID);
        Assert.AreEqual(uid, get.Id);

        var reply = RoundTrip(new BindingControlFields.GetReply { EndpointID = 9, Id = uid, ControlFields = 0x8000, BindingId = new UId(0x1234, 1) }, 16);
        Assert.AreEqual((ushort)9, reply.EndpointID);
        Assert.AreEqual(uid, reply.Id);
        Assert.AreEqual((ushort)0x8000, reply.ControlFields);
    }

    [TestMethod]
    public void BackgroundQueuedStatusPolicy_HasNoEndpoint()
    {
        RoundTrip(new BackgroundQueuedStatusPolicy.Get(), 0);
        Assert.AreEqual((byte)3, RoundTrip(new BackgroundQueuedStatusPolicy.GetReply { CurrentPolicyID = 1, PolicyCount = 3 }, 2).PolicyCount);
        Assert.AreEqual((byte)2, RoundTrip(new BackgroundQueuedStatusPolicy.Set { PolicyID = 2 }, 1).PolicyID);
    }

    [TestMethod]
    public void TcpCommsStatus_UsesTheE133Layout()
    {
        var reply = RoundTrip(new TcpCommsStatus.GetReply
        {
            Scope = "default",
            BrokerIPv4Address = IPAddress.Parse("192.168.240.10"),
            BrokerIPv6Address = IPAddress.Parse("fe80::1"),
            BrokerPort = 8888,
            UnhealthyTcpEvents = 3,
        }, 87);

        Assert.AreEqual("default", reply.Scope);
        Assert.AreEqual(IPAddress.Parse("192.168.240.10"), reply.BrokerIPv4Address);
        Assert.AreEqual(IPAddress.Parse("fe80::1"), reply.BrokerIPv6Address);
        Assert.AreEqual((ushort)8888, reply.BrokerPort);
        Assert.AreEqual((ushort)3, reply.UnhealthyTcpEvents);

        Assert.AreEqual("default", RoundTrip(new TcpCommsStatus.Set { Scope = "default" }, 63).Scope);
    }

    [TestMethod]
    public void NackReason_HasTheE120_2025Table()
    {
        Assert.AreEqual(0x0B, (int)NackReason.ActionNotSupported);
        Assert.AreEqual(0x0E, (int)NackReason.UnknownUid);
        Assert.AreEqual(0x13, (int)NackReason.InvalidPort);
        Assert.AreEqual(0x20, (int)NackReason.AllCallSetFail);
    }
}
