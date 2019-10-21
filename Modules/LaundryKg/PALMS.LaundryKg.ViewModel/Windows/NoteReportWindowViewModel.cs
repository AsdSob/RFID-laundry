using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.Invoices.ViewModel.Windows;
using PALMS.Reports.Common;
using PALMS.Reports.Model.ReportTypes;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using ClientViewModel = PALMS.LaundryKg.ViewModel.EntityViewModel.ClientViewModel;


namespace PALMS.LaundryKg.ViewModel.Windows
{
    public class NoteReportWindowViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IExcelReportService _excelReportService;
        private readonly ICanExecuteMediator _canExecuteMediator;
        private readonly IDialogService _dialogService;
        private readonly IDispatcher _dispatcher;
        private readonly IResolver _resolverService;
        private readonly IDataService _dataService;
        private readonly IReportService _reportService;

        public Action<bool> CloseAction { get; set; }

        private List<NoteHeader> _noteHeaders;
        private List<ClientViewModel> _clients;
        private List<Department> _departments;
        private List<LinenList> _linenLists;
        private DateTime _dateEnd;
        private DateTime _dateStart;
        private Department _selectedDepartment;
        private List<UnitViewModel> _serviceTypes;
        private ClientViewModel _selectedClient;
        private List<UnitViewModel> _noteStatuses;
        private NoteHeader _selectedNoteHeader;
        private List<NoteRow> _noteRows;
        private bool _showDeliveryNotes;
        private bool _showClientNotes;
        private bool _showPreInvoiceNotes;
        private bool _showInvoiceNotes;

        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public NoteHeader SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public List<UnitViewModel> NoteStatuses
        {
            get => _noteStatuses;
            set => Set(ref _noteStatuses, value);
        }
        public ClientViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public DateTime DateEnd
        {
            get => _dateEnd;
            set => Set(ref _dateEnd, value);
        }
        public DateTime DateStart
        {
            get => _dateStart;
            set => Set(ref _dateStart, value);
        }
        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<ClientViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public List<NoteHeader> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }

        public bool ShowDeliveryNotes
        {
            get => _showDeliveryNotes;
            set => Set(ref _showDeliveryNotes, value);
        }

        public bool ShowClientNotes
        {
            get => _showClientNotes;
            set => Set(ref _showClientNotes, value);
        }

        public bool ShowPreInvoiceNotes
        {
            get => _showPreInvoiceNotes;
            set => Set(ref _showPreInvoiceNotes, value);
        }

        public bool ShowInvoiceNotes
        {
            get => _showInvoiceNotes;
            set => Set(ref _showInvoiceNotes, value);
        }


        public List<Department> SortedDepartments => Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToList();
        public List<NoteHeader> SortedNoteHeaders => SortNoteHeaders();

        public RelayCommand LinenReportCommand { get; }
        public RelayCommand NoteStatusReportCommand { get; }
        public RelayCommand NoteEditCommand { get; }

        public async Task InitializeAsync()
        {
            DateEnd = DateTime.Now;
            DateStart = DateTime.Now;

            var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
            var clients = client.Select(x => new ClientViewModel(x));
            _dispatcher.RunInMainThread(() => Clients = clients.OrderBy(x => x.ShortName).ToList());

            var department = await _dataService.GetAsync<Department>();
            _dispatcher.RunInMainThread(() => Departments = department.ToList());

            var linens = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
            var linenList = linens;
            _dispatcher.RunInMainThread(() => LinenLists = linenList);

            GetNotes();

            ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
            NoteStatuses = EnumExtentions.GetValues<NoteStatusEnum>();
        }


        public async void GetNotes()
        {
            try
            {
                var noteHeader = await _dataService.GetAsync<NoteHeader>();
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeader.ToList());

                var noteRow = await _dataService.GetAsync<NoteRow>();
                _dispatcher.RunInMainThread(() => NoteRows = noteRow.ToList());
            }

            catch (Exception ex)
            {
                _dialogService.HideBusy();
                Helper.RunInMainThread(() => _dialogService.ShowErrorDialog($"Initialization error: {ex.Message}"));
            }

            finally
            {
                _dialogService.HideBusy();
            }
        }

        public NoteReportWindowViewModel(ICanExecuteMediator canExecuteMediator,
            IResolver resolver,
            IDataService dataService,
            IDispatcher dispatcher,
            IDialogService dialogService,
            IExcelReportService excelReportService,
            IReportService reportService)
        {
            _excelReportService = excelReportService ?? throw new ArgumentNullException(nameof(excelReportService));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            LinenReportCommand = new RelayCommand(LinenReport);
            NoteStatusReportCommand = new RelayCommand(GetNoteStatusReport);
            NoteEditCommand = new RelayCommand(NoteEdit);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(DateStart))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(DateEnd))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(ShowInvoiceNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(ShowClientNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(ShowDeliveryNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(ShowPreInvoiceNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
        }

        public List<ClientViewModel> GetSelectedClients()
        {
            var clients = Clients?.Where(x => x.IsSelected).ToList();

            return clients;
        }

        public List<NoteHeader> GetSelectedHeaders()
        {
            var noteHeaders = new List<NoteHeader>();
            var clients = GetSelectedClients();

            if (clients == null || clients.Count == 0)
            {
                noteHeaders.AddRange(NoteHeaders.Where(x => x.ClientId == SelectedClient.Id));
            }
            else
            {
                foreach (var client in clients)
                {
                    noteHeaders.AddRange(NoteHeaders.Where(x => x.ClientId == client.Id));
                }
            }

            return noteHeaders;
        }

        public List<NoteHeader> SortNoteHeaders()
        {
            if (NoteHeaders == null) return null;
            var start = GetStartRange();
            var end = GetEndRange();
            var note = GetSelectedHeaders();
            var notes =note.Where(x => x.DeliveredDate >= start && x.DeliveredDate <= end ).ToList();

            if (notes == null || notes.Count == 0) return null;
            var noteHeaders = new List<NoteHeader>();

            if (ShowClientNotes)
            {
                noteHeaders.AddRange(notes.Where(x => x.NoteStatus == (int)NoteStatusEnum.ClientNote));
            }

            if (ShowPreInvoiceNotes)
            {
                noteHeaders.AddRange(notes.Where(x => x.NoteStatus == (int)NoteStatusEnum.PreInvoice));
            }

            if (ShowInvoiceNotes)
            {
                noteHeaders.AddRange(notes.Where(x => x.NoteStatus == (int)NoteStatusEnum.Invoiced));
            }

            if (ShowDeliveryNotes)
            {
                noteHeaders.AddRange(notes.Where(x => x.NoteStatus == (int)NoteStatusEnum.DeliveryNote));
            }

            return noteHeaders;
        }

        public DateTime GetStartRange()
        {
            var start = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, DateTime.MinValue.Hour,
                DateTime.MinValue.Minute, DateTime.MinValue.Second);

            return start;
        }

        public DateTime GetEndRange()
        {
            var end = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, DateTime.MaxValue.Hour,
                DateTime.MaxValue.Minute, DateTime.MaxValue.Second);

            return end;
        }

        private void LinenReport()
        {
            if (SelectedClient == null || SelectedDepartment == null)
            {
                _dialogService.ShowInfoDialog("Please select client/department");
                return;
            }
            var startDate = GetStartRange();
            var endDate = GetEndRange();

            var notes = NoteHeaders.Where(x => x.DepartmentId == SelectedDepartment.Id &&
                                               x.DeliveredDate > startDate &&
                                               x.DeliveredDate < endDate).ToList();
            if (notes.Count == 0)
            {
                _dialogService.ShowInfoDialog("Number of notes = 0");
                return;
            }

            var linenReport = new LinenReportViewModel()
            {
                ClientName = SelectedClient.Name,
                DepartmentName = SelectedDepartment.Name,
                DateEnd = DateEnd,
                DateStart = DateStart,
                CollectionWeight = 0,
                DeliveryWeight = 0,
                Items = new List<LinenReportItem>(),
            };

            var items = new List<LinenReportItem>();

            foreach (var noteHeader in notes)
            {
                linenReport.CollectionWeight = +noteHeader.CollectionWeight;
                linenReport.DeliveryWeight = +noteHeader.CollectionWeight;

                foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id))
                {
                    if (items.Any(x => x.LinenId == noteRow.LinenListId && x.ServiceId == noteRow.ServiceTypeId))
                    {
                        var linen = items.FirstOrDefault(x =>
                            x.LinenId == noteRow.LinenListId && x.ServiceId == noteRow.ServiceTypeId);

                        linen.ClientReceivedQty += noteRow.ClientReceivedQty;
                        linen.DeliveryQty += noteRow.PrimeDeliveryQty;
                        linen.CollectionQty += noteRow.PrimeCollectionQty;
                    }
                    else
                    {
                        var linen = new LinenReportItem()
                        {
                            LinenName = LinenLists.FirstOrDefault(x => x.Id == noteRow.LinenListId)?.MasterLinen.Name,
                            LinenId = noteRow.LinenListId,
                            ServiceId = noteRow.ServiceTypeId,
                            ServiceName = ServiceTypes.FirstOrDefault(x => x.Id == noteRow.ServiceTypeId)?.Name,
                            ClientReceivedQty = noteRow.ClientReceivedQty,
                            CollectionQty = noteRow.PrimeCollectionQty,
                            DeliveryQty = noteRow.PrimeDeliveryQty,
                        };
                        items.Add(linen);
                    }
                }
            }

            foreach (var item in items)
            {
                item.DifferenceCollectionDelivery = item.DeliveryQty - item.CollectionQty;
                item.DifferenceCollectionClientReceive = item.ClientReceivedQty - item.CollectionQty;
            }

            linenReport.Items = items.OrderBy(x => x.LinenName).ToList();
            _reportService.ShowReportPreview(linenReport);
        }

        #region Note Status Report

        public void GetNoteStatusReport()
        {
            var notes = SortNoteHeaders();

            var statuses = "";
            if (ShowDeliveryNotes)
            {
                statuses += "Delivery Note";
            }
            if (ShowClientNotes)
            {
                statuses += ", Client Note";
            }
            if (ShowPreInvoiceNotes)
            {
                statuses += ", Pre-Invoice Note";
            }
            if (ShowInvoiceNotes)
            {
                statuses += ", Invoice Note";
            }

            var linenReport = new NoteByStatusReportViewModel()
            {
                Items = new List<NoteByStatusReporItem>(),
                NoteStatuses = statuses,
                EndDate = GetEndRange(),
                StartDate = GetStartRange(),
            };

            var items = new List<NoteByStatusReporItem>();
            foreach (var note in notes)
            {
                var item = new NoteByStatusReporItem
                {
                    Department = Departments.FirstOrDefault(x => x.Id == note.DepartmentId)?.Name,
                    Client = Clients.FirstOrDefault(x => x.Id == note.ClientId)?.ShortName,
                    CollectionDate = note.CollectionDate,
                    DeliveryDate = note.DeliveredDate,
                    NoteName = note.Name,
                    NoteStatus = NoteStatuses.FirstOrDefault(x => x.Id == note.NoteStatus)?.Name,
                };

                items.Add(item);
            }

            linenReport.Items = items.OrderBy(x => x.Client).ToList();
            _reportService.ShowReportPreview(linenReport);
        }

        #endregion


        public async void NoteEdit()
        {
            if (SelectedNoteHeader == null)
                return;

            var note = SelectedNoteHeader;

            var noteEditViewModel = _resolverService.Resolve<NoteEditViewModel>();

            await noteEditViewModel.InitializeAsync(new NoteHeaderViewModel(note));
            var showDialog = _dialogService.ShowDialog(noteEditViewModel);

            if (showDialog)
            {
                GetNotes();
            }
        }

    }
}
