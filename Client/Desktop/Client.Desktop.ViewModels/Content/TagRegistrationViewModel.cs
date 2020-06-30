using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.ViewModels.Content
{
    public class TagRegistrationViewModel :ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private ObservableCollection<ClientLinenEntityViewModel> _linens;
        private ClientLinenEntityViewModel _selectedClientLinen;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private string _addShowButton;
        private RfidTagViewModel _selectedTag;
        private ObservableCollection<RfidTagViewModel> _tags;
        private ObservableCollection<RfidReaderEntityViewModel> _readers;
        private ObservableCollection<RfidAntennaEntityViewModel> _antennas;
        private RfidReaderEntityViewModel _selectedReader;
        private string _startStopString;
        public RfidServiceTest RfidService { get; set; }

        public string StartStopString
        {
            get => _startStopString;
            set => Set(ref _startStopString, value);
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
        public ObservableCollection<RfidTagViewModel> Tags
        {
            get => _tags;
            set => Set(ref _tags, value);
        }
        public RfidTagViewModel SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }

        public string AddShowButton
        {
            get => _addShowButton;
            set => Set(ref _addShowButton, value);
        }

        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public ClientLinenEntityViewModel SelectedClientLinen
        {
            get => _selectedClientLinen;
            set => Set(() => SelectedClientLinen, ref _selectedClientLinen, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> Linens
        {
            get => _linens;
            set => Set(() => Linens, ref _linens, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs => SortStaffs();

        public RelayCommand NewLinenCommand { get; }
        public RelayCommand EditLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }

        public RelayCommand InitializeCommand { get; }

        public RelayCommand StartStopCommand { get; }
        public RelayCommand AddShowLinenByTagCommand { get; }
        public RelayCommand DeleteTagCommand { get; }
        public RelayCommand ReaderSettingsWindowCommand { get; }


        public TagRegistrationViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));


            NewLinenCommand = new RelayCommand(() => LinenWindow(null));
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen),() => SelectedClientLinen !=null);
            DeleteLinenCommand = new RelayCommand(DeleteLinen,(() => SelectedClientLinen !=null));

            StartStopCommand = new RelayCommand(StartStopRead, () => SelectedReader != null);
            AddShowLinenByTagCommand = new RelayCommand(AddShowLinenByTag, () => SelectedTag != null);
            DeleteTagCommand = new RelayCommand(DeleteTag, () => SelectedTag != null || SelectedClientLinen != null);

            ReaderSettingsWindowCommand = new RelayCommand(ReaderSettingsWindow);


            InitializeCommand = new RelayCommand(Initialize);
            RfidService = _resolverService.Resolve<RfidServiceTest>();

            AddShowButton = "Add";
            StartStopString = RfidService.GetStartStopString();

            PropertyChanged += OnPropertyChanged;
            RfidService.SortedDataEvent += TagsCollectionChanged;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                DeleteLinenCommand?.RaiseCanExecuteChanged();
                EditLinenCommand?.RaiseCanExecuteChanged();
                DeleteTagCommand?.RaiseCanExecuteChanged();
            }
            else

            if (e.PropertyName == nameof(SelectedTag))
            {
                AddShowLinenByTagCommand?.RaiseCanExecuteChanged();
                DeleteTagCommand?.RaiseCanExecuteChanged();
                AddShowButtonName();
            }            
            
            if (e.PropertyName == nameof(SelectedReader))
            {
                StartStopCommand?.RaiseCanExecuteChanged();
                SetReader();

                //TODO: Set SelectedReader base on Software registration account
            }
        }

        private async void Initialize()
        {
            Clients = await _laundryService.Clients();
            Departments = await _laundryService.Departments();
            MasterLinens = await _laundryService.MasterLinens();

            RaisePropertyChanged(()=> SortedStaffs);

            GetClientLinens();
            GetRfidReaders();
        }

        private async void GetClientLinens()
        {
            Linens = await _laundryService.ClientLinens();
        }
        private async void GetRfidReaders()
        {
            Readers = await _laundryService.RfidReaders();
            Antennas = await _laundryService.RfidAntennas();
        }

        private ObservableCollection<DepartmentEntityViewModel> SortStaffs()
        {
            return Departments.Where(x => x.ParentId != null).ToObservableCollection();
        }

        private void TagsCollectionChanged(ConcurrentDictionary<string, int> dataTags)
        {
            SetTagViewModels(dataTags);
        }

        private void SetTagViewModels(ConcurrentDictionary<string, int> dataTags)
        {
            Tags = new ObservableCollection<RfidTagViewModel>();

            foreach (var data in dataTags)
            {
                var tag = new RfidTagViewModel()
                {
                    Tag = data.Key,
                    Antenna = data.Value,
                };

                SetTagLinenRegistration(tag);

                Tags.Add(tag);
            }
        }

        private void SetTagLinenRegistration(RfidTagViewModel tag)
        {
            tag.IsRegistered = Linens.Any(x => Equals(x.Tag, tag.Tag));
        }

        private void CheckAllTagRegistration()
        {
            if(Tags == null) return;

            foreach (var tag in Tags)
            {
                SetTagLinenRegistration(tag);
            }
        }

        private void SetReader()
        {
            var antennas = Antennas.Where(x => x.RfidReaderId == SelectedReader?.Id).ToList();
            RfidService.Connection(SelectedReader, antennas);
        }

        private void StartStopRead()
        {
            if (SelectedReader == null) return;

            RfidService.StartStopRead();
            StartStopString = RfidService.GetStartStopString();
        }

        private void AddShowButtonName()
        {
            if(SelectedTag == null) return;

            AddShowButton = SelectedTag.IsRegistered ? "Show" : "Add";
        }

        private void AddShowLinenByTag()
        {
            if (SelectedTag.IsRegistered)
            {
                var linen = Linens.FirstOrDefault(x => x.Tag == SelectedTag.Tag);
                if (linen == null) return;
                LinenWindow(linen);
            }
            else
            {
                UseTag();
                CheckAllTagRegistration();
            }
        }

        private void DeleteTag()
        {
            var linen = Linens.FirstOrDefault(x => String.Equals(x.Tag, SelectedTag.Tag));
            if (linen == null)
            {
                return;
            }

            if (!_dialogService.ShowQuestionDialog($"Do you want to Delete \"{SelectedTag.Tag}\" ?"))
            {
                return;
            }

            linen.Tag = null;
            linen.AcceptChanges();
            CheckAllTagRegistration();

            _laundryService.AddOrUpdateAsync(linen.OriginalObject);
            AddShowButtonName();
        }

        private void UseTag()
        {
            if(SelectedClientLinen ==null || SelectedTag == null) return;

            SelectedClientLinen.Tag = SelectedTag.Tag;
            SelectedClientLinen.AcceptChanges();
            SelectedTag.IsRegistered = true;

            _laundryService.AddOrUpdateAsync(SelectedClientLinen.OriginalObject);
            AddShowButtonName();
        }

        private void LinenWindow(ClientLinenEntityViewModel linen)
        {
            var linenWindow = _resolverService.Resolve<ClientLinenWindowModel>();

            linenWindow.Clients = Clients;
            linenWindow.Departments = Departments;
            linenWindow.MasterLinens = MasterLinens;
            linenWindow.ClientLinens = Linens;

            linenWindow.SetSelectedLinen(linen);

            if (_dialogService.ShowDialog(linenWindow))
            {
                GetClientLinens();
                CheckAllTagRegistration();
            }
        }

        private async void DeleteLinen()
        {
            var masterLinen = MasterLinens.FirstOrDefault(x => x.Id == SelectedClientLinen.MasterLinenId);

            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to DELETE {masterLinen?.Name} ?"))
                return;

            await _laundryService.DeleteAsync(SelectedClientLinen.OriginalObject);

            Linens.Remove(SelectedClientLinen);
            CheckAllTagRegistration();
        }

        private void ReaderSettingsWindow()
        {
            var window = _resolverService.Resolve<RfidReaderWindowModel>();

            if (_dialogService.ShowDialog(window))
            {
                GetRfidReaders();
            }
        }

    }
}
