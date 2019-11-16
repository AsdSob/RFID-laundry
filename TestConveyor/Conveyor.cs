using System;
using ConsoleAppTimer;

namespace TestConveyor
{
    class Conveyor
    {
        public static FinsTcp Plc1, Plc2, Plc3;
        public static string PlcIp1, PlcIp2, PlcIp3, LocalIp;
        public static TestTimer Tmr;

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

                    case ConsoleKey.S:
                        CheckStatusOf(); break;

                    case ConsoleKey.E:
                        CheckClothReady(); break;

                    case ConsoleKey.R:
                        HangUpTo(); break;

                    case ConsoleKey.T:
                        SetHanging(); break;

                    case ConsoleKey.Z:
                        SetNowPoint(); break;

                    case ConsoleKey.X:
                        GoTo(); break;

                    case ConsoleKey.C:
                        HangIn(); break;

                    case ConsoleKey.A:
                        AutoMode(); break;
                }

            }

            Stop();
            Console.WriteLine("\n Disconnected !!!!!");
            Console.ReadKey();
        }

        public static void ShowCommands()
        {
            Console.WriteLine("---- Commands ----");
            Console.WriteLine("Tab => Start");
            Console.WriteLine("Q => Disconnect and Quit");
            Console.WriteLine("S => Show the statuses");
            Console.WriteLine("E => To get cloth ready and send to Line 1");
            Console.WriteLine("R => Hang up to entered point");
            Console.WriteLine("T => Set Hanging Point");
            Console.WriteLine("Z => Set Now Point");
            Console.WriteLine("X => Go To Point");
            Console.WriteLine("C => Hang In");
            Console.WriteLine("A => Auto Mode");

        }

        public static void CheckStatusOf()
        {
            Console.WriteLine("");
            Console.WriteLine("---------------");
            //Console.WriteLine("GetWaitHangNum() => " + Plc2.GetWaitHangNum());
            //Console.WriteLine("GetBasePoint() => " + Plc2.GetBasePoint());
            //Console.WriteLine("GetHanginpoint() => " + Plc2.GetHanginpoint());
            //Console.WriteLine("GetNowPoint() => " + Plc2.GetNowPoint());
            //Console.WriteLine("GetClotheInHook() => " + Plc2.GetClotheInHook());
            //Console.WriteLine("GetWaitHangNum() => " + Plc1.GetWaitHangNum());
            //Console.WriteLine("GetClotheReady() => " + Plc1.GetClotheReady());
            var i = 1;
            Tmr = new TestTimer();
            while (i<100)
            {
                i++;
                Console.WriteLine("HangInState() => " + Plc2.Hang_In_State());
                Tmr.WaitTime(500);
            }
            Console.WriteLine("---------------");


        }

        public static void HangIn()
        {
            Console.Write(" \n HangIn =>  ");
            var v = Plc2.Hang_In();

            Console.Write("  -- Done -- result ( " + v + " )");
        }

        public static void GoTo()
        {
            Console.Write(" \n GoToPoint =>  ");
            var v = Plc2.GotoPoint();

            Console.Write("  -- Done -- result ( " + v + " )");
        }

        public static void SetNowPoint()
        {
            Console.Write(" \n Set Now Point To =>  ");
            var a = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
            var v = Plc2.SetNowPoint(a);

            Console.Write("  -- Done -- result ( " + v + " )");
        }

        private static void SetHanging()
        {
            Console.Write(" \n Set Hanging Point To =>  ");
            var a = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
            var v = Plc2.SetHanginpoint(a);

            Console.Write("  -- Done -- result ( " + v + " )");
        }

        public static void HangUpTo()
        {
            Console.Write(" \n Hang To =>  ");
            var a = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
            Plc2.HangUpToPoint(a);
            ///esli net belya vozvrashaet 0

            Console.Write("  -- Done -- ");
        }

        public static void CheckClothReady()
        {
            Plc1.GetClotheReady();
            Plc1.Sorting(1);
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


        public static void AutoMode()
        {
            Tmr = new TestTimer();
            CheckStatusOf();
            var i = 1;

            while (i < 4)
            {
                Console.WriteLine($"Start 1 = {DateTime.Now:T}");
                SendToLine1();

                Console.WriteLine($"Start 2 = {DateTime.Now:T}");
                CheckBelt();

                Console.WriteLine($"Start 3 = {DateTime.Now:T}");
                SendToSlot();

                i++;
                Console.WriteLine($"{i} circle");
                //Console.ReadKey();
            }
        }

        public static void SendToSlot()
        {
            Tmr.WaitTime(2000);

            while (!Plc2.GetClotheInHook())
            {
                Console.Write(" - HangIn F");
                Plc2.Hang_In();
                Tmr.WaitTime(1000);
            }

            while (Plc2.GetClotheInHook())
            {
                Console.Write(" - HangIn T");
                Tmr.WaitTime(1000);
            }
            return;
        }

        public static void CheckBelt()
        {
            //TODO: Проверка на запоненность линии, подготовка линии

            PrepareBelt1Slot();
        }

        public static void PrepareBelt1Slot()
        {
            var i = Plc2.GetNowPoint();
            var j = CheckBeltSlot(i);

            //if ( j == i) return;
            
            //TODO: поменять i++ на j.
            Plc2.SetNowPoint(i +1);
            Tmr.WaitTime(500);
            Plc2.GotoPoint();

            while (Plc2.DialState())
            {
                Tmr.WaitTime(500);
            }
        }

        /// <summary>
        /// проверка свободности слота в линии 1, слот может быть свободен или занять.
        /// </summary>
        /// <returns></returns>
        public static int CheckBeltSlot(int i)
        {
            //TODO: проверка свободен ли слот, найти ближайший свободный слот
            return i;
        }

        /// <summary>
        /// проверка линии подачи белья
        /// </summary>
        public static void SendToLine1()
        {
            while (!Plc1.GetClotheReady())
            {
                Console.Write(".");
                Tmr.WaitTime(500);
            }

            var i = Plc1.GetWaitHangNum();
            Console.WriteLine("\n Waiting Hang Qty = " + i);

            if (i > 1)
            {
                Plc1.Sorting(1);
                Plc1.ResetWaitHangNum();
            }

            if (i == 1)
            {
                Plc1.Sorting(1);
            }

            //if (i > 1)
            //{
            //    Console.WriteLine("\n Waiting Hang Qty = " + i);
            //    Plc1.Sorting(1);
            //    Plc1.ResetWaitHangNum();
            //}

        }
    }
}
