using System;
using System.Collections.Generic;
using Impinj.OctaneSdk;

namespace TestConveyor.Svoyo.Services
{
    public class ImpinjSpeedway : IRfidReader
    {
        public ImpinjReader Impinj;
        public Settings Settings;

        public bool Connection(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                //TODO: Show msg 
            }

            try
            {
                Impinj = new ImpinjReader();
                Impinj.Connect(address);
                Impinj.Stop();
            }

            catch (OctaneSdkException ee)
            {
                Console.WriteLine($"Octane SDK exception: Impinj \" {address} \" - {ee.Message} error");
            }

            catch (Exception ee)
            {
                Console.WriteLine($"Exception : Impinj \" {address} \" - {ee.Message} error");
                Console.WriteLine(ee.StackTrace);
            }
            Settings = Impinj.QueryDefaultSettings();

            Settings.Report.IncludeFastId = true;
            Settings.Report.IncludeFirstSeenTime = true;
            Settings.Report.IncludeAntennaPortNumber = true;
            Settings.Report.IncludeLastSeenTime = true;
            Settings.Report.IncludeSeenCount = true;

            Settings.Report.Mode = ReportMode.Individual;
            Settings.ReaderMode = ReaderMode.AutoSetDenseReader;//.AutoSetDenseReader;
            Settings.SearchMode = SearchMode.DualTarget;//.DualTarget;
            Settings.Session = 1;
            Settings.TagPopulationEstimate = Convert.ToUInt16(200);

            Impinj.ApplySettings(Settings);

            return Impinj.IsConnected;
        }

        public void RunAntenna(ushort numb)
        {
            var j = Settings.Antennas.Length;

            Settings.Antennas.GetAntenna(numb).IsEnabled = true;
            Settings.Antennas.GetAntenna(numb).TxPowerInDbm = Convert.ToDouble("15");
            Settings.Antennas.GetAntenna(numb).RxSensitivityInDbm = Convert.ToDouble("-70");
        }

        public TagReport GeTagReport(int timeInSeconds)
        {
            var report = Impinj.QueryTags(seconds: timeInSeconds);

            return report;
        }
        
        public void Start()
        {
            Impinj.Start();
        }

        public void Stop()
        {
            Impinj.Stop();
        }

        public bool Connect(string address)
        {
            Connection(address);
            return Impinj.IsConnected;
        }

        public void SetAntenna(int i)
        {
            RunAntenna((ushort) i);
        }

        public void SetAllAntenna()
        {
            Settings.Antennas.DisableAll();

            var antennas = Settings.Antennas.AntennaConfigs.Count;

            for (ushort i = 1; i <= antennas; i++)
            {
                RunAntenna(i);
            }
        }
    }
}
