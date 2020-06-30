using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Impinj.OctaneSdk;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Services
{
    public class BinRfidReaderViewModel : ViewModelBase
    {
        private readonly LaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _dispatcher;

        public Action<bool> CloseAction { get; set; }

        private ObservableCollection<RfidReaderEntityViewModel> _rfidReaders;
        private RfidReaderEntityViewModel _selectedRfidReader;
        private ObservableCollection<RfidAntennaEntityViewModel> _rfidAntennas;
        private RfidService _readerService;
        private ObservableCollection<RfidTagViewModel> _tags;

        public ObservableCollection<RfidTagViewModel> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
        }
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

        public BinRfidReaderViewModel(LaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            //TODO: get account and BinLocation entities
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            //ReaderService = new RfidService();

            Tags = new ObservableCollection<RfidTagViewModel>();

            Initialize();
        }

        private async Task Initialize()
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
            //Start();

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedRfidReader))
            {

            }
        }

        //public void Start()
        //{
        //    //TODO: Get Reader and Client from locationBinSettings
        //    SelectedRfidReader = RfidReaders.FirstOrDefault();


        //    if (SelectedRfidReader == null) return;
        //    var antennas = RfidAntennas.Where(x => x.RfidReaderId == SelectedRfidReader.Id).ToList();

        //    ReaderService.Connection(SelectedRfidReader, antennas);

        //    ReaderService.StartRead();
        //    ReaderService.Reader.TagsReported += DisplayTag;
        //}

        //private void DisplayTag(ImpinjReader reader, TagReport report)
        //{
        //    foreach (Tag tag in report)
        //    {
        //        if (Tags.Any(x => Equals(x.Tag, tag.Epc.ToString())))
        //        {
        //            continue;
        //        }

        //        _dispatcher.RunInMainThread((() =>
        //        {
        //            Tags.Add(new RfidTagViewModel()
        //            {
        //                Tag = tag.Epc.ToString(),
        //                Antenna = tag.AntennaPortNumber
        //            });
        //        }));
        //    }
        //}
    }
}
