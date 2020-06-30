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
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class MasterStaffViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<ClientStaffEntityViewModel> _staff;
        private ClientStaffEntityViewModel _selectedStaff;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _linens;
        private ClientLinenEntityViewModel _selectedClientLinen;

        private string _addShowButton;

        public string AddShowButton
        {
            get => _addShowButton;
            set => Set(ref _addShowButton, value);
        }
        public RfidReaderWindowModel RfidReaderWindow { get; set; }

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
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public ClientStaffEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public ObservableCollection<ClientStaffEntityViewModel> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<ClientStaffEntityViewModel> SortedStaff => SortStaff();

        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortLinen();

        public RelayCommand AddStaffCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand RfidReaderCommand { get; }
        public RelayCommand EditStaffCommand { get; }
        public RelayCommand EditLinenCommand { get; }

        public RelayCommand AddShowLinenByTagCommand { get; }
        public RelayCommand DeleteTagCommand { get; }

        public RelayCommand InitializeCommand { get; }
        

        public MasterStaffViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddStaffCommand = new RelayCommand(AddNewStaff, (() => SelectedDepartment != null));
            EditStaffCommand = new RelayCommand(EditStaff, (() => SelectedStaff != null));

            AddLinenCommand = new RelayCommand(() => LinenWindow(null));
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen), () => SelectedClientLinen != null);

            InitializeCommand = new RelayCommand(Initialize);

            RfidReaderCommand = new RelayCommand(RfidReader);
            AddShowLinenByTagCommand = new RelayCommand(AddShowLinenByTag, () => SelectedTag != null);
            DeleteTagCommand = new RelayCommand(DeleteTag, () => SelectedTag != null || SelectedClientLinen != null);

            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

            AddShowButton = "Add";
            //RfidReaderWindow.Tags.CollectionChanged += TagsCollectionChanged;


        }

        //private void TagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var tag in RfidReaderWindow.Tags)
        //        {
        //            if (Linens.Any(x => x.Tag == tag.Tag))
        //            {
        //                tag.IsRegistered = true;
        //            }
        //        }
        //    }
        //}

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                var clients = client.Select(x => new ClientEntityViewModel(x));
                Clients = clients.ToObservableCollection();

                var department = await _laundryService.GetAllAsync<DepartmentEntity>();
                var departments = department.Select(x => new DepartmentEntityViewModel(x));
                Departments = departments.ToObservableCollection();

                var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
                var masterLinens = masterLinen.Select(x => new MasterLinenEntityViewModel(x));
                MasterLinens = masterLinens.ToObservableCollection();

                GetStaffs();
                GetClientLinens();
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

        private async void GetStaffs()
        {
            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            Staff = staffs.ToObservableCollection();
        }

        private async void GetClientLinens()
        {
            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            Linens = linens.ToObservableCollection();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
                SelectedDepartment = SortedDepartments.FirstOrDefault();
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(()=> SortedStaff);
                RaisePropertyChanged(() => SortedLinens);

                AddStaffCommand.RaiseCanExecuteChanged();
                AddLinenCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                RaisePropertyChanged(() => SortedLinens);
                EditStaffCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                EditLinenCommand.RaiseCanExecuteChanged();
                DeleteTagCommand.RaiseCanExecuteChanged();
            }

            if(e.PropertyName == nameof(SelectedTag))
            {
                AddShowLinenByTagCommand.RaiseCanExecuteChanged();
                DeleteTagCommand.RaiseCanExecuteChanged();
                AddShowButtonName();
            }
        }

        private ObservableCollection<ClientLinenEntityViewModel> SortLinen()
        {
            var linens = new ObservableCollection<ClientLinenEntityViewModel>();

            if (Linens == null) return linens;

            if (SelectedDepartment == null)
            {
                linens = Linens.Where(x=> x.ClientId == SelectedClient?.Id).ToObservableCollection();
            }
            else
            {
                linens = Linens.Where(x=> x.DepartmentId == SelectedDepartment.Id).ToObservableCollection();
            }

            return linens;
        }

        private ObservableCollection<ClientStaffEntityViewModel> SortStaff()
        {
            var staffs = new ObservableCollection<ClientStaffEntityViewModel>();

            if (SelectedDepartment == null)
            {
                if (SortedDepartments == null)
                    return staffs;

                foreach (var department in SortedDepartments)
                {
                    staffs.AddRange(Staff?.Where(x => x.DepartmentId == department?.Id));
                }
            }
            else
            {
                staffs.AddRange(Staff?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection());
            }

            return staffs;
        }

        private void Save()
        {
            var linens = Linens.Where(x => x.HasChanges());

            foreach (var item in linens)
            {
                item.AcceptChanges();

                _laundryService.AddOrUpdateAsync(item.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes saved");
        }

        private void EditStaff()
        {
            StaffWindow(SelectedStaff);
        }

        private void AddNewStaff()
        {
            StaffWindow(null);
        }

        private void StaffWindow(ClientStaffEntityViewModel staff)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            staffWindow.SetSelectedStaff(staff, SelectedDepartment);
            staffWindow.Staffs = Staff;

            if (_dialogService.ShowDialog(staffWindow))
            {
                Staff.Clear();
                GetStaffs();
                RaisePropertyChanged((() => SortedStaff));
            }
        }

        private void LinenWindow(ClientLinenEntityViewModel linen)
        {
            var linenWindow = _resolverService.Resolve<ClientLinenWindowModel>();

            linenWindow.Clients = Clients;
            linenWindow.Departments = Departments;
            linenWindow.Staffs = Staff;
            linenWindow.MasterLinens = MasterLinens;
            linenWindow.ClientLinens = Linens;

            if (linen == null)
            {
                linen = new ClientLinenEntityViewModel()
                {
                    Tag = SelectedTag?.Tag,
                };
            }

            linenWindow.SetSelectedLinen(linen);

            if (_dialogService.ShowDialog(linenWindow))
            {
                GetClientLinens();
                CheckTags();
            }
        }


        private void RfidReader()
        {
            //RfidReaderWindow.ReaderService.StopRead();

            var showDialog = _dialogService.ShowDialog(RfidReaderWindow);
        }

        private void AddShowButtonName()
        {
            if (SelectedTag == null) return;

            AddShowButton = SelectedTag.IsRegistered ? "Show" : "Add";
        }

        private void CheckTags()
        {
            //foreach (var tag in RfidReaderWindow.Tags)
            //{
            //    tag.IsRegistered = Linens.Any(x => Equals(x.Tag, tag.Tag));
            //}
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
                CheckTags();
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
            CheckTags();

            _laundryService.AddOrUpdateAsync(linen.OriginalObject);
        }

        private void UseTag()
        {
            if (SelectedClientLinen == null || SelectedTag == null) return;

            SelectedClientLinen.Tag = SelectedTag.Tag;
            SelectedClientLinen.AcceptChanges();
            SelectedTag.IsRegistered = true;

            _laundryService.AddOrUpdateAsync(SelectedClientLinen.OriginalObject);
        }


        #region RFID Part

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
        private async void GetRfidReaders()
        {
            var reader = await _laundryService.GetAllAsync<RfidReaderEntity>();
            var readers = reader.Select(x => new RfidReaderEntityViewModel(x));
            Readers = readers.ToObservableCollection();

            var antenna = await _laundryService.GetAllAsync<RfidAntennaEntity>();
            var antennas = antenna.Select(x => new RfidAntennaEntityViewModel(x));
            Antennas = antennas.ToObservableCollection();
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
            foreach (var tag in Tags)
            {
                SetTagLinenRegistration(tag);
            }
        }

        private void SetReader()
        {
            var antennas = Antennas.Where(x => x.RfidReaderId == SelectedReader.Id).ToList();
            RfidService.Connection(SelectedReader, antennas);
        }

        private void StartStopRead()
        {
            if (SelectedReader == null) return;

            RfidService.StartStopRead();
            StartStopString = RfidService.GetStartStopString();
        }
        #endregion


    }
}
