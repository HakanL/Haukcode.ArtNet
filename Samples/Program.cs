using System;
using System.Net;

namespace Haukcode.ArtNet.Samples;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to ArtNet samples!");

        var addr = Haukcode.Network.Utils.GetFirstBindAddress();

        do
        {
            Console.WriteLine("Choose sample:");
            Console.WriteLine("\t1 - ArtTriggerSample");
            Console.WriteLine("\t2 - RdmSample");
            Console.WriteLine("\t0 - Exit");
            switch (Console.ReadLine())
            {
                case "1":
                    ArtTriggerSample(addr);
                    break;
                case "2":
                    RdmSample(addr);
                    break;
                case "0":
                    Console.WriteLine("Bye...");
                    return;
            }
           
        } while (true);
    }

    private static void ArtTriggerSample((IPAddress IPAddress, IPAddress NetMask, byte[] MacAddress) addr)
    {
        using (var tester = new ArtTriggerSender(localIp: addr.IPAddress, localSubnetMask: addr.NetMask))
        {
            Console.WriteLine("Hit enter to send select/play command...");
            Console.ReadLine();
        
            byte showId = 1;
            // Select show
            tester.SendArtTrigger(oemCode: 0x6A6B, key: 0x03, subKey: showId);
        
            // Playback
            tester.SendArtTrigger(oemCode: 0x6A6B, key: 0x02, subKey: 0x47);
            
            Console.WriteLine("Hit enter to exit...");
            Console.ReadLine();
        }
    }

    private static void RdmSample((IPAddress IPAddress, IPAddress NetMask, byte[] MacAddress) addr)
    {
        using (var tester = new RdmSample(localIp: addr.IPAddress, localSubnetMask: addr.NetMask))
        {
            Console.WriteLine("Sending single ArtPollPacket...");
            //Console.ReadLine();

            tester.SendArtPoll();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Hit enter to request DeviceLabels...");
            Console.ReadLine();

            tester.GetDeviceLabels();
            
            Console.WriteLine("Hit enter to exit...");
            Console.ReadLine();
        }
    }
}
