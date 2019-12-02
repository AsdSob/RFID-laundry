using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTest
{
    class Program
    {
        public static ManualResetEvent Plc1Thread { get; set; }
        public static ManualResetEvent Plc2Thread { get; set; }

        static void Main(string[] args)
        {
            Plc1Thread = new ManualResetEvent(false);
            Plc2Thread = new ManualResetEvent(false);

            Console.WriteLine("Start!");

            Console.ReadKey();

            Task.Factory.StartNew(StartThread1);
            Plc1Thread.WaitOne();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"glavniy potok -{i}");
                Thread.Sleep(500);

                if (i == 4)
                {
                    Plc2Thread.Reset();


                    Plc2Thread.WaitOne();
                }
            }


            Console.ReadKey();
            Console.WriteLine("Finish!");
            Console.ReadKey();

        }


        private static void StartThread1()
        {
            Console.WriteLine("Thread 1 start");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"potok 1 -{i}");
                Thread.Sleep(500);

                if (i == 2)
                {
                    Task.Factory.StartNew(StartThread2);
                }

            }

            Plc1Thread.Set();
        }

        private static void StartThread2()
        {
            Console.WriteLine("Thread 2 start");

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"-- 2 --{i}");
                Thread.Sleep(500);

                if (i == 15)
                {
                    Plc2Thread.Set();
                }
            }


        }
    }
}
