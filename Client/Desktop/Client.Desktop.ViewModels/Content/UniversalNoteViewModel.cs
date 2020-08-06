using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Content
{
    public class UniversalNoteViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private bool _showAllLinen;
        private ObservableCollection<DeliveryNoteEntityViewModel> _deliveryNotes;
        private DeliveryNoteEntityViewModel _selectedDeliveryNote;
        private ObservableCollection<DeliveryNoteRowEntityViewModel> _deliveryNoteRows;

        public ObservableCollection<DeliveryNoteRowEntityViewModel> DeliveryNoteRows
        {
            get => _deliveryNoteRows;
            set => Set(ref _deliveryNoteRows, value);
        }
        public DeliveryNoteEntityViewModel SelectedDeliveryNote
        {
            get => _selectedDeliveryNote;
            set => Set(ref _selectedDeliveryNote, value);
        }
        public ObservableCollection<DeliveryNoteEntityViewModel> DeliveryNotes
        {
            get => _deliveryNotes;
            set => Set(ref _deliveryNotes, value);
        }
        public bool ShowAllLinen
        {
            get => _showAllLinen;
            set => Set(ref _showAllLinen, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(ref _clientLinens, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs =>
            Departments?.Where(x => x.ParentId != null && x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => Departments
            ?.Where(x => x.ClientId == SelectedClient?.Id && x.ParentId == null).ToObservableCollection();

        public ObservableCollection<DeliveryNoteEntityViewModel> SortedNoteHeaders => SortNoteHeaders();
        public ObservableCollection<DeliveryNoteRowEntityViewModel> SortedNoteRows => SortNoteRows();

        public RelayCommand InitializeCommand { get; }
        public RelayCommand NewNoteCommand { get; }
        public RelayCommand EditNoteCommand { get; }

        public UniversalNoteViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            InitializeCommand = new RelayCommand(Initialize);
            //NewNoteCommand = new RelayCommand(DeliveryNoteWindow(null), ()=> SelectedDepartment != null);
            //EditNoteCommand = new RelayCommand(DeliveryNoteWindow(SelectedDeliveryNote), ()=> SelectedDeliveryNote != null);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(()=> SortedDepartments);
            }else            
            
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
                NewNoteCommand.RaiseCanExecuteChanged();
            }
            else

            if (e.PropertyName == nameof(SelectedDeliveryNote))
            {
                RaisePropertyChanged(() => SortedNoteRows);
                EditNoteCommand.RaiseCanExecuteChanged();
            }

        }

        private async void Initialize()
        {
            Clients = await _laundryService.Clients();
            Departments = await _laundryService.Departments();
            MasterLinens = await _laundryService.MasterLinens();

            GetClientLinens();
        }

        private async void GetClientLinens()
        {
            ClientLinens = await _laundryService.ClientLinens();
        }

        private ObservableCollection<DeliveryNoteEntityViewModel> SortNoteHeaders()
        {
            var notes = new ObservableCollection<DeliveryNoteEntityViewModel>();
            if (SelectedDepartment == null) return notes;

            notes.AddRange(DeliveryNotes.Where(x=> x.DepartmentId == SelectedDepartment.Id));

            var staffs = Departments.Where(x => x.ParentId == SelectedDepartment.Id);
            foreach (var staff in staffs)
            {
                notes.AddRange(DeliveryNotes.Where(x => x.DepartmentId == staff.Id));
            }

            return notes;
        }

        private ObservableCollection<DeliveryNoteRowEntityViewModel> SortNoteRows()
        {
            var rows = new ObservableCollection<DeliveryNoteRowEntityViewModel>();
            if (SelectedDeliveryNote == null) return rows;

            rows = DeliveryNoteRows.Where(x => x.DeliveryNoteId == SelectedDeliveryNote.Id).ToObservableCollection();

            return rows;
        }

        private void DeliveryNoteWindow(DeliveryNoteEntityViewModel item)
        {
            //var window = _resolverService.Resolve<DeliveryNoteWindowModel>();

            //window.Clients = Clients;
            //window.Departments = Departments;

            //window.SetSelectedLinen(item);

            //if (_dialogService.ShowDialog(window))
            //{

            //}
        }


    }
}
