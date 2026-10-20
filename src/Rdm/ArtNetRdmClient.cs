using System.Collections.Concurrent;
using Haukcode.ArtNet.Rdm.Packets.Management;
using Haukcode.ArtNet.Rdm.Packets.Status;

namespace Haukcode.ArtNet.Rdm;

public class ArtNetRdmClient : ArtNetClient
{
    public ArtNetRdmClient(
        IPAddress localAddress,
        IPAddress localSubnetMask,
        Func<Haukcode.ArtNet.Internal.ReceiveDataPacket, Task>? channelWriter = null,
        Action? channelWriterComplete = null,
        int port = DefaultPort,
        UId? rdmId = null,
        int senderCount = 1)
        // The base client only parses received packets when there's a channel writer, and
        // RequestAsync needs them parsed to see its responses
        : base(localAddress, localSubnetMask, channelWriter ?? IgnorePacket, channelWriterComplete, port, rdmId, senderCount)
    {
    }

    private static Task IgnorePacket(Haukcode.ArtNet.Internal.ReceiveDataPacket packet) => Task.CompletedTask;

    // Offsets in RDM data as ArtRdm carries it (from the Message Length byte on)
    private const int SourceIdOffset = 7;
    private const int TransactionNumberOffset = 13;
    private const int ResponseTypeOffset = 14;
    private const int CommandOffset = 18;
    private const int ParameterIdOffset = 19;

    // Wait between polls of QUEUED_MESSAGE while the responder has nothing queued yet
    private static readonly TimeSpan QueuedMessagePollInterval = TimeSpan.FromMilliseconds(100);

    private int transactionNumber;

    // Requests sent by RequestAsync that are waiting for their response, by transaction number
    private readonly ConcurrentDictionary<byte, PendingRequest> pendingRequests = new();

    /// <summary>How long <see cref="RequestAsync"/> waits for each response unless told otherwise.</summary>
    public static readonly TimeSpan DefaultResponseTimeout = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Most follow-up requests <see cref="RequestAsync"/> sends for one request: repeats for
    /// ACK_OVERFLOW and polls of QUEUED_MESSAGE after ACK_TIMER.
    /// </summary>
    public const int MaxFollowUpRequests = 256;

    /// <param name="targetUniverse">The 15-bit Port-Address the device is on.</param>
    public Task SendRdm(RdmPacket packet, IPEndPoint targetAddress, ushort targetUniverse, UId targetId)
    {
        return SendRdm(packet, targetAddress, targetUniverse, targetId, RdmId);
    }

    /// <summary>Port-Addresses are 15 bits.</summary>
    public const ushort MaxPortAddress = 0x7FFF;

    /// <summary>The sub-device id that addresses all sub-devices.</summary>
    public const ushort AllSubDevices = 0xFFFF;

    /// <param name="targetUniverse">The 15-bit Port-Address the device is on.</param>
    /// <remarks>
    /// Every packet gets the next transaction number, starting at 0 (E1.20 6.2.6: "increment this
    /// field every time an RDM packet is transmitted"); responses echo it, so it identifies which
    /// request a response belongs to.
    /// </remarks>
    public Task SendRdm(RdmPacket packet, IPEndPoint targetAddress, ushort targetUniverse, UId targetId, UId sourceId)
    {
        return SendRdm(packet, targetAddress, targetUniverse, targetId, sourceId, NextTransactionNumber());
    }

    private byte NextTransactionNumber() => (byte)(Interlocked.Increment(ref this.transactionNumber) - 1);

    private Task SendRdm(RdmPacket packet, IPEndPoint targetAddress, ushort targetUniverse, UId targetId, UId sourceId, byte transactionNumber)
    {
        if (targetUniverse > MaxPortAddress)
            throw new ArgumentOutOfRangeException(nameof(targetUniverse), targetUniverse, $"The Port-Address must be 0 to {MaxPortAddress}.");

        //Fill in addition details
        packet.Header.SourceId = sourceId;
        packet.Header.DestinationId = targetId;
        packet.Header.TransactionNumber = transactionNumber;

        //Sub Devices
        if (targetId is SubDeviceUId subDevice)
            packet.Header.SubDevice = subDevice.SubDeviceId;

        // E1.20 9: "GET commands sent to the SUB_DEVICE_ALL_CALL Sub-Device ID are not allowed"
        if (packet.Header.Command == RdmCommands.Get && packet.Header.SubDevice == AllSubDevices)
            throw new InvalidOperationException("A GET can't be sent to all sub-devices (0xFFFF).");

        //Create Rdm Packet
        using (var rdmData = new MemoryStream())
        {
            // Text goes out as UTF-8 only when the request says the controller handles Unicode
            var rdmWriter = new RdmBinaryWriter(rdmData)
            {
                AllowUnicode = packet.Header.ControllerFlags.HasFlag(RdmControllerFlags.Unicode)
            };

            //Write the RDM packet
            RdmPacket.WritePacket(packet, rdmWriter);

            //Write the checksum
            rdmWriter.WriteUInt16(RdmPacket.CalculateMessageChecksum(rdmData.GetBuffer().AsSpan(0, (int)rdmData.Length)));

            //Create sACN Packet
            var rdmPacket = new ArtRdmPacket
            {
                Address = (byte)(targetUniverse & 0x00FF),
                Net = (byte)(targetUniverse >> 8),
                SubStartCode = (byte)RdmVersions.SubMessage,
                RdmData = rdmData.ToArray()
            };

            return QueuePacketForSending(targetAddress, rdmPacket);
        }
    }

    /// <summary>
    /// Sends a GET or SET and waits for its response, following up the way E1.20 6.3 describes:
    /// on ACK_OVERFLOW the request is repeated and the parts are joined into one packet, and on
    /// ACK_TIMER / ACK_TIMER_HI_RES the client waits the estimated time and then fetches the
    /// response with GET QUEUED_MESSAGE.
    /// </summary>
    /// <param name="targetUniverse">The 15-bit Port-Address the device is on.</param>
    /// <param name="responseTimeout">How long to wait for each response; <see cref="DefaultResponseTimeout"/> when null.</param>
    /// <returns>
    /// The response: the typed packet for an ACK, or <see cref="RdmNack"/> for a NACK.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A response is matched to its request by transaction number, by coming from the target
    /// device and being addressed to <see cref="ArtNetClient.RdmId"/>, and by having the request's
    /// PID and the response command class (GET_COMMAND_RESPONSE for a GET). Responses to
    /// QUEUED_MESSAGE carry the PID of the queued message, so other queued messages fetched while
    /// looking for this one are skipped; read them yourself if you need them.
    /// </para>
    /// <para>
    /// Don't run other requests to the same device while one is collecting ACK_OVERFLOW data: the
    /// responder restarts the transfer when it receives a different PID or command class.
    /// </para>
    /// </remarks>
    /// <exception cref="TimeoutException">A response didn't arrive in time.</exception>
    /// <exception cref="InvalidPacketException">The responses don't make up a valid response.</exception>
    public async Task<RdmPacket> RequestAsync(
        RdmRequestPacket packet,
        IPEndPoint targetAddress,
        ushort targetUniverse,
        UId targetId,
        TimeSpan? responseTimeout = null,
        CancellationToken cancellationToken = default)
    {
        if (packet.Header.Command != RdmCommands.Get && packet.Header.Command != RdmCommands.Set)
            throw new ArgumentException("Only GET and SET requests get a response.", nameof(packet));
        if (targetId.DeviceId == 0xFFFFFFFF)
            throw new ArgumentException("Broadcast requests get no response; send them with SendRdm.", nameof(targetId));

        var timeout = responseTimeout ?? DefaultResponseTimeout;
        var overflow = new List<ReadOnlyMemory<byte>>();

        // The request to repeat on ACK_OVERFLOW: the original, or QUEUED_MESSAGE once the
        // response is deferred
        RdmRequestPacket current = packet;
        bool queued = false;

        byte[] response = await ExchangeAsync(current, targetAddress, targetUniverse, targetId, matchParameter: true, timeout, cancellationToken);

        for (int followUps = 0; ; followUps++)
        {
            if (followUps > MaxFollowUpRequests)
                throw new InvalidPacketException($"No complete response after {MaxFollowUpRequests} follow-up requests.");

            switch ((RdmResponseTypes)response[ResponseTypeOffset])
            {
                case RdmResponseTypes.AckOverflow:
                    overflow.Add(response);
                    response = await ExchangeAsync(current, targetAddress, targetUniverse, targetId, matchParameter: !queued, timeout, cancellationToken);
                    break;

                case RdmResponseTypes.AckTimer:
                case RdmResponseTypes.AckTimerHiRes:
                    if (overflow.Count > 0)
                        throw new InvalidPacketException("ACK_TIMER in the middle of an ACK_OVERFLOW sequence.");

                    var timer = (RdmAckTimer)RdmPacket.ReadPacket(response);
                    await Task.Delay(timer.EstimatedResponseDelay, cancellationToken);

                    current = new QueuedMessage.Get { StatusType = StatusTypes.Error };
                    current.Header.ControllerFlags = packet.Header.ControllerFlags;
                    queued = true;
                    response = await GetQueuedResponseAsync(current, packet.Header, targetAddress, targetUniverse, targetId, timeout, cancellationToken);
                    break;

                case RdmResponseTypes.NackReason:
                    return RdmPacket.ReadPacket(response);

                default:
                    if (overflow.Count == 0)
                        return RdmPacket.ReadPacket(response);

                    overflow.Add(response);
                    return RdmPacket.ReadOverflowPacket(overflow);
            }

            // Parts of a queued ACK_OVERFLOW response come back through QUEUED_MESSAGE too, and so
            // can unrelated queued messages in between
            if (queued && !IsResponseTo(response, packet.Header))
            {
                response = await GetQueuedResponseAsync(current, packet.Header, targetAddress, targetUniverse, targetId, timeout, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Polls QUEUED_MESSAGE until the responder sends the deferred response to
    /// <paramref name="original"/>, skipping other queued messages and waiting while it answers
    /// with STATUS_MESSAGES (nothing queued yet).
    /// </summary>
    private async Task<byte[]> GetQueuedResponseAsync(RdmRequestPacket queuedMessage, RdmHeader original, IPEndPoint targetAddress,
        ushort targetUniverse, UId targetId, TimeSpan timeout, CancellationToken cancellationToken)
    {
        // E1.20 10.3.1: QUEUED_MESSAGE goes to the root device, which holds its sub-devices' messages
        var rootId = new UId(targetId.ManufacturerId, targetId.DeviceId);

        for (int poll = 0; poll < MaxFollowUpRequests; poll++)
        {
            var response = await ExchangeAsync(queuedMessage, targetAddress, targetUniverse, rootId, matchParameter: false, timeout, cancellationToken);

            if (IsResponseTo(response, original))
                return response;

            if ((RdmParameters)ReadUInt16(response, ParameterIdOffset) == RdmParameters.StatusMessage)
                await Task.Delay(QueuedMessagePollInterval, cancellationToken);
        }

        throw new InvalidPacketException($"The responder didn't queue the response to {original.ParameterId} within {MaxFollowUpRequests} polls.");
    }

    private static bool IsResponseTo(byte[] response, RdmHeader request) =>
        (RdmCommands)response[CommandOffset] == request.Command + 1
        && (RdmParameters)ReadUInt16(response, ParameterIdOffset) == request.ParameterId;

    private static ushort ReadUInt16(byte[] data, int offset) => (ushort)((data[offset] << 8) | data[offset + 1]);

    /// <summary>Sends one request and waits for the RDM data of its response.</summary>
    /// <param name="matchParameter">
    /// Whether the response must have the request's PID and command class; QUEUED_MESSAGE
    /// responses carry those of the queued message instead.
    /// </param>
    private async Task<byte[]> ExchangeAsync(RdmRequestPacket packet, IPEndPoint targetAddress, ushort targetUniverse, UId targetId,
        bool matchParameter, TimeSpan timeout, CancellationToken cancellationToken)
    {
        byte number = NextTransactionNumber();
        var pending = new PendingRequest(targetId, RdmId, matchParameter ? packet.Header.Command + 1 : null, matchParameter ? packet.Header.ParameterId : null);

        // Registered before sending so a fast response can't arrive first. Transaction numbers
        // wrap after 256 requests; a stale entry with the same number is replaced
        this.pendingRequests[number] = pending;
        try
        {
            await SendRdm(packet, targetAddress, targetUniverse, targetId, RdmId, number);

            return await pending.Response.Task.WaitAsync(timeout, cancellationToken);
        }
        catch (TimeoutException)
        {
            throw new TimeoutException($"No RDM response from {targetId} to {packet.Header.Command} {packet.Header.ParameterId} within {timeout.TotalMilliseconds} ms.");
        }
        finally
        {
            this.pendingRequests.TryRemove(new KeyValuePair<byte, PendingRequest>(number, pending));
        }
    }

    protected override void OnPacketReceived(ArtNetPacket packet, IPEndPoint source)
    {
        if (packet is not ArtRdmPacket artRdm || this.pendingRequests.IsEmpty)
            return;

        var data = artRdm.RdmData;
        if (data.Length < 24 || !RdmPacket.IsChecksumValid(data))
            return;

        if (this.pendingRequests.TryGetValue(data[TransactionNumberOffset], out var pending) && pending.Matches(data))
            pending.Response.TrySetResult(data);
    }

    private sealed class PendingRequest
    {
        private readonly UId target;
        private readonly UId controller;
        private readonly RdmCommands? responseCommand;
        private readonly RdmParameters? parameterId;

        public PendingRequest(UId target, UId controller, RdmCommands? responseCommand, RdmParameters? parameterId)
        {
            this.target = target;
            this.controller = controller;
            this.responseCommand = responseCommand;
            this.parameterId = parameterId;
        }

        // Completed on the receive thread; continuations must not run there
        public TaskCompletionSource<byte[]> Response { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool Matches(byte[] data)
        {
            var command = (RdmCommands)data[CommandOffset];
            if (command != RdmCommands.GetResponse && command != RdmCommands.SetResponse)
                return false;
            if (this.responseCommand != null && command != this.responseCommand)
                return false;
            if (this.parameterId != null && (RdmParameters)ReadUInt16(data, ParameterIdOffset) != this.parameterId)
                return false;

            return MatchesUId(data, 1, this.controller) && MatchesUId(data, SourceIdOffset, this.target);
        }

        private static bool MatchesUId(byte[] data, int offset, UId id) =>
            ReadUInt16(data, offset) == id.ManufacturerId
            && (uint)((data[offset + 2] << 24) | (data[offset + 3] << 16) | (data[offset + 4] << 8) | data[offset + 5]) == id.DeviceId;
    }
}
