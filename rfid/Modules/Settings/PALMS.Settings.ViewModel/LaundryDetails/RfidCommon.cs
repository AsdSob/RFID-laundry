using System;
using Impinj.OctaneSdk;

namespace PALMS.Settings.ViewModel.LaundryDetails
{
    public class RfidCommon
    {
        public ImpinjReader Reader = new ImpinjReader();
        public Impinj.OctaneSdk.Settings settings;

        public bool Connection()
        {
            try
            {
                Reader.Connect("192.168.250.55");
                Reader.Stop();

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
            settings.Report.IncludeSeenCount = true;

            settings.ReaderMode = ReaderMode.MaxThroughput;//.AutoSetDenseReader;
            settings.SearchMode = SearchMode.DualTarget;//.DualTarget;
            settings.Session = 1;
            settings.TagPopulationEstimate = Convert.ToUInt16(200);

            settings.Report.Mode = ReportMode.Individual;

            Antenna();

            Reader.ApplySettings(settings);

            return Reader.IsConnected;
        }


        public void Antenna()
        {
            settings.Antennas.DisableAll();
            var j = settings.Antennas.AntennaConfigs.Count;

            for (ushort i = 1; i <= 4; i++)
            {
                settings.Antennas.GetAntenna(i).IsEnabled = true;
                settings.Antennas.GetAntenna(i).TxPowerInDbm = Convert.ToDouble("15");
                settings.Antennas.GetAntenna(i).RxSensitivityInDbm = Convert.ToDouble("-70");
            }

        }

        public void Start()
        {
            Reader.Start();
        }

        public void Stop()
        {
            Reader.Stop();
        }

        public bool Connect()
        {
            Connection();
            return Reader.IsConnected;
        }



    }
}
