using System;
using System.Linq;
using System.Threading.Tasks;
using Impinj.OctaneSdk;
using static System.ConsoleKey;

namespace RFID_Test
{
    class Program
    {
        public static RfidCommon Impinj = new RfidCommon();
        public static int  Setchik;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var isRunning = true;

            while (isRunning)
            {
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Q:
                        isRunning = false;
                        break;

                    case ConsoleKey.Tab:
                        Start();
                        break;

                    case ConsoleKey.Z:
                        Console.WriteLine(" +10.0 to +31.5 | Enter TxPower=>");
                        double txPower = Convert.ToDouble(Console.ReadLine());
                        TXSet(txPower);
                        break;

                    case ConsoleKey.X:
                        Console.WriteLine(" -85 to 0 | Enter RxPower=>");
                        double rxPower = Convert.ToDouble(Console.ReadLine());
                        RXSet(rxPower);
                        break;

                    case ConsoleKey.A:
                        Task.Factory.StartNew(StartShowTags);
                        break;

                    case ConsoleKey.S:
                        StopShowTags();
                        break;
                }
            }
        }

        public static void Start()
        {
            Impinj.Connection();
            Console.WriteLine("Connected");
        }

        public static void StartShowTags()
        {
            Setchik = 0;
            Impinj.Reader.TagsReported += ShowTags;
            Console.WriteLine("---Started---");
        }

        public static void StopShowTags()
        {
            Impinj.Reader.TagsReported -= ShowTags;
            Console.WriteLine("---Stopped---");
        }

        public static void ShowTags(ImpinjReader reader, TagReport report)
        {
            Setchik++;
            foreach (var tag in report.Tags.OrderBy(x=> x.AntennaPortNumber))
            {
                Console.WriteLine($"EPC== {tag.Epc} |||||== {tag.TagSeenCount}");
            }
        }

        public static void TXSet(double power)
        {
            for (int i = 1; i <= 4; i++)
            {
                Impinj.settings.Antennas[i].TxPowerInDbm = power;
            }

            Impinj.Reader.ApplySettings(Impinj.settings);
        }

        public static void RXSet(double power)
        {
            for (int i = 1; i <= 4; i++)
            {
                Impinj.settings.Antennas[i].RxSensitivityInDbm = power;
            }

            Impinj.Reader.ApplySettings(Impinj.settings);
        }
    }
}
