using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impinj.OctaneSdk;

namespace ImpinjSpeedway
{
    public class Rfid
    {
        public static ImpinjReader Reader = new ImpinjReader();
        public static Settings ReaderSetting = new Settings();

        static void Main(string[] args)
        {
            Console.WriteLine("Begin--------");

            Reader.Connect();
            Reader.Address();
        }
    }
}
