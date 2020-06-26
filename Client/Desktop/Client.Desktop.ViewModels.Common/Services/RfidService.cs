using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.ViewModels;
using Impinj.OctaneSdk;

namespace Client.Desktop.ViewModels.Common.Services
{
    public class RfidService : ViewModelBase
    {
        public ImpinjReader Reader = new ImpinjReader();
        private Settings settings;
        public ConcurrentDictionary<string, int> _data = new ConcurrentDictionary<string, int>();

        private string _connectionStatus;
        private bool _isReading;

        public bool IsReading
        {
            get => _isReading;
            set => Set(ref _isReading, value);
        }
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => Set(() => ConnectionStatus, ref _connectionStatus, value);
        }

        public void Connect(RfidReaderEntityViewModel reader, List<RfidAntennaEntityViewModel> antennas)
        {
            if (reader == null || !antennas.Any()) return;

            Connection(reader, antennas);
        }

        public void Disconnect()
        {
            if(Reader == null || !Reader.IsConnected) return;

            StopRead();
            Reader.Disconnect();
            ConnectionStatus = "Disconnected";
        }

        private void Connection(RfidReaderEntityViewModel newReader, List<RfidAntennaEntityViewModel> antennas)
        {
            try
            {
                Disconnect();

                Reader.Connect(newReader.ReaderIp);
                Reader.Stop();
            }

            catch (OctaneSdkException ee)
            {
                Console.WriteLine("Octane SDK exception: Reader #1" + ee.Message, "error");
            }
            catch (Exception ee)
            {
                Console.WriteLine("Exception : Reader #1" + ee.Message, "error");
                Console.WriteLine(ee.StackTrace);
            }

            if (!Reader.IsConnected) return;
            ConnectionStatus = "Connected";

            SetSettings(newReader.TagPopulation);
            SetAntennaSettings(antennas);
            Reader.ApplySettings(settings);
        }

        private void SetSettings(ushort tagPopulation)
        {
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

            //ReaderMode.AutoSetDenseReaderDeepScan | Rx = -70 | Tx = 15/20
            //ReaderMode.MaxThrouput | Rx = -80 | Tx = 15

            settings.ReaderMode = ReaderMode.AutoSetDenseReaderDeepScan;//.AutoSetDenseReader;
            settings.SearchMode = SearchMode.DualTarget;//.DualTarget;
            settings.Session = 1;
            settings.TagPopulationEstimate = tagPopulation;
            settings.Report.Mode = ReportMode.Individual;
        }

        private void SetAntennaSettings(List<RfidAntennaEntityViewModel> antennas)
        {
            settings.Antennas.DisableAll();
            foreach (var antenna in antennas)
            {
                settings.Antennas.GetAntenna((ushort)antenna.AntennaNumb).IsEnabled = true;
                settings.Antennas.GetAntenna((ushort)antenna.AntennaNumb).TxPowerInDbm = antenna.TxPower;
                settings.Antennas.GetAntenna((ushort)antenna.AntennaNumb).RxSensitivityInDbm = antenna.RxSensitivity;
            }
        }

        public void StartStopRead()
        {
            if (!Reader.IsConnected) return;

            if (!IsReading)
            {
                StartRead();
            }
            else
            {
                StopRead();
            }
        }

        public void StartRead()
        {
            IsReading = true;
            Reader.TagsReported += DisplayTag;
            Reader.Start();
        }

        public void StopRead()
        {
            IsReading = false;
            Reader.Stop();
            Reader.TagsReported -= DisplayTag;
        }

        public string GetStartStopString()
        {
            if (IsReading)
            {
                return "Stop";
            }
            else
            {
                return "Start";
            }
        }

        public string CheckConnection()
        {
            if(Reader == null || !Reader.IsConnected) return "Error";

            if (Reader.IsConnected)
            {
                return "Connected";
            }
            else
            {
                return "Not Connected";
            }
        }


        public delegate void SortedData(ConcurrentDictionary<string, int> data);
        public event SortedData SortedDataEvent;

        private void DisplayTag(ImpinjReader reader, TagReport report)
        {
            _data = new ConcurrentDictionary<string, int>();

            foreach (Tag tag in report)
            {
                AddData(tag.Epc.ToString(), tag.AntennaPortNumber);
            }

            SortedDataEvent?.Invoke(_data);
        }

        private void AddData(string epc, int antenna)
        {
            if (!_data.TryGetValue(epc, out int val))
            {
                _data.TryAdd(epc, antenna);
            }
            else
            {
                _data.TryUpdate(epc, antenna, val);
            }
        }

    }

}
