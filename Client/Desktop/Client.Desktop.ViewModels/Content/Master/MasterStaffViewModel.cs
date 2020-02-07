using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
        private readonly IMainDispatcher _dispatcher;

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

        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _tagData =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

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
        public RelayCommand GetReaderTagsCommand { get; }

        public RelayCommand AddSelectedTagCommand { get; }
        

        public MasterStaffViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            AddStaffCommand = new RelayCommand(AddStaff, (() => SelectedDepartment != null));
            DeleteStaffCommand = new RelayCommand(DeleteStaff, (() => SelectedStaff != null));

            AddLinenCommand = new RelayCommand(AddLinen, (() => SelectedDepartment != null));
            DeleteLinenCommand = new RelayCommand(DeleteLinen, (() => SelectedLinen != null));

            RfidReaderCommand = new RelayCommand(RfidReader);
            GetReaderTagsCommand = new RelayCommand(GetReaderTags);
            AddSelectedTagCommand = new RelayCommand(AddSelectedTag, (() => SelectedTag != null));

            Task.Factory.StartNew( () => GetData());

            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

            Tags = new ObservableCollection<Tuple<int, string>>();
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
            
            if(e.PropertyName == nameof(_tagData))
            {
                UpdateTags();
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

            foreach (var item in staffs)
            {
                item.AcceptChanges();

                _laundryService.AddOrUpdate(item.OriginalObject);
            }

            foreach (var item in linens)
            {
                item.AcceptChanges();

                _laundryService.AddOrUpdate(item.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes saved");
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

        private void GetReaderTags()
        {
            Tags = RfidReaderWindow.Tags;
        }

        private void AddSelectedTag()
        {
            if (SelectedLinen == null)
                AddLinen();

            if (SelectedLinen.Tag != null)
            {
                if (!_dialogService.ShowQuestionDialog("Linen already has tag \n Do you want to replace it?"))
                {
                    return;
                }
            }

            if (Linens.Any(x => Equals(x.Tag, SelectedTag.Item2)))
            {
                var existLinen = Linens.FirstOrDefault(x => Equals(x.Tag, SelectedTag.Item2));
                var staff = Staff.FirstOrDefault(x => x.Id == existLinen.StaffId);

                if (!_dialogService.ShowQuestionDialog(
                    $"Tag {SelectedTag.Item2} already using by {staff.StaffName} in <{existLinen.Id}> linen \n Do you want to shift Tag?")
                ) return;

                existLinen.Tag = null;
            }

            SelectedLinen.Tag = SelectedTag.Item2;

        }

        private void UpdateTags()
        {
            if(_tagData.Count == 0) return;
            var tags = new List<string>();


            foreach (var antenna in _tagData)
            { 
                tags.AddRange(antenna.Value.Select(x=> x.Key));
            }

            foreach (var tag in tags)
            {
                if (Tags.Any(x => Equals(x.Item2, tag)))
                {
                    continue;
                }

                Tags.Add(new Tuple<int, string>(1, tag));
            }

        }
        
    }
}
