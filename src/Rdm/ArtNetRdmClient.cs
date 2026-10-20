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

    /// <param name="targetUniverse">The 15-bit Port-Address the device is on.</param>
    /// <remarks>
    /// A transaction number is assigned unless the packet already has a non-zero one; responses
    /// echo it, so it identifies which request a response belongs to.
    /// </remarks>
    public Task SendRdm(RdmPacket packet, IPEndPoint targetAddress, ushort targetUniverse, UId targetId, UId sourceId)
    {
        //Fill in addition details
        packet.Header.SourceId = sourceId;
        packet.Header.DestinationId = targetId;

        if (packet.Header.TransactionNumber == 0)
            packet.Header.TransactionNumber = (byte)Interlocked.Increment(ref this.transactionNumber);

        //Sub Devices
        if (targetId is SubDeviceUId subDevice)
            packet.Header.SubDevice = subDevice.SubDeviceId;

        //Create Rdm Packet
        using (var rdmData = new MemoryStream())
        {
            var rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM packet
            RdmPacket.WritePacket(packet, rdmWriter);

            //Write the checksum
            rdmWriter.WriteUInt16((short)(RdmPacket.CalculateChecksum(rdmData.GetBuffer()) +
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