using System;
using Impinj.OctaneSdk;

namespace ImpinjSpeedway
{
    public class Rfid
    {
        public static bool IsConnected;

        static void Main(string[] args)
        {
            RfidCommands reader = new RfidCommands();
            TagReport tagReport = null;
            Console.WriteLine("Begin--------");

            bool exit = false;
            ConsoleKey enterKey;

            while (!exit)
            {
                enterKey = Console.ReadKey().Key;

                switch (enterKey)
                {
                    case ConsoleKey.Tab:
                        IsConnected = reader.Connection();
                        Console.WriteLine($"Reader Connected - {IsConnected}");
                        break;

                    case ConsoleKey.Q:
                        reader.Antenna();
                        break;

                    case ConsoleKey.W:
                        reader.Start();
                        Console.WriteLine("Started...");
                        break;

                    case ConsoleKey.E:
                        reader.Stop();
                        Console.WriteLine("Stop...");
                        break;

                    case ConsoleKey.A:
                        exit = true;
                        break;

                    case ConsoleKey.Z:
                        tagReport = reader.Reader.QueryTags(5);
                        Console.WriteLine("Tag reading complete");
                        break;

                    case ConsoleKey.X:
                        DisplayTag(reader.Reader, tagReport);
                        break;
                }

            }

            Console.WriteLine("\n Disconnected !!!!!");
            Console.ReadKey();

        }

        private static void DisplayTag(ImpinjReader reader, TagReport report)
        {
            foreach (Tag tag in report)
            {
                if (tag.Epc != null)
                {
                    Console.WriteLine(
                        $"Reader= {reader.Name} /Antenna= {tag.AntennaPortNumber} /EPC= {tag.Epc} /RSS= {tag.PeakRssiInDbm} /Frequency= {tag.ChannelInMhz} /Phase= {tag.PhaseAngleInRadians}");
                }
            }
        }

    }
}
