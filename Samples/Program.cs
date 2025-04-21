using System;

namespace Haukcode.ArtNet.Samples;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("ArtNet samples!");

        var addr = Haukcode.Network.Utils.GetFirstBindAddress();

        // using (var tester = new ArtTriggerSender(localIp: addr.IPAddress, localSubnetMask: addr.NetMask))
        // {
        //     Console.WriteLine("Hit enter to send select/play command...");
        //     Console.ReadLine();
        //
        //     byte showId = 1;
        //     // Select show
        //     tester.SendArtTrigger(oemCode: 0x6A6B, key: 0x03, subKey: showId);
        //
        //     // Playback
        //     tester.SendArtTrigger(oemCode: 0x6A6B, key: 0x02, subKey: 0x47);
        //
        //
        //     Console.WriteLine("Hit enter to exit...");
        //     Console.ReadLine();
        // }

        using (var tester = new RdmSample(localIp: addr.IPAddress, localSubnetMask: addr.NetMask))
        {
            Console.WriteLine("Sending single ArtPollPacket...");
            //Console.ReadLine();

            tester.SendArtPoll();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Hit enter to request DeviceLabels...");
            Console.ReadLine();

            tester.GetDeviceLabels();
            
            Console.ReadLine();
        }
    }
}
