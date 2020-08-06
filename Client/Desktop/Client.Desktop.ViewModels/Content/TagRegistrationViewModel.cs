using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Services;
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
        private RfidTagViewModel _selectedTag;
        private RfidServiceTest _reader;
        private ClientEntityViewModel _selectedClient;
        private DepartmentEntityViewModel _selectedDepartment;
        private DepartmentEntityViewModel _selectedStaff;

        public DepartmentEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(ref _selectedStaff, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public RfidServiceTest Reader
        {
            get => _reader;
            set => Set(ref _reader, value);
        }
        public RfidTagViewModel SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
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
        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartments();
        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortLinens();

        public RelayCommand NewLinenCommand { get; }
        public RelayCommand EditLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }

        public RelayCommand InitializeCommand { get; }

        public TagRegistrationViewModel(RfidServiceTest reader, ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));

            NewLinenCommand = new RelayCommand(() => LinenWindow(null));
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen),() => SelectedClientLinen !=null);
            DeleteLinenCommand = new RelayCommand(DeleteLinen,(() => SelectedClientLinen !=null));

            InitializeCommand = new RelayCommand(Initialize);

            //TODO: Set SelectedReader base on Software registration account

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(()=> SortedDepartments);
                RaisePropertyChanged(() => SortedLinens);
            }
            else             
            
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(()=> SortedStaffs);
                RaisePropertyChanged(() => SortedLinens);
            }
            else            
            
            if (e.PropertyName == nameof(SelectedStaff))
            {
                RaisePropertyChanged(()=> SortedLinens);
            }else 

            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                DeleteLinenCommand?.RaiseCanExecuteChanged();
                EditLinenCommand?.RaiseCanExecuteChanged();
                Reader.SelectedLinen = SelectedClientLinen;
            }
        }

        private async void Initialize()
        {
            Clients = await _laundryService.Clients();
            Departments = await _laundryService.Departments();
            MasterLinens = await _laundryService.MasterLinens();

            await GetClientLinens();

            RaisePropertyChanged(() => SortedLinens);
        }

        private async Task GetClientLinens()
        {
            Linens = await _laundryService.ClientLinens();
            Reader.SetLinens(Linens);
        }

        private ObservableCollection<DepartmentEntityViewModel> SortDepartments()
        {
            var departments = new ObservableCollection<DepartmentEntityViewModel>();

            if (SelectedClient != null)
            {
                departments = Departments.Where(x => x.ParentId == null && x.ClientId == SelectedClient.Id).ToObservableCollection();
            }

            SelectedDepartment = departments.FirstOrDefault();
            return departments;
        }

        private ObservableCollection<DepartmentEntityViewModel> SortStaffs()
        {
            var staffs = new ObservableCollection<DepartmentEntityViewModel>();

            if (SelectedClient == null)
            {
               staffs = Departments?.Where(x => x.ParentId != null).ToObservableCollection();
            }
            else
            {
                if (SelectedDepartment == null)
                {
                    staffs = Departments?.Where(x => x.ParentId != null && x.ClientId == SelectedClient?.Id)
                        .ToObservableCollection();
                }
                else
                {
                    staffs = Departments?.Where(x => x.ParentId != null && x.ParentId == SelectedDepartment?.Id).ToObservableCollection();
                }
            }

            SelectedStaff = staffs?.FirstOrDefault();
            return staffs;
        }          
        
        private ObservableCollection<ClientLinenEntityViewModel> SortLinens()
        {
            ObservableCollection<ClientLinenEntityViewModel> linens;

            if (SelectedClient == null)
            {
                linens = Linens;
            }
            else
            {
                if (SelectedDepartment == null)
                {
                    linens = Linens.Where(x => x.ClientId == SelectedClient.Id).ToObservableCollection();
                }
                else
                {
                    linens = SelectedStaff == null
                        ? Linens.Where(x => x.DepartmentId == SelectedDepartment.Id).ToObservableCollection()
                        : Linens.Where(x => x.StaffId == SelectedStaff.Id).ToObservableCollection();
                }
            }
            return linens;
        }        


        private void LinenWindow(ClientLinenEntityViewModel linen)
        {
            var linenWindow = _resolverService.Resolve<ClientLinenWindowModel>();

            linenWindow.ClientLinens = Linens;

            linenWindow.SetItem(linen);

            if (_dialogService.ShowDialog(linenWindow))
            {
                GetClientLinens();
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
        }

    }
}
