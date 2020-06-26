using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.ViewModels;
using Impinj.OctaneSdk;

namespace Client.Desktop.ViewModels.Common.Services
{
    public class RfidServiceTest : ViewModelBase
    {
        public TestImpinj Reader = new TestImpinj();
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

        public bool Connection(RfidReaderEntityViewModel newReader, List<RfidAntennaEntityViewModel> antennas)
        {
            IsReading = false;
            return true;
        }
        public void StartStopRead()
        {
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
            Reader.UserEvent += DisplayTag;
            Reader.Start();

            IsReading = true;
        }

        public void StopRead()
        {
            Reader.UserEvent -= DisplayTag;

            IsReading = false;
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
            if (Reader == null ) return "Error";

            return "Connected";
        }


        public delegate void SortedData(ConcurrentDictionary<string, int> data);
        public event SortedData SortedDataEvent;

        private void DisplayTag(List<Tuple<string, int>> tags)
        {
            _data = new ConcurrentDictionary<string, int>();

            foreach (var tag in tags)
            {
                AddData(tag.Item1, tag.Item2);
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

    public class TestImpinj
    {
        public delegate void MyDelegate(List<Tuple<string, int>> val);

        public event MyDelegate UserEvent;

        private List<Tuple<string, int>> tages;

        public void Start()
        {
            tages = new List<Tuple<string, int>>();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                tages.Add(new Tuple<string, int>($"TagNumber - {i}", random.Next(1, 4)));
            }

            UserEvent?.Invoke(tages); ;
        }
    }
}
