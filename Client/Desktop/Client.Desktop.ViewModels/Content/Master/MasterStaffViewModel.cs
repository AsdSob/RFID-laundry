using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Impinj.OctaneSdk;
using Storage.Laundry.Models;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Content.Master
{
    public class MasterStaffViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;
        private List<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private List<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<StaffEntityViewModel> _staff;
        private StaffEntityViewModel _selectedStaff;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<LinenEntityViewModel> _linens;
        private LinenEntityViewModel _selectedLinen;
        private ObservableCollection<Tuple<int, string>> _tags;
        private Tuple<int, string> _selectedTag;

        public RfidReaderWindowModel RfidReaderWindow { get; set; }

        public Tuple<int,string> SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public ObservableCollection<Tuple<int, string>> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
        }
        public LinenEntityViewModel SelectedLinen
        {
            get => _selectedLinen;
            set => Set(() => SelectedLinen, ref _selectedLinen, value);
        }
        public ObservableCollection<LinenEntityViewModel> Linens
        {
            get => _linens;
            set => Set(() => Linens, ref _linens, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public StaffEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public ObservableCollection<StaffEntityViewModel> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public List<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public List<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<StaffEntityViewModel> SortedStaff =>
            Staff?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

        public ObservableCollection<LinenEntityViewModel> SortedLinens => SortLinen();

        public RelayCommand SaveCommand { get; }
        public RelayCommand AddStaffCommand { get; }
        public RelayCommand DeleteStaffCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }
        public RelayCommand RfidReaderCommand { get; }

        public RelayCommand StartReadingCommand { get; }
        public RelayCommand StopReadingCommand { get; }
        public RelayCommand AddSelectedTagCommand { get; }


        public MasterStaffViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            SaveCommand = new RelayCommand(Save);
            AddStaffCommand = new RelayCommand(AddStaff, (() => SelectedDepartment != null));
            DeleteStaffCommand = new RelayCommand(DeleteStaff, (() => SelectedStaff != null));

            AddLinenCommand = new RelayCommand(AddLinen, (() => SelectedDepartment != null));
            DeleteLinenCommand = new RelayCommand(DeleteLinen, (() => SelectedLinen != null));

            RfidReaderCommand = new RelayCommand(RfidReader);
            StartReadingCommand = new RelayCommand(StartReading);
            StopReadingCommand = new RelayCommand(StartReading);
            AddSelectedTagCommand = new RelayCommand(AddSelectedTag, (() => SelectedTag != null));

            Task.Factory.StartNew( () => GetData());

            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();
        }


        private async Task GetData()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                var clients = client.Select(x => new ClientEntityViewModel(x));
                Clients = clients.ToList();

                var department = await _laundryService.GetAllAsync<DepartmentEntity>();
                var departments = department.Select(x => new DepartmentEntityViewModel(x));
                Departments = departments.ToList();

                var master = await _laundryService.GetAllAsync<MasterLinenEntity>();
                var masters = master.Select(x => new MasterLinenEntityViewModel(x));
                MasterLinens = masters.ToObservableCollection();

                var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
                var linens = linen.Select(x => new LinenEntityViewModel(x));
                Linens = new ObservableCollection<LinenEntityViewModel>();
                Linens = linens.ToObservableCollection();

                var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
                var staffs = staff.Select(x => new StaffEntityViewModel(x));
                Staff = staffs.ToObservableCollection();

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

                DeleteStaffCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedLinen))
            {
                DeleteLinenCommand.RaiseCanExecuteChanged();
            }

            if(e.PropertyName == nameof(SelectedTag))
            {
                AddSelectedTagCommand.RaiseCanExecuteChanged();
            }

        }

        private ObservableCollection<LinenEntityViewModel> SortLinen()
        {
            var linens = new ObservableCollection<LinenEntityViewModel>();

            if (SelectedStaff == null)
            {
                if (SelectedDepartment == null) return linens;

                return Linens?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
            }

            linens = Linens.Where(x => x.OriginalObject.ClientStaffEntity == SelectedStaff.OriginalObject).ToObservableCollection();
            return linens;
        }

        private void Save()
        {
            var staffs = Staff.Where(x => x.HasChanges());
            var linens = Linens.Where(x => x.HasChanges());

        }

        private void SaveEntity<T>(T entity) where T : EntityBase
        {
            _laundryService.AddOrUpdate(entity);
        }

        private void DeleteEntity<T>(T entity) where T : EntityBase
        {
            //TODO: Delete Entity
        }

        private void AddStaff()
        {
            if(SelectedDepartment == null) return;
            if (!_dialogService.ShowQuestionDialog("Do you want to add new Staff?"))return;

            var newStaff = new StaffEntityViewModel()
            {
                DepartmentId = SelectedDepartment.Id,
                PhoneNumber = "+971",
            };


            Staff.Add(newStaff);

            RaisePropertyChanged(()=> SortedStaff);
            SelectedStaff = newStaff;
        }

        private void DeleteStaff()
        {
            var staff = SelectedStaff;

            if (staff == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {staff.StaffName}?")) return;

            DeleteEntity(staff.OriginalObject);
            Staff.Remove(staff);

            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {staff.StaffName} linens also?")) return;

            var linens = Linens.Where(x => x.StaffId == staff.Id).ToList();

            foreach (var linen in linens)
            {
                DeleteEntity(linen.OriginalObject);
                Linens.Remove(linen);
            }

        }

        private void AddLinen()
        {
            if (SelectedDepartment == null) return;
            if (!_dialogService.ShowQuestionDialog("Do you want to add new Linen?")) return;

            var newLinen = new LinenEntityViewModel()
            {
                ClientId =  SelectedClient.Id,
                DepartmentId = SelectedDepartment.Id,
                MasterLinenId = 1,
                StatusId = (int) LinenStatusEnum.InUse,
            };

            if (SelectedStaff != null)
            {
                newLinen.OriginalObject.ClientStaffEntity = SelectedStaff.OriginalObject;
            }

            Linens.Add(newLinen);

            RaisePropertyChanged(()=> SortedLinens);
            SelectedLinen = newLinen;
        }

        private void DeleteLinen()
        {
            var linen = SelectedLinen;

            if (linen == null) return;
            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to DELETE {MasterLinens?.FirstOrDefault(x => x.Id == SelectedLinen?.MasterLinenId)?.Name}?")
            ) return;

            DeleteEntity(linen.OriginalObject);
            Linens.Remove(linen);

            RaisePropertyChanged(()=> SortedLinens);
        }


        private void RfidReader()
        {
            RfidReaderWindow.ReaderService.StopRead();

            var showDialog = _dialogService.ShowDialog(RfidReaderWindow);
            
            
        }

        private void StartReading()
        {
            RfidReaderWindow.ReaderService.StartRead();

            RfidReaderWindow.ReaderService.Reader.TagsReported += SHowAntennaTags;
        }

        private void StopReading()
        {
            RfidReaderWindow.ReaderService.StopRead();

        }

        private void AddSelectedTag()
        {
            RfidReaderWindow.ReaderService.StopRead();

            RfidReaderWindow.ReaderService.Reader.TagsReported -= SHowAntennaTags;

        }

        public async void SHowAntennaTags(ImpinjReader reader, TagReport report)
        {
            foreach (var tag in report.Tags)
            {
                if (Tags.Any(x => Equals(x.Item2, tag.Epc.ToString()) && Equals(x.Item1, tag.AntennaPortNumber)))
                {
                    continue;
                }

                Tags.Add(new Tuple<int, string>(tag.AntennaPortNumber, tag.Epc.ToString()));
            }
        }
    }
}
