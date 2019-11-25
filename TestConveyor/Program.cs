using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConveyor
{
    class Conveyor
    {
        public static FinsTcp Plc1, Plc2, Plc3;
        public static string PlcIp1, PlcIp2, PlcIp3, LocalIp;

        static void Main(string[] args)
        {
            PlcIp1 = "192.168.250.1";
            PlcIp2 = "192.168.250.2";
            PlcIp3 = "192.168.250.3";
            LocalIp = "192.168.250.119";
            Connect();

            ShowCommands();

            bool exit = false;
            ConsoleKey enterKey;

            while (!exit)
            {
                enterKey = Console.ReadKey().Key;

                switch (enterKey)
                {
                    case ConsoleKey.Tab:
                        Start(); break;

                    case ConsoleKey.Q:
                        exit = true; break;

                    case ConsoleKey.A:
                        Console.WriteLine("Enter split by comma TakeOutNumb =");
                        var takeOutNumb = Console.ReadLine();
                        Plc2.TakeOutClothes(takeOutNumb);
                        break;

                    case ConsoleKey.S:
                        Plc1.Packclothes(); break;

                    case ConsoleKey.Z:
                        Task.Factory.StartNew(CheckStatusOf); break;

                }

            }

            Stop();
            Console.WriteLine("\n Disconnected !!!!!");
            Console.ReadKey();
        }

        public static void ShowCommands()
        {

        }

        public static void CheckStatusOf()
        {
            while (true)
            {
                Console.Write($"-{Plc2.GetTakeOutClothesState()}-|");
                Thread.Sleep(500);
            }
            Console.WriteLine("---------------");
        }


        public static void Connect()
        {
            Plc1 = new FinsTcp(LocalIp, PlcIp1, 9600);
            Plc2 = new FinsTcp(LocalIp, PlcIp2, 9600);
            Plc3 = new FinsTcp(LocalIp, PlcIp3, 9600);


            CheckConnection();
        }
        public static void Start()
        {
            Plc2.Start();
            Plc3.Start();

            //Tmr = new TestTimer (CheckStatusOf, 1000);
            //Tmr.Start();
        }
        public static void Stop()
        {
            //Tmr.Stop();

            Plc1.Stop();
            Plc2.Stop();
            Plc3.Stop();
        }
        public static void CheckConnection()
        {
            Console.WriteLine("Plc 1 = " + Plc1.conn(LocalIp, PlcIp1, 9600).ToString());
            Console.WriteLine("Plc 2 = " + Plc2.conn(LocalIp, PlcIp2, 9600).ToString());
            //Console.WriteLine(Plc1.conn(LocalIp, PlcIp3, 9600).ToString());
        }


    }
}
