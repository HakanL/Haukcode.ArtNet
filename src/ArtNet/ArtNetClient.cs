using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet;

public class ArtNetClient : HighPerfComm.Client<ArtNetClient.SendData, Internal.ReceiveDataPacket>
{
    public class SendData : HighPerfComm.SendData
    {
        public IPEndPoint? Destination { get; set; }

        public SendData(IPEndPoint destination)
        {
            Destination = destination;
        }
    }

    public const int DefaultPort = 6454;
    public const int ReceiveBufferSize = 680 * 20 * 200;
    private const int SendBufferSize = 680 * 20 * 200;
    private static readonly IPEndPoint _blankEndpoint = new(IPAddress.Any, 0);

    private Socket? listenSocket;
    private readonly Socket sendSocket;
    private readonly IPEndPoint localEndPoint;
    private readonly IPEndPoint broadcastEndPoint;
    private readonly Dictionary<ushort, byte> sequenceIds = [];
    private readonly object lockObject = new();
    private readonly Dictionary<IPAddress, IPEndPoint> endPointCache = [];

    public ArtNetClient(
        IPAddress localAddress,
        IPAddress localSubnetMask,
        Func<Internal.ReceiveDataPacket, Task>? channelWriter = null,
        Action? channelWriterComplete = null,
        int port = DefaultPort,
        UId? rdmId = null)
        : base(ArtNetPacket.MAX_PACKET_SIZE, channelWriter, channelWriterComplete)
    {
        RdmId = rdmId ?? UId.Empty;

        this.localEndPoint = new IPEndPoint(localAddress, port);
        this.broadcastEndPoint =
            new IPEndPoint(Haukcode.Network.Utils.GetBroadcastAddress(localAddress, localSubnetMask), port);

        this.sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        this.sendSocket.SendBufferSize = SendBufferSize;

        Haukcode.Network.Utils.SetSocketOptions(this.sendSocket);

        this.sendSocket.DontFragment = true;
        this.sendSocket.EnableBroadcast = true;

        // Bind to the local interface
        this.sendSocket.Bind(new IPEndPoint(localAddress, 0));

        this.sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        StartReceive();
    }

    /// <summary>
    /// Gets or sets the RDM Id to use when sending packets.
    /// </summary>
    public UId RdmId { get; protected set; }

    public IPEndPoint LocalEndPoint => this.localEndPoint;

    public IPAddress BroadcastAddress => this.broadcastEndPoint.Address;



    /// <summary>
    /// Send data
    /// </summary>
    /// <param name="address">The optional address to unicast to</param>
    /// <param name="universeId">The Universe ID</param>
    /// <param name="dmxData">Up to 512 bytes of DMX data</param>
    /// <param name="important">Important</param>
    public Task SendDmxData(
        IPAddress? address,
        ushort universeId,
        ReadOnlyMemory<byte> dmxData,
        bool important = false)
    {
        if (!IsOperational)
            return Task.CompletedTask;

        byte sequenceId = GetNewSequenceId(universeId);

        var packet = new ArtNetDmxPacket
        {
            DmxData = dmxData.ToArray(),
            Universe = (short)(universeId - 1),
            Sequence = sequenceId
        };

        return QueuePacketForSending(address, packet, important);
    }

    /// <summary>
    /// Send sync
    /// </summary>
    /// <param name="address">The optional address to unicast to</param>
    public Task SendSync(IPAddress? address)
    {
        if (!IsOperational)
            return Task.CompletedTask;

        var packet = new ArtSyncPacket();

        return QueuePacketForSending(address, packet, true);
    }

    private byte GetNewSequenceId(ushort universeId)
    {
        lock (this.lockObject)
        {
            this.sequenceIds.TryGetValue(universeId, out byte sequenceId);

            sequenceId++;

            this.sequenceIds[universeId] = sequenceId;

            return sequenceId;
        }
    }

    /// <summary>
    /// Send packet
    /// </summary>
    /// <param name="destination">Destination</param>
    /// <param name="packet">Packet</param>
    /// <param name="important">Important</param>
    public Task QueuePacketForSending(IPAddress? destination, ArtNetPacket packet, bool important = false)
    {
        IPEndPoint? sendDataDestination = null;

        if (destination != null)
        {
            if (!this.endPointCache.TryGetValue(destination, out var ipEndPoint))
            {
                ipEndPoint = new IPEndPoint(destination, this.localEndPoint.Port);
                this.endPointCache.Add(destination, ipEndPoint);
            }

            // Only works for when subnet mask is /24 or less
            if (ipEndPoint.Address.GetAddressBytes().Last() == 255)
                sendDataDestination = null;
            else
                sendDataDestination = ipEndPoint;
        }

        return QueuePacketForSending(sendDataDestination ?? this.broadcastEndPoint, packet, important);
    }

    /// <summary>
    /// Send packet
    /// </summary>
    /// <param name="destination">Destination</param>
    /// <param name="packet">Packet</param>
    /// <param name="important">Important</param>
    public async Task QueuePacketForSending(IPEndPoint destination, ArtNetPacket packet, bool important = false)
    {
        await base.QueuePacket(
            allocatePacketLength: packet.PacketLength,
            important: important,
            sendDataFactory: () => new SendData(destination),
            packetWriter: packet.WriteToBuffer);
    }

    protected override ValueTask<int> SendPacketAsync(SendData sendData, ReadOnlyMemory<byte> payload)
    {
        return this.sendSocket.SendToAsync(payload, SocketFlags.None, sendData.Destination!);
    }

    protected async override ValueTask<(int ReceivedBytes, SocketReceiveMessageFromResult Result)> ReceiveData(
        Memory<byte> memory, CancellationToken cancelToken)
    {
        var result =
            await this.listenSocket!.ReceiveMessageFromAsync(memory, SocketFlags.None, _blankEndpoint, cancelToken);

        return (result.ReceivedBytes, result);
    }

    public int? ActualReceiveBufferSize
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                // Linux reports the internal buffer size, which is double the requested size
                return this.listenSocket?.ReceiveBufferSize / 2;
            else
                return this.listenSocket?.ReceiveBufferSize;
        }
    }

    protected override void InitializeReceiveSocket()
    {
        this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        this.listenSocket.ReceiveBufferSize = ReceiveBufferSize;

        Haukcode.Network.Utils.SetSocketOptions(this.listenSocket);

        // Linux wants IPAddress.Any to get all types of packets (unicast/multicast/broadcast)
        this.listenSocket.Bind(new IPEndPoint(IPAddress.Any, this.localEndPoint.Port));
    }

    protected override void DisposeReceiveSocket()
    {
        try
        {
            this.listenSocket?.Shutdown(SocketShutdown.Both);
        }
        catch
        {
        }

        this.listenSocket?.Close();
        this.listenSocket?.Dispose();
        this.listenSocket = null;
    }

    protected override Internal.ReceiveDataPacket? TryParseObject(ReadOnlyMemory<byte> buffer, double timestampMS,
        IPEndPoint sourceIP, IPAddress destinationIP)
    {
        var packet = ArtNetPacket.Parse(buffer);

        // Note that we're still using the memory from the pipeline here, the packet is not allocating its own DMX data byte array
        if (packet != null)
        {
            // Protect against UDP loopback where we receive our own packets, except for poll/pollreply commands.
            if (!LocalEndPoint.Equals(sourceIP) ||
                packet.OpCode == ArtNetOpCodes.Poll ||
                packet.OpCode == ArtNetOpCodes.PollReply)
            {
                var parsedObject = new Internal.ReceiveDataPacket
                {
                    TimestampMS = timestampMS,
                    Source = sourceIP,
                    Packet = packet
                };

                if (!this.endPointCache.TryGetValue(destinationIP, out var ipEndPoint))
                {
                    ipEndPoint = new IPEndPoint(destinationIP, this.localEndPoint.Port);
                    this.endPointCache.Add(destinationIP, ipEndPoint);
                }

                parsedObject.Destination = ipEndPoint ?? this.broadcastEndPoint;

                return parsedObject;
            }
        }

        return null;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            try
            {
                this.sendSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            this.sendSocket.Close();
            this.sendSocket.Dispose();
        }
    }
}
