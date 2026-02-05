# Haukcode.ArtNet [![NuGet Version](http://img.shields.io/nuget/v/Haukcode.ArtNet.svg?style=flat)](https://www.nuget.org/packages/Haukcode.ArtNet/)

A high-performance ArtNet library for .NET (8.0+) written in C#. Based on the abandoned [Architecture for Control Networks (ACN)](http://acn.codeplex.com) project codebase.

## What is ArtNet?

**Art-Net** (Artistic License Network Protocol) is a royalty-free communications protocol for transmitting DMX512 lighting control data over UDP/IP networks. It was developed by Artistic Licence Holdings Ltd. and has become one of the most widely used protocols for networked lighting control in professional entertainment, architectural, and theatrical lighting applications.

### Key Features of the ArtNet Protocol:
- **DMX over IP**: Transmits standard DMX512 lighting data over Ethernet networks
- **Multiple Universes**: Supports up to 32,768 universes (each with 512 channels)
- **Device Discovery**: Built-in polling mechanism to discover ArtNet devices on the network
- **RDM Support**: Remote Device Management for configuring and monitoring fixtures
- **Broadcast and Unicast**: Supports both broadcast (all devices) and unicast (specific device) transmission
- **Synchronization**: Frame synchronization for coordinated output across multiple nodes
- **Trigger Messages**: Send custom trigger commands for show control
- **IP Configuration**: Remote configuration of network settings on ArtNet nodes

## Library Features

This library provides a complete implementation of the Art-Net protocol with the following capabilities:

### Core Functionality
- ✅ **High Performance**: Built on the HighPerfComm library for optimal throughput
- ✅ **DMX Transmission**: Send DMX512 data to lighting fixtures and controllers
- ✅ **DMX Reception**: Receive DMX data from other ArtNet nodes
- ✅ **Multiple Universes**: Support for multiple DMX universes simultaneously
- ✅ **Device Discovery**: Poll and discover ArtNet devices on the network
- ✅ **Broadcast & Unicast**: Send to all devices or specific nodes
- ✅ **Frame Synchronization**: ArtSync support for synchronized output

### Advanced Features
- ✅ **RDM Support**: Full Remote Device Management (separate Haukcode.ArtNet.Rdm package)
  - Device discovery (TOD - Table of Devices)
  - Parameter queries and configuration
  - Device identification
  - Sensor monitoring
- ✅ **ArtTrigger**: Send trigger messages for show control and automation
- ✅ **IP Programming**: Remote network configuration of ArtNet nodes
- ✅ **Packet Parsing**: Parse and handle all standard ArtNet packet types

### Supported ArtNet Packets
- `ArtPoll` / `ArtPollReply` - Device discovery
- `ArtDmx` - DMX data transmission
- `ArtSync` - Frame synchronization
- `ArtTrigger` - Trigger/macro messages
- `ArtAddress` - Node configuration
- `ArtInput` - Input port configuration
- `ArtTodRequest` / `ArtTodData` / `ArtTodControl` - RDM table of devices
- `ArtRdm` / `ArtRdmSub` - RDM messages
- `ArtIpProg` / `ArtIpProgReply` - IP configuration

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Haukcode.ArtNet
```

For RDM support, also install:

```bash
dotnet add package Haukcode.ArtNet.Rdm
```

NuGet Package: https://www.nuget.org/packages/Haukcode.ArtNet

## Quick Start

### Basic DMX Transmission

```csharp
using System.Net;
using Haukcode.ArtNet;

// Get local network interface
var localIp = IPAddress.Parse("192.168.1.100");
var subnetMask = IPAddress.Parse("255.255.255.0");

// Create ArtNet client
using var client = new ArtNetClient(localIp, subnetMask);

// Prepare DMX data (512 channels)
var dmxData = new byte[512];
dmxData[0] = 255;  // Channel 1 at full brightness
dmxData[1] = 128;  // Channel 2 at 50%
dmxData[2] = 0;    // Channel 3 off

// Send to universe 1 (broadcast)
await client.SendDmxData(
    address: null,           // null = broadcast
    universeId: 1,
    dmxData: dmxData);
```

### Receiving DMX Data

```csharp
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using Haukcode.ArtNet;
using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Internal;

var localIp = IPAddress.Parse("192.168.1.100");
var subnetMask = IPAddress.Parse("255.255.255.0");

// Create a channel to receive packets
var channel = Channel.CreateUnbounded<ReceiveDataPacket>();

// Create client with packet handler
using var client = new ArtNetClient(
    localAddress: localIp,
    localSubnetMask: subnetMask,
    channelWriter: async (packet) => await channel.Writer.WriteAsync(packet),
    channelWriterComplete: () => channel.Writer.Complete());

// Process incoming packets
await foreach (var receivedPacket in channel.Reader.ReadAllAsync())
{
    if (receivedPacket.Packet is ArtNetDmxPacket dmxPacket)
    {
        Console.WriteLine($"Received DMX on Universe {dmxPacket.Universe + 1}");
        Console.WriteLine($"Channel 1: {dmxPacket.DmxData[0]}");
    }
}
```

### Sending ArtSync for Synchronized Output

```csharp
using var client = new ArtNetClient(localIp, subnetMask);

// Send DMX data to multiple universes
await client.SendDmxData(null, 1, dmxDataUniverse1);
await client.SendDmxData(null, 2, dmxDataUniverse2);
await client.SendDmxData(null, 3, dmxDataUniverse3);

// Send sync to apply all changes simultaneously
await client.SendSync(null);
```

### Device Discovery with ArtPoll

```csharp
using var client = new ArtNetClient(localIp, subnetMask);

// Send ArtPoll to discover devices
await client.QueuePacketForSending(
    destination: null,  // broadcast
    packet: new ArtPollPacket());

// Handle ArtPollReply packets in your packet handler
// (see "Receiving DMX Data" example for packet handling setup)
```

### Sending ArtTrigger Messages

```csharp
using Haukcode.ArtNet.Packets;

using var client = new ArtNetClient(localIp, subnetMask);

// Send a trigger command
var triggerPacket = new ArtTriggerPacket
{
    OemCode = 0x6A6B,     // Your OEM code
    Key = 0x03,            // Command: Select show
    SubKey = 1,            // Show ID
    Data = new byte[512]   // Optional payload
};

await client.QueuePacketForSending(null, triggerPacket);
```

### RDM - Remote Device Management

```csharp
using Haukcode.ArtNet;
using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.Packets.Product;

// First, discover RDM devices by sending ArtTodRequest
// Then send RDM commands to discovered devices

var deviceUid = new UId(0x1234, 0x56789ABC);  // Device UID from TOD
var universeId = 1;
var deviceIp = IPAddress.Parse("192.168.1.50");

// Get device label
var getLabel = new DeviceLabel.Get();
await client.SendRdm(getLabel, deviceIp, (short)universeId, deviceUid);

// Handle RDM responses in your packet handler
// Parse RdmPacket from ArtRdmPacket.RdmData
```

## Usage Examples

The library includes comprehensive samples demonstrating various features:

### Sample Projects

The `Samples` directory contains working examples:

1. **ArtTrigger Sample** - Demonstrates sending ArtTrigger packets for show control
2. **RDM Sample** - Shows device discovery and RDM parameter queries

To run the samples:

```bash
cd Samples
dotnet run
```

### Common Use Cases

#### 1. Simple Lighting Controller

```csharp
public class LightingController : IDisposable
{
    private readonly ArtNetClient client;
    
    public LightingController(IPAddress localIp, IPAddress subnetMask)
    {
        client = new ArtNetClient(localIp, subnetMask);
    }
    
    public async Task SetChannel(ushort universe, int channel, byte value)
    {
        var dmxData = new byte[512];
        dmxData[channel - 1] = value;  // DMX channels are 1-indexed
        
        await client.SendDmxData(null, universe, dmxData);
    }
    
    public void Dispose() => client.Dispose();
}
```

#### 2. DMX Monitor/Recorder

```csharp
public class DmxMonitor
{
    private readonly Dictionary<int, byte[]> universeData = new();
    
    public async Task StartMonitoring(IPAddress localIp, IPAddress subnetMask)
    {
        var channel = Channel.CreateUnbounded<ReceiveDataPacket>();
        
        using var client = new ArtNetClient(
            localIp, subnetMask,
            async (p) => await channel.Writer.WriteAsync(p),
            () => channel.Writer.Complete());
        
        await foreach (var packet in channel.Reader.ReadAllAsync())
        {
            if (packet.Packet is ArtNetDmxPacket dmx)
            {
                int universe = dmx.Universe + 1;
                universeData[universe] = dmx.DmxData.ToArray();
                
                Console.WriteLine($"Universe {universe}: " +
                    $"Seq {dmx.Sequence}, Channels: {dmx.DmxData.Length}");
            }
        }
    }
}
```

## API Reference

### ArtNetClient

Main class for ArtNet communication.

#### Constructor

```csharp
public ArtNetClient(
    IPAddress localAddress,
    IPAddress localSubnetMask,
    Func<ReceiveDataPacket, Task>? channelWriter = null,
    Action? channelWriterComplete = null,
    int port = 6454,
    UId? rdmId = null)
```

**Parameters:**
- `localAddress`: Local IP address to bind to
- `localSubnetMask`: Subnet mask for broadcast address calculation
- `channelWriter`: Optional callback for receiving packets
- `channelWriterComplete`: Optional callback when receive channel closes
- `port`: ArtNet port (default: 6454)
- `rdmId`: RDM UID for this client (optional)

#### Key Methods

```csharp
// Send DMX data
Task SendDmxData(
    IPAddress? address, 
    ushort universeId, 
    ReadOnlyMemory<byte> dmxData, 
    bool important = false)

// Send sync packet
Task SendSync(IPAddress? address)

// Send any ArtNet packet
Task QueuePacketForSending(
    IPAddress? destination, 
    ArtNetPacket packet, 
    bool important = false)

// Send RDM packet (requires Haukcode.ArtNet.Rdm)
Task SendRdm(
    RdmPacket packet,
    IPEndPoint destination,
    short universe,
    UId targetId)
```

#### Properties

```csharp
IPEndPoint LocalEndPoint { get; }      // Local endpoint
IPAddress BroadcastAddress { get; }    // Broadcast address
UId RdmId { get; }                     // RDM UID
```

## Performance Considerations

- The library uses high-performance UDP communication via the `Haukcode.HighPerfComm` library
- Large receive buffers (configurable) to handle high packet rates
- Efficient memory allocation and buffer management
- Supports high-frequency DMX updates (up to 44 Hz per universe)
- Sequence numbering to detect packet loss

## Thread Safety

- `ArtNetClient` is thread-safe for sending operations
- Received packets are delivered via async callbacks
- Multiple clients can coexist on the same machine with different ports

## Platform Support

- **.NET 8.0** and above
- **.NET 9.0** and above  
- **.NET 10.0** and above
- **Windows**, **Linux**, and **macOS** (cross-platform)

## Project Structure

```
src/
├── ArtNet/              # Core ArtNet protocol implementation
│   ├── Packets/         # All ArtNet packet types
│   ├── IO/              # Binary readers/writers
│   └── Internal/        # Internal implementation details
├── Rdm/                 # RDM (Remote Device Management) implementation
│   └── Packets/         # RDM packet types
└── Package/             # NuGet package configuration

Samples/                 # Example applications
```

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

### Development Setup

1. Clone the repository
2. Install .NET 8.0 SDK or later
3. Open `Haukcode-ArtNet.sln` in Visual Studio or your preferred IDE
4. Build and run tests

## License

This library is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

Copyright (c) 2018-2024 Hakan Lindestaf

## Credits

Based on the abandoned [Architecture for Control Networks (ACN)](http://acn.codeplex.com) project.

## Related Resources

- [Art-Net Protocol Specification](https://www.artisticlicence.com/WebSiteMaster/User%20Guides/art-net.pdf) - Official specification from Artistic Licence
- [DMX512 Standard](https://www.usitt.org/dmx512) - USITT DMX512-A standard
- [RDM Standard](https://www.estechnical.org/) - ANSI E1.20 RDM specification

## Support

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/HakanL/Haukcode.ArtNet/issues)
- **NuGet**: https://www.nuget.org/packages/Haukcode.ArtNet
- **Source**: https://github.com/HakanL/Haukcode.ArtNet
