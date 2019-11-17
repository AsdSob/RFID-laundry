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
                        break;

                    case ConsoleKey.E:
                        reader.Stop();
                        break;

                    case ConsoleKey.A:
                        exit= true;
                        break;

                    case ConsoleKey.Z:
                        reader.Reader.TagsReported += DisplayTag;
                        break;
                    case ConsoleKey.X:
                        reader.Reader.TagsReported -= DisplayTag;
                        break;


                }

            }

            reader.Reader.TagsReported += DisplayTag;

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
