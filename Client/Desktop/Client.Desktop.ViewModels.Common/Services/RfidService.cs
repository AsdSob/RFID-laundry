using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.ViewModels;
using Impinj.OctaneSdk;

namespace Client.Desktop.ViewModels.Common.Services
{
    public class RfidService : ViewModelBase
    {
        public ImpinjReader Reader = new ImpinjReader();
        private Settings settings;

        public RfidReaderEntityViewModel SelectedReader { get; set; }
        private ObservableCollection<Tuple<int, string>> _tags;

        public ObservableCollection<Tuple<int,string>> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
        }
        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();



        public bool Connection(RfidReaderEntityViewModel r, List<RfidAntennaEntityViewModel> antennas)
        {
            if (r == null || !antennas.Any()) return false;

            try
            {
                if (Reader.IsConnected)
                {
                    Reader.Disconnect();
                }

                Reader.Connect(r.ReaderIp, r.ReaderPort);
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

            SetAntennaSettings(antennas);
            SetSettings(r);

            return Reader.IsConnected;
        }

        private void SetSettings(RfidReaderEntityViewModel r)
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
            settings.TagPopulationEstimate = r.TagPopulation;

            settings.Report.Mode = ReportMode.Individual;

            Reader.ApplySettings(settings);
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

        public void StartRead()
        {
            if (!Reader.IsConnected) return;

            _data = new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

            Reader.Start();
            Reader.TagsReported += DisplayTag;
        }

        public void StopRead()
        {
            Reader.TagsReported -= DisplayTag;
            Reader.Stop();
            Reader.Disconnect();
        }

        #region Read tags during specified time

        public void ReadDuringTime(double seconds)
        {
            int time = (int) (seconds * 1000);

            StartRead();

            Thread.Sleep(time);

            StopRead();
        }

        public ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> GetFullData()
        {
            return _data;
        }

        public List<Tuple<int, string>> GetAntennaAndTags()
        {
            var data = GetFullData();
            var tags = new List<Tuple<int, string>>();

            foreach (var antenna in data)
            {
                tags.AddRange(antenna.Value.Select(tag => new Tuple<int, string>(antenna.Key, tag.Key)));
            }

            return tags;
        }

        public List<string> GetOnlyTags()
        {
            var data = GetFullData();
            var tags = new List<string>();

            foreach (var antenna in data)
            {
                foreach (var tag in antenna.Value)
                {
                    if (tags.Any(x => x == tag.Key))
                    {
                        continue;
                    }
                    tags.Add(tag.Key);
                }
            }

            return tags;
        }

        public List<string> GetAntennaTags(int antNumb)
        {
            var data = GetFullData();
            var tags = new List<string>();

            var antenna = data.FirstOrDefault(x => x.Key == antNumb).Value;

            if (antenna == null || antenna.Count == 0)
            {
                return tags;
            }

            tags.AddRange(antenna?.Select(x => x.Key));

            return tags;
        }

        #endregion


        private void DisplayTag(ImpinjReader reader, TagReport report)
        {
            foreach (Tag tag in report)
            {
                AddData(tag.AntennaPortNumber, tag.Epc.ToString(), tag.LastSeenTime.LocalDateTime);
            }
        }

        private void AddData(int antenna, string epc, DateTime time)
        {
            // проверка ест ли словарь антенны 
            if (!_data.TryGetValue(antenna, out ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>> val))
            {
                val = new ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>();

                _data.TryAdd(antenna, val);
            }

            // проверка на наличие чипа в словаре
            if (!val.TryGetValue(epc, out Tuple<DateTime?, DateTime?> times))
            {
                times = new Tuple<DateTime?, DateTime?>(time, null);
                val.TryAdd(epc, times);
            }
            else
            {
                val.TryUpdate(epc, new Tuple<DateTime?, DateTime?>(times.Item1, time), times);
                //данные можно сохранять в БД, но метке можно обнулить
            }
        }

    }

}
