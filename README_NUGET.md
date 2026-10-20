# Haukcode.ArtNet

A high-performance ArtNet library for .NET (8.0+) enabling DMX512 lighting control over IP networks.

## What is ArtNet?

Art-Net is a royalty-free protocol for transmitting DMX512 lighting control data over UDP/IP networks. It's widely used in professional entertainment, architectural, and theatrical lighting.

## Features

- ✅ DMX transmission and reception
- ✅ Multiple universe support (up to 32,768 universes)
- ✅ Device discovery (ArtPoll)
- ✅ Frame synchronization (ArtSync)
- ✅ RDM support (Remote Device Management) - separate package, with request/response matching
- ✅ ArtTrigger for show control
- ✅ High-performance UDP communication
- ✅ Cross-platform (.NET 8.0+)

## Standards

Checked against the published documents: every packet Art-Net 4 (rev 1.4dp) defines a layout for, and every PID in ANSI E1.20-2025 (RDM), E1.37-1, E1.37-2, E1.37-7 and the RDM PIDs of E1.33. See the [compliance table](https://github.com/HakanL/Haukcode.ArtNet#standards-compliance) for details.

## Quick Start

### Installation

```bash
dotnet add package Haukcode.ArtNet
```

For RDM support:
```bash
dotnet add package Haukcode.ArtNet.Rdm
```

### Basic Usage

```csharp
using System.Net;
using Haukcode.ArtNet;

var localIp = IPAddress.Parse("192.168.1.100");
var subnetMask = IPAddress.Parse("255.255.255.0");

// Create client
using var client = new ArtNetClient(localIp, subnetMask);

// Send DMX data to a node (Art-Net 4 doesn't allow DMX to be broadcast)
var dmxData = new byte[512];
dmxData[0] = 255;  // Channel 1 at full
await client.SendDmxData(IPAddress.Parse("192.168.1.50"), universeId: 1, dmxData);
```

### Receiving DMX

```csharp
using System.Threading.Channels;
using Haukcode.ArtNet.Packets;

var channel = Channel.CreateUnbounded<ReceiveDataPacket>();

using var client = new ArtNetClient(
    localIp, subnetMask,
    channelWriter: async (p) => await channel.Writer.WriteAsync(p));

await foreach (var packet in channel.Reader.ReadAllAsync())
{
    if (packet.Packet is ArtNetDmxPacket dmx)
    {
        Console.WriteLine($"Universe {dmx.Universe + 1}: {dmx.DmxData[0]}");
    }
}
```

### Device Discovery

```csharp
// Send ArtPoll to discover devices
await client.QueuePacketForSending(null, new ArtPollPacket());
```

### Frame Synchronization

```csharp
// Send data to multiple universes
var node = IPAddress.Parse("192.168.1.50");
await client.SendDmxData(node, 1, dmxData1);
await client.SendDmxData(node, 2, dmxData2);

// Sync all outputs (always broadcast)
await client.SendSync();
```

## Key Methods

```csharp
// Send DMX data (address must be a node unless AllowDmxBroadcast is set)
Task SendDmxData(IPAddress? address, ushort universeId,
                 ReadOnlyMemory<byte> dmxData, bool important = false)

// Send sync packet (broadcast)
Task SendSync()

// Send any packet
Task QueuePacketForSending(IPAddress? destination,
                          ArtNetPacket packet, bool important = false)
```

## Supported Packets

- `ArtPoll` / `ArtPollReply` - Device discovery
- `ArtDmx`, `ArtNzs` / `ArtVlc` - DMX data
- `ArtSync` - Frame synchronization
- `ArtTrigger`, `ArtTimeCode`, `ArtCommand` - Show control
- `ArtAddress`, `ArtInput`, `ArtIpProg` - Node configuration
- `ArtDataRequest` / `ArtDataReply`, `ArtDiagData` - Product data and diagnostics
- `ArtTodRequest` / `ArtTodData` / `ArtTodControl` - RDM device discovery
- `ArtRdm` / `ArtRdmSub` - RDM commands (requires RDM package)
- `ArtFirmwareMaster` / `ArtFirmwareReply` - Firmware upload

## Platform Support

- .NET 8.0+, 9.0+, 10.0+
- Windows, Linux, macOS

## Documentation & Examples

📖 **Full Documentation**: https://github.com/HakanL/Haukcode.ArtNet

🔧 **Sample Code**: Included in the GitHub repository

## Resources

- [Art-Net Specification](https://www.artisticlicence.com/)
- [GitHub Repository](https://github.com/HakanL/Haukcode.ArtNet)
- [Report Issues](https://github.com/HakanL/Haukcode.ArtNet/issues)

## License

MIT License - Copyright (c) 2018-2024 Hakan Lindestaf
