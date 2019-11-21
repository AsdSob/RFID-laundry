using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;

namespace ConsoleAppTimer
{
    class Program
    {
        static void Main(string[] args)
        {
            var testTimer = new TestTimer(DoWork, 1000);
            testTimer.Start();

            Console.WriteLine("timer started");

            Console.ReadKey();
            testTimer.Stop();
            Console.WriteLine("\n timer stopped");
            Console.ReadKey();
        }



        public static void ReadLine()
        {
            Console.Write("\n Hang To =>");
            var a = Console.ReadLine();
            Console.WriteLine(a);
        }

        private static void DoWork()
        {
            Console.WriteLine($"{DateTime.Now:T}");
        }
    }

    public class TestTimer
    {
        private readonly Action _action;
        private System.Timers.Timer _timer;
        private int _timeMs;

        public TestTimer(Action action, int timeMs)
        {
            _action = action;
            _timeMs = timeMs;
        }

        public TestTimer()
        {
            
        }

        public bool WaitTime(int timeMs)
        {
            System.Threading.Thread.Sleep(timeMs);

            return true;
        }

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer(_timeMs);
                _timer.Elapsed += TimerOnElapsed;
            }

            _timer.Start();
        }

        public void Stop()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            _timer.Elapsed -= TimerOnElapsed;
            _timer = null;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _action.Invoke();
        }
    }
}
