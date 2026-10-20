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
        : base(localAddress, localSubnetMask, channelWriter, channelWriterComplete, port, rdmId, senderCount)
    {
    }

    private int transactionNumber;

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
        if (targetUniverse > MaxPortAddress)
            throw new ArgumentOutOfRangeException(nameof(targetUniverse), targetUniverse, $"The Port-Address must be 0 to {MaxPortAddress}.");

        //Fill in addition details
        packet.Header.SourceId = sourceId;
        packet.Header.DestinationId = targetId;
        packet.Header.TransactionNumber = (byte)(Interlocked.Increment(ref this.transactionNumber) - 1);

        //Sub Devices
        if (targetId is SubDeviceUId subDevice)
            packet.Header.SubDevice = subDevice.SubDeviceId;

        // E1.20 9: "GET commands sent to the SUB_DEVICE_ALL_CALL Sub-Device ID are not allowed"
        if (packet.Header.Command == RdmCommands.Get && packet.Header.SubDevice == AllSubDevices)
            throw new InvalidOperationException("A GET can't be sent to all sub-devices (0xFFFF).");

        //Create Rdm Packet
        using (var rdmData = new MemoryStream())
        {
            var rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM packet
            RdmPacket.WritePacket(packet, rdmWriter);

            //Write the checksum
            rdmWriter.WriteUInt16((ushort)(RdmPacket.CalculateChecksum(rdmData.GetBuffer(), 0, (int)rdmData.Length) +
                                           (int)RdmVersions.SubMessage + (int)DmxStartCodes.RDM));

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
}