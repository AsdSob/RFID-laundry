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
            Console.WriteLine("Conveyor test start");
            Connect();
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
                        ShowStatus();
                        break;



                }
            }
        }

        private static void Connect()
        {
            Plc1 = new FinsTcp("192.168.250.119", "192.168.250.1", 9600);
            Belt1 = new FinsTcp("192.168.250.119", "192.168.250.2", 9600);

            Console.WriteLine($"Plc1 = {Plc1.conn("192.168.250.119", "192.168.250.1", 9600)}"); 
            Console.WriteLine($"Belt1 = {Belt1.conn("192.168.250.119", "192.168.250.2", 9600)}");

            Thread.Sleep(1000);

            Plc1.Start();
            Belt1.Start();
        }

        private static void ShowStatus()
        {
            Task.Factory.StartNew(ShowAsync);
        }

        private static void ShowAsync()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine($"=======>  {i}=={DateTime.Now.TimeOfDay}");

                Console.WriteLine($"GetTakeOutClothesState = {Belt1.GetTakeOutClothesState()}");
                Console.Write($"DialState = {Belt1.DialState()}");

                Console.WriteLine($"                <=======");
                Thread.Sleep(1000);
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
