using System;

namespace Haukcode.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ArtNet samples!");

            using (var tester = new ArtNetCapture())
            {
                Console.WriteLine("Hit enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
