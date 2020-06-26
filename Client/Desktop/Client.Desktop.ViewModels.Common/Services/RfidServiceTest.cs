using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Impinj.OctaneSdk;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.Services
{
    public class RfidServiceTest : ViewModelBase
    {
        public TestImpinj Reader = new TestImpinj();
        private Settings settings;
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;

        private string _connectionStatus;
        private string _startStopButton;
        private ObservableCollection<RfidReaderEntityViewModel> _rfidReaders;
        private ConcurrentDictionary<string, int> _data;
        private RfidReaderEntityViewModel _selectedRfidReader;
        private ObservableCollection<RfidAntennaEntityViewModel> _antennas;

        public ConcurrentDictionary<string, int> Data
        {
            get => _data;
            set => Set(ref _data, value);
        }
        public ObservableCollection<RfidAntennaEntityViewModel> Antennas
        {
            get => _antennas;
            set => Set(ref _antennas, value);
        }
        public RfidReaderEntityViewModel SelectedRfidReader
        {
            get => _selectedRfidReader;
            set => Set(ref _selectedRfidReader, value);
        }
        public ObservableCollection<RfidReaderEntityViewModel> RfidReaders
        {
            get => _rfidReaders;
            set => Set(ref _rfidReaders, value);
        }
        public string StartStopButton
        {
            get => _startStopButton;
            set => Set(() => StartStopButton, ref _startStopButton, value);
        }
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => Set(() => ConnectionStatus, ref _connectionStatus, value);
        }

        public RelayCommand StartStopReaderCommand { get; }


        public RfidServiceTest(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            StartStopReaderCommand = new RelayCommand(StartStopRead);

            StartStopButton = "Start";
            ConnectionStatus = "Connected";
            Data = new ConcurrentDictionary<string, int>();
            Initialize();
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedRfidReader))
            {
                StartStopReaderCommand.RaiseCanExecuteChanged();
            }
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var reader = await _laundryService.GetAllAsync<RfidReaderEntity>();
                var readers = reader.Select(x => new RfidReaderEntityViewModel(x));
                RfidReaders = readers.ToObservableCollection();

                var antenna = await _laundryService.GetAllAsync<RfidAntennaEntity>();
                var antennas = antenna.Select(x => new RfidAntennaEntityViewModel(x));
                Antennas = antennas.ToObservableCollection();
            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }
        }

        public bool CheckConnection()
        {
            var isConnected = false;

            if (ConnectionStatus == "Connected")
            {
                ConnectionStatus = "Disconnected";
                isConnected = false;
            }
            else
            {
                isConnected = true;
                ConnectionStatus = "Connected";
            }

            return isConnected;
        }

        public void Connect()
        {
            if (SelectedRfidReader == null) return;
            var antennas = Antennas.Where(x => x.RfidReaderId == SelectedRfidReader.Id).ToList();
            if (!antennas.Any()) return;

            Connection(SelectedRfidReader, antennas);
        }

        public void Disconnect()
        {
            if (Reader == null) return;

            Reader.Stop();
            CheckConnection();
        }

        public bool Connection(RfidReaderEntityViewModel newReader, List<RfidAntennaEntityViewModel> antennas)
        {
            CheckConnection();
            return true;
        }

      
        private void DisplayTag(List<Tuple<string, int>> tags)
        {
            Data = new ConcurrentDictionary<string, int>();

            foreach (var tag in tags)
            {
                AddData(tag.Item1, tag.Item2);
            }
        }

        private void AddData(string epc, int antenna)
        {
            if (!Data.TryGetValue(epc, out int val))
            {
                Data.TryAdd(epc, antenna);
            }
            else
            {
                Data.TryUpdate(epc, antenna, val);
            }
        }

        public void StartStopRead()
        {
            if (StartStopButton == "Start")
            {
                StartRead();
                StartStopButton = "Stop";
            }
            else
            {
                StopRead();
                StartStopButton = "Start";
            }
        }

        public void StartRead()
        {
            Reader.UserEvent += DisplayTag;
            Reader.Start();
        }

        public void StopRead()
        {
            Reader.UserEvent -= DisplayTag;
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

            for (int i = 0; i < 5; i++)
            {
                tages.Add(new Tuple<string, int>($"TagNumber - {i}", random.Next(1, 4)));
            }

            UserEvent.Invoke(tages); ;
        }

        public void Stop()
        {
            
        }
    }
}
