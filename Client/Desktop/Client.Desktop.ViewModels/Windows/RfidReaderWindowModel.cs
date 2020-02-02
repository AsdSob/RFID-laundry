using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class RfidReaderWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;

        public Action<bool> CloseAction { get; set; }


        private ObservableCollection<RfidReaderEntityViewModel> _rfidReaders;
        private RfidReaderEntityViewModel _selectedRfidReader;
        private ObservableCollection<RfidAntennaEntityViewModel> _rfidAntennas;
        private RfidService _readerService;

        public RfidService ReaderService
        {
            get => _readerService;
            set => Set(() => ReaderService, ref _readerService, value);
        }
        public ObservableCollection<RfidAntennaEntityViewModel> RfidAntennas
        {
            get => _rfidAntennas;
            set => Set(() => RfidAntennas, ref _rfidAntennas, value);
        }
        public RfidReaderEntityViewModel SelectedRfidReader
        {
            get => _selectedRfidReader;
            set => Set(() => SelectedRfidReader, ref _selectedRfidReader, value);
        }
        public ObservableCollection<RfidReaderEntityViewModel> RfidReaders
        {
            get => _rfidReaders;
            set => Set(() => RfidReaders, ref _rfidReaders, value);
        }

        public ObservableCollection<RfidAntennaEntityViewModel> SortedAntennas => GetReaderAntennas();

        public RelayCommand SaveCommand { get; }
        public RelayCommand SelectReaderCommand { get; }
        public RelayCommand DeleteReaderCommand { get; }
        public RelayCommand AddReaderCommand { get; }
        public RelayCommand CloseCommand { get; }

        public RfidReaderWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            ReaderService = new RfidService();

            SaveCommand = new RelayCommand(Save);
            SelectReaderCommand = new RelayCommand(Close);
            AddReaderCommand = new RelayCommand(AddReader);
            CloseCommand = new RelayCommand(Close);
            DeleteReaderCommand = new RelayCommand(DeleteReader, () => SelectedRfidReader != null);
            
            GetData();
        }

        private async Task GetData()
        {
            _dialogService.ShowBusy();

            try
            {
                var reader = await _laundryService.GetAllAsync<RfidReaderEntity>();
                var readers = reader.Select(x => new RfidReaderEntityViewModel(x));
                RfidReaders = readers.ToObservableCollection();

                var antenna = await _laundryService.GetAllAsync<RfidAntennaEntity>();
                var antennas = antenna.Select(x => new RfidAntennaEntityViewModel(x));
                RfidAntennas = antennas.ToObservableCollection();
            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedRfidReader))
            {
                RaisePropertyChanged(()=> SortedAntennas);
                DeleteReaderCommand.RaiseCanExecuteChanged();
            }
        }

        public void ConnectReader()
        {
            if(SelectedRfidReader == null) return;

            ReaderService.StopRead();

            var isConnected = ReaderService.Connection(SelectedRfidReader, SortedAntennas.ToList());

            _dialogService.ShowInfoDialog(isConnected
                ? $"{SelectedRfidReader.Name} Is Connected"
                : $"{SelectedRfidReader.Name} Is NOT Connected");
        }

        private ObservableCollection<RfidAntennaEntityViewModel> GetReaderAntennas()
        {
            var antennas = new ObservableCollection<RfidAntennaEntityViewModel>();
            if (SelectedRfidReader == null) return antennas;

            antennas.AddRange(SelectedRfidReader.IsNew
                ? RfidAntennas.Where(x => x.OriginalObject.RfidReaderEntity == SelectedRfidReader.OriginalObject)
                : RfidAntennas.Where(x => x.RfidReaderId == SelectedRfidReader.Id));

            if (antennas.Count != 4)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if(antennas.Any(x=> x.AntennaNumb == i))continue;

                    var newAntenna = new RfidAntennaEntityViewModel()
                    {
                        AntennaNumb = i,
                        RxSensitivity = 0,
                        TxPower = 0,
                    };

                    if (SelectedRfidReader.IsNew)
                    {
                        newAntenna.OriginalObject.RfidReaderEntity = SelectedRfidReader.OriginalObject;
                    }
                    else
                    {
                        newAntenna.RfidReaderId = SelectedRfidReader.Id;
                    }

                    antennas.Add(newAntenna);
                }
            }
            return antennas;
        }

        private void Save()
        {
            var readers = RfidReaders.Where(x => x.HasChanges());
            var antennas = RfidAntennas.Where(x => x.HasChanges());

            foreach (var item in readers)
            {
                item.AcceptChanges();

                _laundryService.AddOrUpdate(item.OriginalObject);
            }

            foreach (var item in antennas)
            {
                item.AcceptChanges();

                _laundryService.AddOrUpdate(item.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes saved");
        }

        private void AddReader()
        {
            var reader = new RfidReaderEntityViewModel()
            {
                ReaderIp = "192.168.0.0",
                ReaderPort = 4001,
                TagPopulation = 200,
            };

            AddReaderAntennas(reader);

            RfidReaders.Add(reader);
            SelectedRfidReader = reader;
        }

        private void AddReaderAntennas(RfidReaderEntityViewModel reader)
        {
            var antennas = new ObservableCollection<RfidAntennaEntityViewModel>();

            for (int i = 1; i <= 4; i++)
            {
                var antenna = new RfidAntennaEntityViewModel()
                {
                    AntennaNumb = i,
                    RxSensitivity = 0,
                    TxPower = 0,
                };

                antenna.OriginalObject.RfidReaderEntity = reader.OriginalObject;
                antennas.Add(antenna);
            }

            RfidAntennas.AddRange(antennas);
        }

        private void DeleteReader()
        {
            var reader = SelectedRfidReader;
            var antennas = SortedAntennas.Where(x=> !x.IsNew);

            if(!_dialogService.ShowQuestionDialog($"Do you want to DELETE {reader.Name} ?")) return;

            _laundryService.Delete(reader.OriginalObject);
            _laundryService.Delete(antennas.Select(x=> x.OriginalObject));

            antennas.ForEach(x=> RfidAntennas.Remove(x));
            RfidReaders.Remove(reader);

            SelectedRfidReader = RfidReaders.FirstOrDefault();
        }

        public RfidReaderEntityViewModel GetSelectedReader()
        {
            return SelectedRfidReader;
        }

        private void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
            {
                ConnectReader();
                CloseAction?.Invoke(true);
            }
        }

    }
}
