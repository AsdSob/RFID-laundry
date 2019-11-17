﻿using System;
using Impinj.OctaneSdk;

namespace ImpinjSpeedway
{
    public class RfidCommands
    {
        public ImpinjReader Reader = new ImpinjReader();
        public Settings settings;

        public bool Connection()
        {
            //Reader.Stop();
            //Reader.Disconnect();
            try
            {
                Reader.Connect("192.168.250.55");

            }
            catch (OctaneSdkException ee)
            {
                Console.WriteLine("Octane SDK exception: Reader #1" + ee.Message, "error");
            }
            catch (Exception ee)
            {
                // Handle other .NET errors.
                Console.WriteLine("Exception : Reader #1" + ee.Message, "error");
                Console.WriteLine(ee.StackTrace);
            }
            settings = Reader.QueryDefaultSettings();

            settings.Report.IncludeAntennaPortNumber = true;
            settings.Report.IncludePhaseAngle = true;
            settings.Report.IncludeChannel = true;
            settings.Report.IncludeDopplerFrequency = true;
            settings.Report.IncludeFastId = true;
            settings.Report.IncludeFirstSeenTime = true;
            settings.Report.IncludeLastSeenTime = true;
            settings.Report.IncludePeakRssi = true;
            settings.Report.IncludeSeenCount = true;
            settings.Report.IncludePcBits = true;

            settings.ReaderMode = ReaderMode.MaxThroughput;//.AutoSetDenseReader;
            settings.SearchMode = SearchMode.DualTarget;//.DualTarget;
            settings.Session = 1;
            settings.TagPopulationEstimate = Convert.ToUInt16(200);

            Antenna();

            settings.Report.Mode = ReportMode.Individual;
            Reader.ApplySettings(settings);

            return Reader.IsConnected;
        }


        public void Antenna()
        {
            settings.Antennas.DisableAll();
            var j = settings.Antennas.AntennaConfigs.Count;

            Console.WriteLine($"Number of Antennas = {j}");

            Console.Write("TxPowerInDbm = ");
            var TxPower = Console.ReadLine();

            Console.Write("\n RxSensitivityInDbm = ");
            var RxSensitivity = Console.ReadLine();

            Console.Write("Number of Antenna to On =");
            var i = Console.ReadKey();
            ushort num = Convert.ToUInt16(i);

            settings.Antennas.GetAntenna(num).IsEnabled = true;
            settings.Antennas.GetAntenna(num).TxPowerInDbm = Convert.ToDouble(TxPower);
            settings.Antennas.GetAntenna(num).RxSensitivityInDbm = Convert.ToDouble(RxSensitivity);
        }

        public void Start()
        {
            Console.Beep();
            Reader.Start();
        }

        public void Stop()
        {
            Console.Beep();
            Reader.Stop();
        }



    }
}
