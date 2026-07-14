using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet;

public class ArtNetClient : HighPerfComm.Client<ArtNetClient.SendData, Internal.ReceiveDataPacket>
{
    public class SendData : HighPerfComm.SendData
    {
        public IPEndPoint? Destination { get; set; }

        /// <summary>
        /// The destination pre-serialized once. Socket.SendTo(..., EndPoint) re-serializes the
        /// EndPoint into a fresh SocketAddress on every call; handing it an already-serialized one
        /// removes that work and the last per-packet allocations.
        /// </summary>
        public SocketAddress? DestinationAddress { get; set; }

        public SendData(IPEndPoint destination)
        {
            Destination = destination;
            DestinationAddress = destination.Serialize();
        }
    }

    public const int DefaultPort = 6454;
    public const int ReceiveBufferSize = 680 * 20 * 200;
    private const int SendBufferSize = 680 * 20 * 200;
    private static readonly IPEndPoint _blankEndpoint = new(IPAddress.Any, 0);

    private Socket? listenSocket;

    // One send socket per sender shard. Several threads sharing one UDP socket serialize on the
    // kernel's socket lock, which gives back most of the gain from sharding.
    private readonly Socket[] sendSockets;

    private readonly IPEndPoint localEndPoint;
    private readonly IPEndPoint broadcastEndPoint;
    private readonly Dictionary<ushort, byte> sequenceIds = [];
    private readonly object lockObject = new();
    private readonly Dictionary<IPAddress, IPEndPoint> endPointCache = [];

    // Serialized destinations, so the hot path never re-serializes an IPEndPoint. Only touched from
    // the single queue-writer thread (the send-data factory).
    private readonly Dictionary<IPEndPoint, SocketAddress> socketAddressCache = [];

    /// <param name="senderCount">
    /// Number of sender threads/sockets, sharded by universe id. Art-Net is per-universe just like
    /// sACN — 600 universes is 600 packets a frame whether they are unicast, broadcast or
    /// multicast — and a single sender thread saturates a core at roughly 24,000 packets/sec.
    /// Default 1 = the original behavior.
    /// </param>
    public ArtNetClient(
        IPAddress localAddress,
        IPAddress localSubnetMask,
        Func<Internal.ReceiveDataPacket, Task>? channelWriter = null,
        Action? channelWriterComplete = null,
        int port = DefaultPort,
        UId? rdmId = null,
        int senderCount = 1)
        : base(ArtNetPacket.MAX_PACKET_SIZE, channelWriter, channelWriterComplete, senderCount)
    {
        RdmId = rdmId ?? UId.Empty;

        this.localEndPoint = new IPEndPoint(localAddress, port);
        this.broadcastEndPoint =
            new IPEndPoint(Haukcode.Network.Utils.GetBroadcastAddress(localAddress, localSubnetMask), port);

        this.sendSockets = new Socket[SenderCount];
        for (int i = 0; i < SenderCount; i++)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SendBufferSize = SendBufferSize;

            Haukcode.Network.Utils.SetSocketOptions(socket);

            socket.DontFragment = true;
            socket.EnableBroadcast = true;

            // Bind to the local interface (ephemeral port)
            socket.Bind(new IPEndPoint(localAddress, 0));

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            this.sendSockets[i] = socket;
        }

        StartReceive();
    }

    /// <summary>
    /// Serialized form of a destination, cached. Called from the send-data factory on the single
    /// queue-writer thread.
    /// </summary>
    private SocketAddress GetSocketAddress(IPEndPoint endPoint)
    {
        if (!this.socketAddressCache.TryGetValue(endPoint, out var socketAddress))
        {
            socketAddress = endPoint.Serialize();
            this.socketAddressCache.Add(endPoint, socketAddress);
        }

        return socketAddress;
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

        return QueuePacketForSending(address, packet, important, shardKey: universeId);
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

        return QueueSyncPacketForSending(ResolveDestination(address), packet);
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
    public Task QueuePacketForSending(IPAddress? destination, ArtNetPacket packet, bool important = false, int shardKey = 0)
    {
        return QueuePacketForSending(ResolveDestination(destination), packet, important, shardKey);
    }

    private IPEndPoint ResolveDestination(IPAddress? destination)
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

        return sendDataDestination ?? this.broadcastEndPoint;
    }

    /// <summary>
    /// Send packet
    /// </summary>
    /// <param name="destination">Destination</param>
    /// <param name="packet">Packet</param>
    /// <param name="important">Important</param>
    public async Task QueuePacketForSending(IPEndPoint destination, ArtNetPacket packet, bool important = false, int shardKey = 0)
    {
        await QueuePacket(
            allocatePacketLength: packet.PacketLength,
            important: important,
            sendDataFactory: CreateSendData(destination),
            packetWriter: packet.WriteToBuffer,
            // Shard by universe: every packet for a universe goes out on the same thread and
            // socket, so its Art-Net sequence numbers stay ordered on the wire.
            shardKey: shardKey);
    }

    /// <summary>
    /// Queue an ArtSync. It must follow every DMX frame it synchronizes, so with more than one
    /// sender shard it goes out as an ordering barrier — otherwise it could be transmitted while a
    /// slower shard still had that frame's DMX pending, silently breaking synchronization.
    /// </summary>
    private async Task QueueSyncPacketForSending(IPEndPoint destination, ArtNetPacket packet)
    {
        await QueueBarrierPacket(
            allocatePacketLength: packet.PacketLength,
            sendDataFactory: CreateSendData(destination),
            packetWriter: packet.WriteToBuffer);
    }

    private Func<SendData> CreateSendData(IPEndPoint destination)
    {
        return () =>
        {
            // Reuse a spent send-data object returned by the sender instead of allocating a
            // new one for every queued packet. Every field is rewritten before use.
            var pooledSendData = RentSendData();
            if (pooledSendData != null)
            {
                pooledSendData.Destination = destination;
                pooledSendData.DestinationAddress = GetSocketAddress(destination);

                return pooledSendData;
            }

            // Pool empty (startup only) — the constructor serializes the destination itself.
            return new SendData(destination);
        };
    }

    /// <summary>
    /// Send packet immediately, bypassing the send queue
    /// </summary>
    /// <param name="destination">Destination</param>
    /// <param name="packet">Packet</param>
    /// <param name="important">Important</param>
    public Task SendPacketImmediately(IPAddress? destination, ArtNetPacket packet, bool important = false)
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

        return SendPacketImmediately(sendDataDestination ?? this.broadcastEndPoint, packet, important);
    }

    public async Task SendPacketImmediately(IPEndPoint destination, ArtNetPacket packet, bool important = false)
    {
        await SendImmediateAsync(
            allocatePacketLength: packet.PacketLength,
            important: important,
            sendDataFactory: () => new SendData(destination),
            packetWriter: packet.WriteToBuffer);

    }

    // Single sender shard (the base class default), so senderIndex is always 0. Art-Net is
    // unicast/broadcast rather than a multicast group per universe, so it has none of the
    // per-universe fan-out that makes sharding pay off for sACN.
    protected override int SendPacket(SendData sendData, ReadOnlyMemory<byte> payload, int senderIndex)
    {
        // SendTo(..., SocketAddress) with a pre-serialized destination: the EndPoint overload
        // re-serializes into a fresh SocketAddress on every call.
        return this.sendSockets[senderIndex].SendTo(payload.Span, SocketFlags.None, sendData.DestinationAddress!);
    }

    /// <summary>
    /// Send a packet from the receive (listen) socket, so the datagram's source
    /// port is the Art-Net port rather than the ephemeral send-socket port.
    /// Required for vendor request/reply exchanges where the responder replies
    /// to the request's source port instead of a fixed :6454 (e.g. the DMXking
    /// LeDMX settings opcode) — otherwise the reply lands on the ephemeral
    /// socket and is never received. Normal output keeps using the send socket.
    /// </summary>
    public async Task SendFromListenSocketAsync(IPEndPoint destination, ArtNetPacket packet)
    {
        var socket = this.listenSocket;
        if (socket == null)
            return;

        var buffer = new byte[packet.PacketLength];
        int length = packet.WriteToBuffer(buffer);

        await socket.SendToAsync(new ArraySegment<byte>(buffer, 0, length), SocketFlags.None, destination);
    }

    protected override int ReceiveData(Memory<byte> memory, out IPEndPoint? remoteEndPoint, out IPAddress? destinationAddress)
    {
        if (!System.Runtime.InteropServices.MemoryMarshal.TryGetArray<byte>(memory, out var segment))
            throw new InvalidOperationException("Expected an array-backed receive buffer");

        var socketFlags = SocketFlags.None;
        EndPoint endPoint = _blankEndpoint;
        int receivedBytes = this.listenSocket!.ReceiveMessageFrom(segment.Array!, segment.Offset, segment.Count, ref socketFlags, ref endPoint, out IPPacketInformation packetInformation);

        remoteEndPoint = endPoint as IPEndPoint;
        destinationAddress = packetInformation.Address;

        return receivedBytes;
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
            foreach (var socket in this.sendSockets)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }

                socket.Close();
                socket.Dispose();
            }
        }
    }
}
