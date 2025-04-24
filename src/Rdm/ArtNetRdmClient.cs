namespace Haukcode.ArtNet.Rdm;

public class ArtNetRdmClient : ArtNetClient
{
    public ArtNetRdmClient(
        IPAddress localAddress,
        IPAddress localSubnetMask,
        Func<Haukcode.ArtNet.Internal.ReceiveDataPacket, Task>? channelWriter = null,
        Action? channelWriterComplete = null,
        int port = DefaultPort,
        UId? rdmId = null)
        : base(localAddress, localSubnetMask, channelWriter, channelWriterComplete, port, rdmId)
    {
    }

    public Task SendRdm(RdmPacket packet, IPEndPoint targetAddress, short targetUniverse, UId targetId)
    {
        return SendRdm(packet, targetAddress, targetUniverse, targetId, RdmId);
    }

    public Task SendRdm(RdmPacket packet, IPEndPoint targetAddress, short targetUniverse, UId targetId, UId sourceId)
    {
        //Fill in addition details
        packet.Header.SourceId = sourceId;
        packet.Header.DestinationId = targetId;

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