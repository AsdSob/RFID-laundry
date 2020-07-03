using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Impinj.OctaneSdk;

namespace Client.Desktop.ViewModels.Services
{
    public class RfidService : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IDialogService _dialogService;
        private readonly ImpinjReader _impinjReader = new ImpinjReader();
        private Settings _settings;

        public ConcurrentDictionary<string, int> _data = new ConcurrentDictionary<string, int>();

        private string _connectionStatus;
        private bool _isReading;
        private ObservableCollection<RfidReaderEntityViewModel> _readers;
        private ObservableCollection<RfidAntennaEntityViewModel> _antennas;
        private RfidReaderEntityViewModel _selectedReader;
        private string _startStopContent;
        private ObservableCollection<RfidTagViewModel> _tags;
        private string _addShowContent;
        private RfidTagViewModel _selectedTag;

        public RfidTagViewModel SelectedTag
        {
            get => _selectedTag;
            set => Set(ref _selectedTag, value);
        }
        public string AddShowContent
        {
            get => _addShowContent;
            set => Set(ref _addShowContent, value);
        }
        public ObservableCollection<RfidTagViewModel> Tags
        {
            get => _tags;
            set => Set(ref _tags, value);
        }
        public string StartStopContent
        {
            get => _startStopContent;
            set => Set(ref _startStopContent, value);
        }
        public RfidReaderEntityViewModel SelectedReader
        {
            get => _selectedReader;
            set => Set(ref _selectedReader, value);
        }

        public ObservableCollection<RfidAntennaEntityViewModel> Antennas
        {
            get => _antennas;
            set => Set(ref _antennas, value);
        }
        public ObservableCollection<RfidReaderEntityViewModel> Readers
        {
            get => _readers;
            set => Set(ref _readers, value);
        }
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

        public ObservableCollection<ClientLinenEntityViewModel> Linens { get; set; }
        public ClientLinenEntityViewModel SelectedLinen { get; set; }

        public List<RfidAntennaEntityViewModel> SortedAntennas => SortAntennas();

        public RelayCommand ConnectToReaderCommand { get; }
        public RelayCommand StartStopCommand { get; }
        public RelayCommand RfidReaderWindowCommand { get; }
        public RelayCommand AddShowCommand { get; }
        public RelayCommand RemoveTagCommand { get; }


        public RfidService(ILaundryService laundryService, IResolver resolver, IDialogService dialoge)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dialogService = dialoge ?? throw new ArgumentNullException(nameof(dialoge));

            ConnectToReaderCommand = new RelayCommand(Connect, () => SelectedReader !=null);
            StartStopCommand = new RelayCommand(StartStopRead, () => SelectedReader !=null);
            RfidReaderWindowCommand = new RelayCommand(RfidReaderWindow);
            AddShowCommand = new RelayCommand(AddShowLinenTag, () => SelectedTag != null);
            RemoveTagCommand = new RelayCommand(RemoveTag,()=> SelectedTag != null);


            Initialize();
            UpdateStartStopContent();

            PropertyChanged += OnPropertyChanged;
        }

        private async void Initialize()
        {
            Readers = await _laundryService.RfidReaders();
            Antennas = await _laundryService.RfidAntennas();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedReader))
            {
                RaisePropertyChanged(() => SortedAntennas);
                ConnectToReaderCommand.RaiseCanExecuteChanged();
                StartStopCommand.RaiseCanExecuteChanged();
            }
            else 
            
            if (e.PropertyName == nameof(SelectedTag))
            {
                AddShowCommand.RaiseCanExecuteChanged();
                RemoveTagCommand.RaiseCanExecuteChanged();
                UpdateShowAddContent();
            }
            else 
            
            if (e.PropertyName == nameof(IsReading))
            {
                UpdateStartStopContent();
            }
        }

        private List<RfidAntennaEntityViewModel> SortAntennas()
        {
            var antennas = new List<RfidAntennaEntityViewModel>();

            antennas.AddRange(Antennas?.Where(x=> x.RfidReaderId == SelectedReader?.Id));

            return antennas;
        }


#region Connection

        public virtual void Connect()
        {
            if (SelectedReader == null) return;

            Connection(SelectedReader, SortedAntennas);
        }

        public void Connect(RfidReaderEntityViewModel reader, List<RfidAntennaEntityViewModel> antennas)
        {
            if (reader == null || !antennas.Any()) return;

            Connection(reader, antennas);
        }

        public void Disconnect()
        {
            if (_impinjReader == null || !_impinjReader.IsConnected) return;

            StopRead();
            _impinjReader.Disconnect();
            UpdateConnectionStatus();
        }

        private void Connection(RfidReaderEntityViewModel newReader, List<RfidAntennaEntityViewModel> antennas)
        {
            try
            {
                Disconnect();

                _impinjReader.Connect(newReader.ReaderIp);
                _impinjReader.Stop();
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

            UpdateConnectionStatus();
            if (!_impinjReader.IsConnected) return;

            SetSettings(newReader.TagPopulation);
            SetAntennaSettings(antennas);
            _impinjReader.ApplySettings(_settings);
        }

        private void SetSettings(ushort tagPopulation)
        {
            _settings = _impinjReader.QueryDefaultSettings();

            _settings.Report.IncludeAntennaPortNumber = true;
            _settings.Report.IncludePhaseAngle = true;
            _settings.Report.IncludeChannel = true;
            _settings.Report.IncludeDopplerFrequency = true;
            _settings.Report.IncludeFastId = true;
            _settings.Report.IncludeFirstSeenTime = true;
            _settings.Report.IncludeLastSeenTime = true;
            _settings.Report.IncludePeakRssi = true;
            _settings.Report.IncludeSeenCount = true;
            _settings.Report.IncludePcBits = true;
            _settings.Report.IncludeSeenCount = true;

            //ReaderMode.AutoSetDenseReaderDeepScan | Rx = -70 | Tx = 15/20
            //ReaderMode.MaxThrouput | Rx = -80 | Tx = 15

            _settings.ReaderMode = ReaderMode.AutoSetDenseReaderDeepScan;//.AutoSetDenseReader;
            _settings.SearchMode = SearchMode.DualTarget;//.DualTarget;
            _settings.Session = 1;
            _settings.TagPopulationEstimate = tagPopulation;
            _settings.Report.Mode = ReportMode.Individual;
        }

        private void SetAntennaSettings(List<RfidAntennaEntityViewModel> antennas)
        {
            _settings.Antennas.DisableAll();
            foreach (var antenna in antennas)
            {
                _settings.Antennas.GetAntenna((ushort)antenna.AntennaNumb).IsEnabled = true;
                _settings.Antennas.GetAntenna((ushort)antenna.AntennaNumb).TxPowerInDbm = antenna.TxPower;
                _settings.Antennas.GetAntenna((ushort)antenna.AntennaNumb).RxSensitivityInDbm = antenna.RxSensitivity;
            }
        }

        public virtual void StartStopRead()
        {
            if (!_impinjReader.IsConnected) return;

            if (!IsReading)
            {
                StartRead();
            }
            else
            {
                StopRead();
            }
        }

        public virtual void StartRead()
        {
            IsReading = true;
            _impinjReader.TagsReported += DisplayTag;
            _impinjReader.Start();
        }

        public virtual void StopRead()
        {
            IsReading = false;
            _impinjReader.Stop();
            _impinjReader.TagsReported -= DisplayTag;
        }

        private void UpdateStartStopContent()
        {
            StartStopContent = IsReading ? "Stop" : "Start";
        }
 
        private void UpdateShowAddContent()
        {
            if (SelectedTag == null) return;

            AddShowContent = SelectedTag.IsRegistered ? "Show" : "Add";
        }

        public void UpdateConnectionStatus()
        {
            if (_impinjReader == null)
            {
                ConnectionStatus = "Error";
            }
            else if(_impinjReader.IsConnected)
            {
                ConnectionStatus = "Connected";
            }
            else if(!_impinjReader.IsConnected)
            {
                ConnectionStatus = "Disconnected";
            }
        }

        #endregion


        #region Tag Manipulation

        private void DisplayTag(ImpinjReader reader, TagReport report)
        {
            _data = new ConcurrentDictionary<string, int>();

            foreach (Tag tag in report)
            {
                AddData(tag.Epc.ToString(), tag.AntennaPortNumber);
            }

            SetTagViewModels();
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

        public void SetTagViewModels()
        {
            var dataTags = _data;

            Tags = new ObservableCollection<RfidTagViewModel>();

            foreach (var data in dataTags)
            {
                var tag = new RfidTagViewModel()
                {
                    Tag = data.Key,
                    Antenna = data.Value,
                };

                CheckTagRegistration(tag);
                Tags.Add(tag);
            }
        }

        private void CheckTagRegistration(RfidTagViewModel tag)
        {
            tag.IsRegistered = Linens.Any(x => Equals(x.Tag, tag.Tag));
        }

        public void CheckAllTagRegistration()
        {
            Tags?.ForEach(CheckTagRegistration);
        }

        public void SetLinens(ObservableCollection<ClientLinenEntityViewModel> linens)
        {
            Linens = linens;

            CheckAllTagRegistration();
        }

        #endregion

        private void RfidReaderWindow()
        {
            var window = _resolverService.Resolve<RfidReaderWindowModel>();

            var showDialog = _dialogService.ShowDialog(window);
        }

        public void AddShowLinenTag()
        {
            if (SelectedTag.IsRegistered)
            {
                var linen = Linens.FirstOrDefault(x => x.Tag == SelectedTag.Tag);
                if (linen == null) return;
                _dialogService.ShowInfoDialog("Linen Window");
                //LinenWindow(linen);
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(SelectedLinen.Tag))
                {
                    if (!_dialogService.ShowQuestionDialog(
                        $"{SelectedLinen.OriginalObject.MasterLinenEntity.Name} already has Tag \"{SelectedLinen.Tag}\" \n Do you want to replace Tag?")
                    ) return;
                }
                SelectedLinen.Tag = SelectedTag.Tag;
                UseTag();
            }
        }

        private void RemoveTag()
        {
            if(!SelectedTag.IsRegistered) return;

            var linen = Linens.FirstOrDefault(x => String.Equals(x.Tag, SelectedTag.Tag));

            if (linen == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to Remove \"{SelectedTag.Tag}\" from {linen.OriginalObject?.MasterLinenEntity?.Name} ?"))
                return;

            linen.Tag = null;
            Save(linen);
        }

        private void UseTag()
        {
            if (SelectedLinen == null || SelectedTag == null) return;

            SelectedLinen.Tag = SelectedTag.Tag;
            SelectedTag.IsRegistered = true;
            SaveSelectedLinen();
        }

        private void SaveSelectedLinen()
        {
            Save(SelectedLinen);
        }

        private void Save(ClientLinenEntityViewModel linen)
        {
            if (!linen.HasChanges()) return;

            linen.AcceptChanges();
            _laundryService.AddOrUpdateAsync(linen.OriginalObject);

            CheckAllTagRegistration();
            UpdateShowAddContent();
        }
    }
}
