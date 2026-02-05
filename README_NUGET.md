# Haukcode.ArtNet

A high-performance ArtNet library for .NET (8.0+) enabling DMX512 lighting control over IP networks.

## What is ArtNet?

Art-Net is a royalty-free protocol for transmitting DMX512 lighting control data over UDP/IP networks. It's widely used in professional entertainment, architectural, and theatrical lighting.

## Features

- ✅ DMX transmission and reception
- ✅ Multiple universe support (up to 32,768 universes)
- ✅ Device discovery (ArtPoll)
- ✅ Frame synchronization (ArtSync)
- ✅ RDM support (Remote Device Management) - separate package
- ✅ ArtTrigger for show control
- ✅ High-performance UDP communication
- ✅ Cross-platform (.NET 8.0+)

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

// Send DMX data
var dmxData = new byte[512];
dmxData[0] = 255;  // Channel 1 at full
await client.SendDmxData(null, universeId: 1, dmxData);
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
await client.SendDmxData(null, 1, dmxData1);
await client.SendDmxData(null, 2, dmxData2);

// Sync all outputs
await client.SendSync(null);
```

## Key Methods

```csharp
// Send DMX data (null = broadcast to all)
Task SendDmxData(IPAddress? address, ushort universeId, 
                 ReadOnlyMemory<byte> dmxData, bool important = false)

// Send sync packet
Task SendSync(IPAddress? address)

// Send any packet
Task QueuePacketForSending(IPAddress? destination, 
                          ArtNetPacket packet, bool important = false)
```

## Supported Packets

- `ArtPoll` / `ArtPollReply` - Device discovery
- `ArtDmx` - DMX data transmission
- `ArtSync` - Frame synchronization
- `ArtTrigger` - Show control triggers
- `ArtAddress` - Node configuration
- `ArtTodRequest` / `ArtTodData` - RDM device discovery
- `ArtRdm` - RDM commands (requires RDM package)
- `ArtIpProg` - Remote IP configuration

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
