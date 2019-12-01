using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PALMS.Settings.ViewModel.Common;
using Console = System.Console;

namespace ConveyorTest
{
    class Program
    {
        private static FinsTcp Plc1, Belt1, Belt2;

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
                        Connect();
                        break;

                    case ConsoleKey.A:
                        ShowStatus();
                        break;

                    case ConsoleKey.S:
                        HangTest();
                        break;

                }
            }
        }

        private static void Connect()
        {
            Plc1 = new FinsTcp("192.168.250.119", "192.168.250.1", 9600);
            Belt1 = new FinsTcp("192.168.250.119", "192.168.250.2", 9600);

            Thread.Sleep(1000);
            Console.WriteLine($"Plc1 = {Plc1.conn("192.168.250.119", "192.168.250.1", 9600)}"); 
            Console.WriteLine($"Belt1 = {Belt1.conn("192.168.250.119", "192.168.250.2", 9600)}"); 
        }

        private static void ShowStatus()
        {
            Task.Factory.StartNew(ShowAsync);
        }

        private static void ShowAsync()
        {
            while (true)
            {
                Console.Write($"GetClothInHook{Belt1.GetClotheInHook()} --");
                Console.WriteLine($"Hang_In_State{Belt1.Hang_In_State()}");
                Thread.Sleep(500);
            }
        }

        private static void HangTest()
        {
            var slot = Belt1.GetNowPoint();
            Console.WriteLine("--Hanging Start--");
            Belt1.Hang_In();
            Console.WriteLine("--Hanging Stop--");

        }
    }
}
