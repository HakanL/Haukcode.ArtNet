using System;
using System.Net;
using System.Threading.Tasks;
using Haukcode.ArtNet.Internal;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Samples;

public class ArtTriggerSender : SampleCapture
{
    public ArtTriggerSender(IPAddress localIp, IPAddress localSubnetMask)
        : base(localIp, localSubnetMask)
    {
    }

    protected override void Socket_NewPacket(double timestampMS, ReceiveDataPacket e)
    {
        base.Socket_NewPacket(timestampMS, e);

        switch (e.Packet.OpCode)
        {
            case ArtNet.ArtNetOpCodes.Trigger:
                DebugPrintArtTrigger(e.Packet as ArtNet.Packets.ArtTriggerPacket);
                break;
        }
    }

    public Task SendArtTrigger(Int16 oemCode, byte key, byte subKey)
    {
        return this.client.QueuePacketForSending((IPAddress)null, new ArtTriggerPacket
        {
            OemCode = oemCode,
            Key = key,
            SubKey = subKey,
            Data = new byte[512]
        });
    }

    private void DebugPrintArtTrigger(ArtNet.Packets.ArtTriggerPacket input)
    {
        Console.WriteLine($"Trigger - OemCode: 0x{input.OemCode:X}  Key: {input.Key:X}   SubKey: {input.SubKey:X}");
    }
}
