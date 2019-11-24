using System;
using System.Collections.Concurrent;
using System.Threading;
using Impinj.OctaneSdk;

namespace ImpinjSpeedway
{
    public class Rfid
    {
        public static bool IsConnected;
        public static ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

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

                    case ConsoleKey.W:
                        Console.Write("Read seconds => ");

                        var time =Convert.ToInt32(Console.ReadLine());
                        reader.Start();
                        reader.Reader.TagsReported += DisplayTag;
                        Thread.Sleep(time);
                        reader.Reader.TagsReported -= DisplayTag;
                        reader.Stop();

                        Console.Write("\n Reading Complete");
                        break;

                    case ConsoleKey.S:
                        ShowTags(_data);
                        Console.ReadKey();
                        break;

                    case ConsoleKey.A:
                        exit = true;
                        break;
                }

            }

            Console.WriteLine("\n Disconnected !!!!!");
            Console.ReadKey();

        }

        private static void ShowTags(ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> data)
        {
            foreach (var item in data)
            {
                Console.WriteLine($"\n antenna = --{item.Key}--");
                foreach (var tuple in item.Value)
                {
                    Console.WriteLine($"Tag = {tuple.Key} | {tuple.Value.Item1}, {tuple.Value.Item2}");
                }
            }
        }

        private static void DisplayTag(ImpinjReader reader, TagReport report)
        {
            foreach (Tag tag in report)
            {
                AddData(tag.AntennaPortNumber, tag.Epc.ToString(), tag.LastSeenTime.LocalDateTime);
            }
        }

        private static void AddData(int antenna, string epc, DateTime time)
        {
            // проверка ест ли словарь антенны 
            if (!_data.TryGetValue(antenna, out ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>> val))
            {
                val = new ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>();

                _data.TryAdd(antenna, val);
            }

            // проверка на наличие чипа в словаре
            if (!val.TryGetValue(epc, out Tuple<DateTime?, DateTime?> times))
            {
                times = new Tuple<DateTime?, DateTime?>(time, null);
                val.TryAdd(epc, times);
            }
            else
            {
                val.TryUpdate(epc, new Tuple<DateTime?, DateTime?>(times.Item1, time), times);
                //данные можно сохранять в БД, но метке можно обнулить
            }

            // если метка повторно падает в считыватель, то нужно предыдущие данные сохранять в БД
        }


    }
}
