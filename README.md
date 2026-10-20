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
- ✅ **Broadcast & Unicast**: Unicast DMX to nodes; broadcast where Art-Net 4 allows it (ArtPoll, ArtSync, ArtTrigger, ...)
- ✅ **Frame Synchronization**: ArtSync support for synchronized output

### Advanced Features
- ✅ **RDM Support**: Full Remote Device Management (separate Haukcode.ArtNet.Rdm package)
  - Device discovery through the node (TOD - Table of Devices)
  - Typed request and response classes for every standard PID
  - `RequestAsync`: request/response matching with ACK_OVERFLOW and ACK_TIMER follow-ups
  - UTF-8 text when the controller allows Unicode
  - Direct-line discovery (DISC_UNIQUE_BRANCH binary search) and ACK_OVERFLOW response writing
- ✅ **ArtTrigger**: Send trigger messages for show control and automation
- ✅ **IP Programming**: Remote network configuration of ArtNet nodes
- ✅ **Packet Parsing**: Parse and handle all standard ArtNet packet types

### Supported ArtNet Packets
- `ArtPoll` / `ArtPollReply` - Device discovery, including targeted polls
- `ArtDmx` - DMX data transmission
- `ArtNzs` / `ArtVlc` - Non-zero start code DMX and Visible Light Communication
- `ArtSync` - Frame synchronization
- `ArtTrigger` - Trigger/macro messages
- `ArtTimeCode` - SMPTE timecode (24/25/29.97/30 fps)
- `ArtCommand` - Text commands
- `ArtDataRequest` / `ArtDataReply` - Product and support URLs
- `ArtDiagData` - Diagnostics
- `ArtAddress` - Node configuration
- `ArtInput` - Input port configuration
- `ArtTodRequest` / `ArtTodData` / `ArtTodControl` - RDM table of devices
- `ArtRdm` / `ArtRdmSub` - RDM messages
- `ArtIpProg` / `ArtIpProgReply` - IP configuration
- `ArtFirmwareMaster` / `ArtFirmwareReply` - Firmware upload

## Standards Compliance

The packet layouts, field types and value tables were checked against the published documents,
and the client follows their transport rules. Tests check packet sizes, field offsets and PDLs against them.

| Standard | Coverage |
|---|---|
| **Art-Net 4** (rev 1.4dp, 2025) | Every packet the specification defines a layout for. ArtDmx, ArtNzs, ArtPollReply, ArtRdm, ArtTodData, ArtDataRequest/Reply and ArtFirmwareMaster/Reply are never broadcast, and packets go out from port 6454 (with the default single sender). Not implemented: the opcodes the specification lists without a layout (ArtMedia, ArtTimeSync, ArtDirectory, ArtFile*) and the deprecated ArtVideo and ArtMac packets. |
| **ANSI E1.20-2025** (RDM) | Every PID, response type (including ACK_TIMER_HI_RES), NACK reason, controller flag, sensor type and unit. ACK_OVERFLOW reading and writing, ACK_TIMER follow-up through QUEUED_MESSAGE, UTF-8 text, the DISC_UNIQUE_BRANCH response encoding and discovery. |
| **ANSI E1.37-1** (dimmer and additional PIDs) | Every PID. |
| **ANSI E1.37-2** (IPv4 and DNS configuration) | Every PID. |
| **ANSI E1.37-7** (gateway and splitter configuration) | Every PID, with the published values (not the draft ones). |
| **ANSI E1.33** (RDMnet) | The RDM PIDs only (component scope, search domain, broker status, TCP comms status). The RDMnet transport itself (LLRP, broker and EPT protocols) is not implemented. |

Checked on hardware against a DMXking LeDMX2 MAX: discovery, IP query, RDM table and RDM
requests to an attached fixture, name programming and unicast DMX output.
Found something that doesn't match the specification? Please [open an issue](https://github.com/HakanL/Haukcode.ArtNet/issues).

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

// Send to universe 1 on a node. Art-Net 4 doesn't allow DMX to be broadcast;
// set client.AllowDmxBroadcast = true if you really need to.
await client.SendDmxData(
    address: IPAddress.Parse("192.168.1.50"),
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
var node = IPAddress.Parse("192.168.1.50");
await client.SendDmxData(node, 1, dmxDataUniverse1);
await client.SendDmxData(node, 2, dmxDataUniverse2);
await client.SendDmxData(node, 3, dmxDataUniverse3);

// Send sync to apply all changes simultaneously (always broadcast)
await client.SendSync();
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
using Haukcode.ArtNet.Rdm.Packets.Management;

// First, discover RDM devices by sending ArtTodRequest
// Then send RDM commands to discovered devices

using var client = new ArtNetRdmClient(localAddress, subnetMask, rdmId: new UId(0x7FF0, 0x00000001));

var deviceUid = new UId(0x1234, 0x56789ABC);  // Device UID from TOD
ushort portAddress = 1;
var node = new IPEndPoint(IPAddress.Parse("192.168.1.50"), ArtNetClient.DefaultPort);

// Send a GET and wait for the matching response. ACK_OVERFLOW responses are collected and
// joined, and ACK_TIMER responses are fetched with QUEUED_MESSAGE once they're ready.
var response = await client.RequestAsync(new DeviceLabel.Get(), node, portAddress, deviceUid);
if (response is DeviceLabel.GetReply label)
    Console.WriteLine(label.Label);
else if (response is RdmNack nack)
    Console.WriteLine($"NACK: {nack.Reason}");

// Or fire and forget, and parse responses yourself from ArtRdmPacket.RdmData
await client.SendRdm(new DeviceLabel.Get(), node, portAddress, deviceUid);
```

For a controller that drives RDM on a DMX line directly (not through an Art-Net node),
`RdmDiscovery` runs the DISC_UNIQUE_BRANCH binary search over callbacks you supply for the
line I/O, and `RdmPacket.WriteResponseMessages` splits a responder's large response into an
ACK_OVERFLOW sequence.

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

### ArtTimeCode Generator (CLI)

`src/TimeCodeGenerator` is a console tool for testing Art-Net timecode receivers. It sends one `ArtTimeCode` packet (OpTimeCode `0x9700`, UDP port 6454) per frame, like a lighting console or media server with timecode output would. All four Art-Net frame-rate types are supported, with correct SMPTE 29.97 drop-frame counting (frames 00 and 01 skipped at the start of every minute except every tenth).

#### Running it

Requires the .NET 10 SDK. From the repository root:

```bash
dotnet run --project src/TimeCodeGenerator -- --fps 25 --start 00:59:50:00
```

Or build a standalone executable once and run it from anywhere:

```bash
dotnet publish src/TimeCodeGenerator -c Release -o ./tcgen
./tcgen/ArtNetTimeCodeGenerator --fps 30 --start 01:00:00:00
```

With no options it plays 30 fps from `00:00:00:00`, StreamId 0, broadcast on the first Ethernet or Wi-Fi adapter. The header shows which interface and destination were picked, and a status line updates four times a second with the current timecode, transport state, packet count and scheduling accuracy. Press `Q` or Ctrl+C to exit; a summary of packets sent and timing lateness is printed.

#### Options

| Option | Default | Description |
|---|---|---|
| `-f`, `--fps <rate>` | `30` | `24`, `25`, `29.97` or `30` (aliases `film`, `ebu`, `df`, `smpte`) |
| `-s`, `--start <timecode>` | `00:00:00:00` | Start value as `HH:MM:SS:FF`. Seconds and frames may be omitted. Use `;` before the frames for drop-frame if you like, e.g. `01:00:00;02` |
| `--stream <id>` | `0` | ArtTimeCode StreamId, 0-255. Receivers usually listen to stream 0 |
| `-d`, `--destination <ip>` | subnet broadcast | Unicast to one node instead of broadcasting |
| `-l`, `--local <ip>` | first adapter | Local interface to send from, when the machine has several |
| `-p`, `--port <port>` | `6454` | UDP port |
| `-t`, `--duration <sec>` | run until quit | Exit after this many seconds, for scripted runs |
| `--stopped` | | Start parked on the start value; press `Space` to play |
| `--hold` | | Start repeating the first frame (frozen clock, still sending) |
| `--script <steps>` | | Non-interactive transport. Semicolon-separated `play`/`pause`/`hold`/`wait`/`stop`/`seek`. Durations: `14s`, `1.5`, `500ms`. Unbounded `play`/`pause`/`hold` (no duration) only as the last step |
| `--jitter <ms>` | `0` | Randomize each frame's send moment by up to plus/minus this many ms |
| `--drop <percent>` | `0` | Randomly skip this percentage of frames |
| `--list-interfaces` | | Print the usable local adapters with their broadcast addresses and exit |
| `-q`, `--quiet` | | No live status line (the summary is still printed) |
| `-h`, `--help` | | Full usage text |

#### Transport keys

The tool behaves like a hardware timecode generator with a transport bar:

| Key | Action | What the receiver sees |
|---|---|---|
| `Space` or `P` | Play / pause | Pause stops sending completely and freezes the clock. Play continues from the same frame. Use it to test dropout, freewheel and re-sync handling |
| `S` | Stop | Pause and rewind to the start value |
| `R` or `Home` | Rewind | Jump back to the start value; keeps playing if playing |
| `L` | Locate | Prompts for a timecode, then continues from there. Tests jumps and re-locate |
| `C` | Clock | Jumps to the system time of day |
| `H` | Hold | Keeps sending, but repeats the same frame. Tests "frozen clock" detection, which is different from a dropout |
| `+` / `-` | Jump 10 s forward / back | Discontinuity while still running |
| `.` / `,` | Step one frame forward / back | Fine offset checks |
| `1` `2` `3` `4` | Switch to 24 / 25 / 29.97 df / 30 fps | Rate change in the middle of a stream, time of day carried over |
| `Q` or `Esc` | Quit | |

#### Typical test scenarios

```bash
# Pre-roll: a timeline that starts at 01:00:00:00 should arm, then play exactly at the hour
dotnet run --project src/TimeCodeGenerator -- --fps 25 --start 00:59:50:00 --stopped
# press Space when the receiver is ready

# Drop-frame across a minute boundary (59;29 -> 00;02)
dotnet run --project src/TimeCodeGenerator -- --fps 29.97 --start 00:00:55;00

# Second stream, unicast to one node
dotnet run --project src/TimeCodeGenerator -- --stream 1 --destination 192.168.1.50

# Bad network: 5 ms jitter and 2% loss for a minute, no interaction
dotnet run --project src/TimeCodeGenerator -- --jitter 5 --drop 2 --duration 60 --quiet

# Pause dropout then locate (no keyboard): play, silence, jump, play
dotnet run --project src/TimeCodeGenerator -- --fps 24 --start 00:59:50:00 --script "play 14s; pause 2s; seek 01:00:08:00; play 10s"

# Frozen-frame hold (packets keep going, value stuck), then locate
dotnet run --project src/TimeCodeGenerator -- --fps 24 --start 00:59:50:00 --script "play 12s; hold 2s; seek 01:00:08:00; play 6s"
```

#### Things to know

- **Loopback is not enough.** Most receivers bind to a real adapter, so packets sent to `127.0.0.1` never arrive. The default broadcast, or unicast to the receiver's own IP, reaches a receiver on the same machine.
- **Same machine, same port.** If the receiver is already listening on 6454 it owns the port; the generator only sends, so that is fine. A separate sniffer on the same machine will not see the packets, use `--port` for that kind of check.
- **Timing.** Frames are scheduled against a fixed anchor, so there is no drift, and if the process stalls the clock skips ahead instead of bursting. On Windows the tool raises the timer resolution to 1 ms while running; scheduling lateness is normally well under 1 ms and is reported at exit.
- **Pause vs hold.** Pause is silence (a dropout). Hold is a stream whose value never changes. Receivers typically treat these differently, so both are available.

#### Reusing the engine in an application

The pacing engine (`ArtTimeCodeGenerator` in `src/TimeCodeGenerator`) has no network dependency: it hands one `ArtTimeCodePacket` per frame to a callback from its own thread. To generate timecode from an application that already has an `ArtNetClient`, wire it to the client:

```csharp
var start = TimeCode.Parse("01:00:00:00", ArtTimeCodeTypes.Smpte);

using var generator = new ArtTimeCodeGenerator(start, streamId: 0,
    packet => client.SendPacketImmediately((IPAddress?)null, packet));

generator.Start();
// generator.Paused = true / false, generator.Seek(...), generator.Hold = true, ...
```

### Common Use Cases

#### 1. Simple Lighting Controller

```csharp
public class LightingController : IDisposable
{
    private readonly ArtNetClient client;
    private readonly IPAddress node;

    public LightingController(IPAddress localIp, IPAddress subnetMask, IPAddress node)
    {
        client = new ArtNetClient(localIp, subnetMask);
        this.node = node;
    }

    public async Task SetChannel(ushort universe, int channel, byte value)
    {
        var dmxData = new byte[512];
        dmxData[channel - 1] = value;  // DMX channels are 1-indexed

        await client.SendDmxData(node, universe, dmxData);
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
    UId? rdmId = null,
    int senderCount = 1)
```

**Parameters:**
- `localAddress`: Local IP address to bind to
- `localSubnetMask`: Subnet mask for broadcast address calculation
- `channelWriter`: Optional callback for receiving packets
- `channelWriterComplete`: Optional callback when receive channel closes
- `port`: ArtNet port (default: 6454)
- `rdmId`: RDM UID for this client (optional)
- `senderCount`: Sender threads/sockets. With 1 (the default) packets go out from port 6454 as Art-Net requires; more is faster but sends from ephemeral ports

#### Key Methods

```csharp
// Send DMX data (address must be a node unless AllowDmxBroadcast is set)
Task SendDmxData(
    IPAddress? address,
    ushort universeId,
    ReadOnlyMemory<byte> dmxData,
    bool important = false)

// Send sync packet (broadcast)
Task SendSync()

// Send any ArtNet packet
Task QueuePacketForSending(
    IPAddress? destination,
    ArtNetPacket packet,
    bool important = false)

// Send RDM packet (ArtNetRdmClient, requires Haukcode.ArtNet.Rdm)
Task SendRdm(
    RdmPacket packet,
    IPEndPoint destination,
    ushort portAddress,
    UId targetId)

// Send an RDM GET/SET and wait for the matching response (ArtNetRdmClient)
Task<RdmPacket> RequestAsync(
    RdmRequestPacket packet,
    IPEndPoint destination,
    ushort portAddress,
    UId targetId,
    TimeSpan? responseTimeout = null,
    CancellationToken cancellationToken = default)
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

- The `ArtNetClient` send methods reuse buffers on the hot path, so call them from one thread at a time
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
├── ArtNet.Tests/        # Unit tests
├── TimeCodeGenerator/   # ArtTimeCode generator CLI for testing receivers
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
- [ESTA published standards](https://tsp.esta.org/tsp/documents/published_docs.php) - ANSI E1.20 (RDM), E1.37 and E1.33, free to download

## Support

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/HakanL/Haukcode.ArtNet/issues)
- **NuGet**: https://www.nuget.org/packages/Haukcode.ArtNet
- **Source**: https://github.com/HakanL/Haukcode.ArtNet
