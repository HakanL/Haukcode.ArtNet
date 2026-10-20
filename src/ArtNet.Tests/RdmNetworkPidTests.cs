using System.Net;

using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Net;
using Haukcode.ArtNet.Rdm.Packets.Network;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// E1.37-2 IPv4/DNS PIDs and the E1.33 COMPONENT_SCOPE / SEARCH_DOMAIN / BROKER_STATUS PIDs (#33),
/// with PDLs from the spec text.
/// </summary>
[TestClass]
public class RdmNetworkPidTests
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
    [DataRow(RdmParameters.ListInterfaces, 0x0700)]
    [DataRow(RdmParameters.InterfaceHardwareAddressType1, 0x0702)]
    [DataRow(RdmParameters.InterfaceApplyConfiguration, 0x0709)]
    [DataRow(RdmParameters.DnsDomainName, 0x070D)]
    [DataRow(RdmParameters.ComponentScope, 0x0800)]
    [DataRow(RdmParameters.BrokerStatus, 0x0803)]
    public void PidValues_MatchTheStandards(RdmParameters pid, int expected)
    {
        Assert.AreEqual(expected, (int)pid);
    }

    [TestMethod]
    public void InterfacePids_RoundTrip()
    {
        var list = RoundTrip(new ListInterfaces.GetReply { Interfaces = [new(1, 1), new(2, 1)] }, 12);
        Assert.AreEqual(2u, list.Interfaces[1].Id);

        Assert.AreEqual("eth0", RoundTrip(new InterfaceLabel.GetReply { InterfaceId = 1, Label = "eth0" }, 8).Label);

        var mac = RoundTrip(new InterfaceHardwareAddressType1.GetReply { InterfaceId = 1, HardwareAddress = [0x00, 0x1A, 0x19, 0x32, 0x01, 0x22] }, 0x0A);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x1A, 0x19, 0x32, 0x01, 0x22 }, mac.HardwareAddress);

        Assert.IsTrue(RoundTrip(new Ipv4DhcpMode.Set { InterfaceId = 1, Enabled = true }, 5).Enabled);
        Assert.IsFalse(RoundTrip(new Ipv4ZeroconfMode.GetReply { InterfaceId = 1, Enabled = false }, 5).Enabled);

        Assert.AreEqual(3u, RoundTrip(new InterfaceApplyConfiguration.Set { InterfaceId = 3 }, 4).InterfaceId);
        RoundTrip(new InterfaceRenewDhcp.Set { InterfaceId = 3 }, 4);
        RoundTrip(new InterfaceReleaseDhcp.Set { InterfaceId = 3 }, 4);
    }

    [TestMethod]
    public void AddressPids_RoundTrip()
    {
        var current = RoundTrip(new Ipv4CurrentAddress.GetReply
        {
            InterfaceId = 1,
            Address = IPAddress.Parse("192.168.240.32"),
            Netmask = 23,
            DhcpStatus = Ipv4CurrentAddress.DhcpStatuses.Inactive,
        }, 0x0A);
        Assert.AreEqual(IPAddress.Parse("192.168.240.32"), current.Address);
        Assert.AreEqual((byte)23, current.Netmask);

        Assert.AreEqual((byte)24, RoundTrip(new Ipv4StaticAddress.Set { InterfaceId = 1, Address = IPAddress.Parse("10.0.0.5"), Netmask = 24 }, 9).Netmask);

        var route = RoundTrip(new Ipv4DefaultRoute.GetReply { Route = IPAddress.Parse("192.168.240.1") }, 8);
        Assert.AreEqual(IPAddress.Parse("192.168.240.1"), route.Route);
        Assert.AreEqual(0u, route.InterfaceId);

        Assert.AreEqual((byte)2, RoundTrip(new DnsIpv4NameServer.Get { Index = 2 }, 1).Index);
        Assert.AreEqual(IPAddress.Parse("1.1.1.1"), RoundTrip(new DnsIpv4NameServer.Set { Index = 0, Address = IPAddress.Parse("1.1.1.1") }, 5).Address);
    }

    [TestMethod]
    public void NamePids_AllowTheirFullLength()
    {
        // Host names go up to 63 characters and domain names up to 231, beyond the usual 32
        var host = new string('h', 63);
        Assert.AreEqual(host, RoundTrip(new DnsHostname.Set { Hostname = host + "x" }, 63).Hostname);

        var domain = new string('d', 100) + ".example";
        Assert.AreEqual(domain, RoundTrip(new DnsDomainName.GetReply { DomainName = domain }, domain.Length).DomainName);
        Assert.AreEqual(string.Empty, RoundTrip(new DnsDomainName.Set { DomainName = "" }, 0).DomainName);

        Assert.AreEqual("lighting.local", RoundTrip(new SearchDomain.Set { DomainName = "lighting.local" }, 14).DomainName);
    }

    [TestMethod]
    public void E133Pids_RoundTrip()
    {
        var scope = RoundTrip(new ComponentScope.Set
        {
            ScopeSlot = 1,
            Scope = "default",
            StaticConfigType = ComponentScope.StaticConfigTypes.IPv4,
            StaticIPv4Address = IPAddress.Parse("192.168.240.10"),
            StaticPort = 8888,
        }, 0x58); // The spec says 0x54, but its fields add up to 0x58
        Assert.AreEqual("default", scope.Scope);
        Assert.AreEqual(ComponentScope.StaticConfigTypes.IPv4, scope.StaticConfigType);
        Assert.AreEqual(IPAddress.Parse("192.168.240.10"), scope.StaticIPv4Address);
        Assert.AreEqual((ushort)8888, scope.StaticPort);

        Assert.AreEqual((ushort)2, RoundTrip(new ComponentScope.Get { ScopeSlot = 2 }, 2).ScopeSlot);

        var status = RoundTrip(new BrokerStatus.GetReply { SetAllowed = true, State = BrokerStatus.BrokerStates.Standby }, 2);
        Assert.AreEqual(BrokerStatus.BrokerStates.Standby, status.State);
        Assert.AreEqual(BrokerStatus.BrokerStates.Disabled, RoundTrip(new BrokerStatus.Set { State = BrokerStatus.BrokerStates.Disabled }, 1).State);
    }
}
