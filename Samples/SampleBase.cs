using System;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Haukcode.ArtNet;
using Haukcode.ArtNet.Internal;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Samples;

public abstract class SampleBase : IDisposable
{
    protected readonly ArtNetClient client;
    private Task writerTask;

    public SampleBase(IPAddress localIp, IPAddress localSubnetMask)
    {
        var channel = Channel.CreateUnbounded<ReceiveDataPacket>();

        this.client = new ArtNetClient(
            localAddress: localIp,
            localSubnetMask: localSubnetMask,
            channelWriter: p => WritePacket(channel, p),
            channelWriterComplete: () => channel.Writer.Complete());

        this.writerTask = Task.Factory.StartNew(async () =>
        {
            await WriteToWriterAsync(channel, CancellationToken.None);
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
    }

    private static async Task WritePacket(Channel<ReceiveDataPacket> channel, ReceiveDataPacket receiveData)
    {
        var dmxData = TransformPacket(receiveData);

        if (dmxData == null)
            return;

        await channel.Writer.WriteAsync(dmxData, CancellationToken.None);
    }

    private static ReceiveDataPacket TransformPacket(ReceiveDataPacket receiveData)
    {
        var copyBuf = new byte[receiveData.Packet.PacketLength];
        receiveData.Packet.WriteToBuffer(copyBuf);

        receiveData.Packet = ArtNetPacket.Parse(copyBuf);

        return receiveData;
    }

    private async Task WriteToWriterAsync(Channel<ReceiveDataPacket> inputChannel, CancellationToken cancellationToken)
    {
        await foreach (var dmxData in inputChannel.Reader.ReadAllAsync(cancellationToken))
        {
            Socket_NewPacket(dmxData.TimestampMS, dmxData);
        }
    }

    protected abstract void Socket_NewPacket(double timestampMS, ReceiveDataPacket e);

    public void Dispose()
    {
        this.client.Dispose();
    }
}
