using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.Notes.ViewModel.Window;
using PALMS.Reports.Common;
using PALMS.Reports.Model.NoteReports;
using PALMS.Reports.Model.Notes;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    public class EditNoteViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public int NoteStatus => 0;
        public string Name => "Edit Notes";

        private readonly NoteCommonMethods _commonMethods;
        private IDispatcher _dispatcher;
        private IDataService _dataService;
        private IDialogService _dialogService;
        private readonly IReportService _reportService;
        private readonly IResolver _resolverService;
        private ObservableCollection<Client> _clients;
        private Client _selectedClient;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private List<DepartmentViewModel> _departments;
        private NoteHeaderViewModel _selectedNoteHeader;
        private ObservableCollection<UnitViewModel> _serviceTypes;
        private List<UnitViewModel> _deliveryTypes;
        private PrimeInfo _primeInfo;
        private List<NoteHeaderViewModel> _selectedNoteHeaders;
        private EnumerationExtension.EnumerationMember _selectedReportType;
        private bool _showAllNotes;
        private List<UnitViewModel> _noteStatuses;

        public DateTime PrintTime { get; set; }
        public List<UnitViewModel> NoteStatuses
        {
            get => _noteStatuses;
            set => Set(ref _noteStatuses, value);
        }
        public bool ShowAllNotes
        {
            get => _showAllNotes;
            set => Set(ref _showAllNotes, value);
        }
        public EnumerationExtension.EnumerationMember SelectedReportType
        {
            get => _selectedReportType;
            set => Set(ref _selectedReportType, value);
        }
        public List<NoteHeaderViewModel> SelectedNoteHeaders
        {
            get => _selectedNoteHeaders;
            set => Set(ref _selectedNoteHeaders, value);
        }
        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public ObservableCollection<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public List<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public ObservableCollection<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRows();

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => SortNoteHeaders();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelChanges { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand PrintDepartmentCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand NoteLinenCommand { get; }

        public EditNoteViewModel(NoteCommonMethods commonMethods, IDialogService dialogService, IDispatcher dispatcher, IDataService dataService, IReportService reportService, IResolver resolverService)
        {
            _commonMethods = commonMethods ?? throw new ArgumentNullException(nameof(commonMethods));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _resolverService = resolverService ?? throw new ArgumentNullException(nameof(resolverService));

            SaveCommand = new RelayCommand(Save, () => SelectedNoteHeader != null);
            CancelChanges = new RelayCommand(Clear, () => SelectedNoteHeader != null);
            PrintCommand = new RelayCommand(Print, () => SelectedNoteHeader != null);
            RemoveCommand = new RelayCommand(Remove, () => SelectedNoteHeader != null);
            PrintDepartmentCommand = new RelayCommand(PrintDepartmentReport);
            NoteLinenCommand = new RelayCommand(NoteLinen);

            PrintTime = DateTime.Now;
            ShowAllNotes = false;
            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            _dispatcher.RunInMainThread(() =>
            {
                Clients = _commonMethods.Clients.ToObservableCollection();
                ServiceTypes = _commonMethods.WashTypes.ToObservableCollection();
                DeliveryTypes = _commonMethods.DeliveryTypes;
                NoteHeaders = _commonMethods.NoteHeaders;
                Departments = _commonMethods.Departments.ToList();
                PrimeInfo = _commonMethods.PrimeInfo;
                NoteStatuses = _commonMethods.NoteStatus;

                NoteRows = _commonMethods.NoteRows;
                NoteRows.ForEach(SubscribeItem);
                NoteRows.CollectionChanged += NoteRowsCollectionChanged;
            });

            SelectedNoteHeaders = new List<NoteHeaderViewModel>();

            await Task.CompletedTask;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                RaisePropertyChanged(() => SortedNoteRows);
                CancelChanges.RaiseCanExecuteChanged();
                PrintCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
            if (e.PropertyName == nameof(SelectedReportType))
            {
                PrintDepartmentReport();
            }

            if (e.PropertyName == nameof(ShowAllNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
        }

        private void UnSubscribeItem(NoteRowViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(NoteRowViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is NoteRowViewModel item)) return;

            if (item.OriginalObject.ServiceTypeId != item.ServiceTypeId)
            {
                var row = _commonMethods.AddNoteRowPriceWeight(item);

                item.Price = row.Price;
            }
        }

        private void NoteRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<NoteRowViewModel>())
                {
                    SubscribeItem(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<NoteRowViewModel>())
                {
                    UnSubscribeItem(item);
                }
            }
        }

        private bool HasChanges()
        {
            if (SelectedNoteHeader == null || SortedNoteHeaders == null)
            {
                return false;
            }

            return SelectedNoteHeader.HasChanges() || SortedNoteRows.Any(x=> x.HasChanges());
        }

        public ObservableCollection<NoteHeaderViewModel> SortNoteHeaders()
        {
            var noteHeaders = new ObservableCollection<NoteHeaderViewModel>();

            if (ShowAllNotes)
            {
                noteHeaders = NoteHeaders?.Where(x => x.ClientId == SelectedClient?.Id).OrderByDescending(x=>x.Id).ToObservableCollection();
                return noteHeaders;
            }

            noteHeaders = NoteHeaders?.Where(x => x.ClientId == SelectedClient?.Id &&  x.NoteStatus == (int)NoteStatusEnum.PreInvoice).OrderByDescending(x => x.Id).ToObservableCollection();
            return noteHeaders;
        }

        public ObservableCollection<NoteRowViewModel> SortNoteRows()
        {
            return NoteRows?.Where(x => x.NoteHeaderId == SelectedNoteHeader?.Id).OrderBy(x => x.LinenName).ToObservableCollection();
        }

        public void Clear()
        {
            if (!_dialogService.ShowQuestionDialog(" Do you want to cancel all changes in note ?"))
                return;

            SelectedNoteHeader?.Reset();

            SortedNoteRows?.ForEach(x=> x.Reset());
        }

        public void Save()
        {
            if (HasChanges() && !_dialogService.ShowQuestionDialog(" Do you want to save all notes ?"))
                return;
            if(SortedNoteRows.Any(x=> x.HasChanges()))
            AddPriceWeightToRows(SortedNoteRows);

            var noteRows = SortedNoteRows.Where(x=> x.NoteHeaderId == SelectedNoteHeader.Id).ToObservableCollection();
            var noteHeader = SelectedNoteHeader;

            _commonMethods.Save(noteHeader, noteRows);
        }

        private async void Remove()
        {
            if (SelectedNoteHeader != null && SortedNoteRows.Count != 0 &&
                !_dialogService.ShowQuestionDialog($" Do you want to DELETE note {SelectedNoteHeader.Name} ?"))
                return;

            var noteRows = SortedNoteRows.Where(x => x.NoteHeaderId == SelectedNoteHeader.Id).ToObservableCollection();
            var noteHeader = SelectedNoteHeader;

            foreach (var noteRow in noteRows)
            {
                await _dataService.DeleteAsync(noteRow.OriginalObject);
            }

            await _dataService.DeleteAsync(noteHeader?.OriginalObject);

            NoteHeaders.Remove(SelectedNoteHeader);
            SortedNoteRows.Clear();

            RaisePropertyChanged(() => SortedNoteHeaders);
        }

        private void AddPriceWeightToRows(ObservableCollection<NoteRowViewModel> noteRows)
        {
            if (noteRows.Count == 0)
                return;

            foreach (var noteRow in noteRows)
            {
                if(!noteRow.HasChanges()) continue;

                var row = _commonMethods.AddNoteRowPriceWeight(noteRow);

                noteRow.Price = row.Price;
                noteRow.Weight = row.Weight;
            }
        }

        public void SetSelectedNoteHeaders()
        {
            SelectedNoteHeaders = new List<NoteHeaderViewModel>();

            foreach (var noteHeader in SortedNoteHeaders.Where(x => x.IsSelected))
            {
                SelectedNoteHeaders.Add(noteHeader);
            }
        }

        private bool GetShowPrice()
        {
            var showPrice = false;

            if (SelectedClient.ClientInfo != null)
                switch (SelectedClient.ClientInfo.NoteId)
                {
                    case (int)NoteTypeEnum.WithPrice:
                        showPrice = true;
                        break;
                }
            return showPrice;
        }

        public async void NoteLinen()
        {
            var noteLinenReplacement = _resolverService.Resolve<NoteLinenReplacementViewModel>();
            await noteLinenReplacement.InitializeAsync();

            var showDialog = _dialogService.ShowDialog(noteLinenReplacement);

            if (!showDialog) return;
        }

        private void Print()
        {
            if (SelectedNoteHeader == null)
                return;

            var report = GetNoteReport(GetShowPrice());

            if (report == null) return;

            _reportService.ShowReportPreview(report);
        }

        private IReport GetNoteReport(bool priceVisibility)
        {
            var deliveryType = DeliveryTypes.FirstOrDefault(x => x.Id == SelectedNoteHeader?.DeliveryTypeId)?.Name;
            var departmentName = Departments.FirstOrDefault(x => x.Id == SelectedNoteHeader.DepartmentId).Name;
            var report = new NoteReport();
            {
                report.ClientAddress = SelectedClient.ClientInfo.Address;
                report.ClientName = SelectedClient.Name;
                report.Comment = SelectedNoteHeader.Comment;
                report.DeliveryDate = SelectedNoteHeader.DeliveryDate;
                report.DeliveryKg = SelectedNoteHeader.DeliveryWeight;
                report.DeliveryType = deliveryType;
                report.DepartmentName = departmentName;
                report.NoteNumber = SelectedNoteHeader.Name;
                report.CollectionDate = SelectedNoteHeader.CollectionDate;
                report.CollectionWeight = SelectedNoteHeader.CollectionWeight;
                report.ClientLogo = Extension.GetBitmap(Extension.GetBitmapImage(SelectedClient.ClientInfo.Logo));
                report.PrimeLogo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo));
                report.ShowItemPrice = priceVisibility;
            }

            foreach (var notRow in SortedNoteRows)
            {
                var serviceType = ServiceTypes.FirstOrDefault(x => x.Id == notRow.ServiceTypeId)?.Name;
                var item = new NotesReportRowItem()
                {
                    Id = notRow.LinenListId,
                    Name = notRow.LinenName,
                    Price = notRow.Price,
                    DeliveryQuantity = notRow.PrimeDeliveredQty,
                    CollectionQuantity = notRow.PrimeCollectedQty,
                    ClientReceivedQuantity = notRow.ClientReceivedQty,
                    Remark = notRow.Comment,
                    ServiceType = serviceType,
                };

                if (report.ShowItemPrice)
                {
                    switch (SelectedNoteHeader.DeliveryTypeId)
                    {
                        case (int)DeliveryTypeEnum.Express:
                            item.Price += item.Price * SelectedNoteHeader.ExpressCharge;
                            break;
                        case (int)DeliveryTypeEnum.ReWash:
                            item.Price = 0;
                            break;
                    }
                }

                report.ItemRows.Add(item);
            }

            return report;
        }

        private void PrintDepartmentReport()
        {
            SetSelectedNoteHeaders();

            if (SelectedNoteHeaders.Count == 0)
                return;

            var report = GetDepartmentReport();
            if (report == null) return;

            _reportService.ShowReportPreview(report);
        }

        private IReport GetDepartmentReport()
        {
            if (SelectedReportType == null)
                return null;

            var reportType = SelectedReportType;

            var report = ReportBase(GetShowPrice());

            switch (reportType.Value)
            {
                case NoteReportTypesEnum.ByTotalPiece:
                    report = GetReportByTotal(report);
                    return report;

                case NoteReportTypesEnum.ByDepartment:
                    report = GetReportByDepartment(report);
                    return report;

                case NoteReportTypesEnum.ByLinen:
                    report = GetReportByLinen(report);
                    return report;

                default: return null;
            }

            
        }

        private IReport ReportHeader(bool priceVisible)
        {
            var report = new NotesReportBase()
            {
                ClientAddress = SelectedClient.ClientInfo.Address,
                ClientName = SelectedClient.Name,
                ClientLogo = Extension.GetBitmap(Extension.GetBitmapImage(SelectedClient.ClientInfo.Logo)),
                PrimeLogo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo)),
                PrintDate = PrintTime,
                ShowItemPrice = priceVisible,
            };

            var departments = new List<Department>();
            foreach (var noteHeader in SelectedNoteHeaders)
            {
                var department = departments.Find(x => x.Id == noteHeader.DepartmentId);

                if (department == null)
                {
                    departments.Add(Departments.FirstOrDefault(x => x.Id == noteHeader.DepartmentId)?.OriginalObject);
                }
            }

            var departmentsParents = new List<Department>();
            foreach (var department in departments)
            {
                if (department.ParentId == null)
                {
                    departmentsParents.Add(department);
                    continue;
                }
                var departmentParent = departmentsParents.Find(x => x.Id == department.ParentId);

                if (departmentParent == null)
                {
                    departmentsParents.Add(Departments.Find(x => x.Id == department.ParentId)?.OriginalObject);
                }
            }


            if (departmentsParents.Count == 1)
            {
                report.DepartmentName = departmentsParents.FirstOrDefault()?.Name;
            }
            else if (departmentsParents.Count > 1)
            {
                foreach (var department in departments)
                {
                    report.DepartmentName += department.Name + ", ";
                }
            }

            return report;
        }

        private IReport ReportBase(bool priceVisible)
        {
            var report = (NotesReportBase)ReportHeader(priceVisible);

            foreach (var noteHeader in SelectedNoteHeaders)
            {
                
                var itemNote = new NotesReportNoteItem()
                {
                    Items = new List<NotesReportRowItem>(),
                    CollectionNoteWeight = noteHeader.CollectionWeight,
                    DeliveryNoteWeight = noteHeader.DeliveryWeight,
                    Name = noteHeader.Name,
                    Id = noteHeader.Id,
                    DepartmentId = noteHeader.Id,
                    DepartmentName = noteHeader.Name,
                    DelivereddDate = noteHeader.DeliveryDate,
                    CollectionDate = noteHeader.CollectionDate,
                };

                foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id))
                {
                    var item = new NotesReportRowItem()
                    {
                        CollectionQuantity = noteRow.PrimeCollectedQty,
                        DeliveryQuantity = noteRow.PrimeDeliveredQty,
                        ClientReceivedQuantity = noteRow.ClientReceivedQty,
                        Id = noteRow.LinenListId,
                        Name = noteRow.LinenName,
                        ServiceType = ServiceTypes.FirstOrDefault(x => x.Id == noteRow.ServiceTypeId)?.Name,
                        Remark = noteRow.Comment,
                        Price = noteRow.Price,
                    };

                    if (report.ShowItemPrice)
                    {
                        switch (noteHeader.DeliveryTypeId)
                        {
                            case (int)DeliveryTypeEnum.Express:
                                item.Price += item.Price * noteHeader.ExpressCharge;
                                break;
                            case (int)DeliveryTypeEnum.ReWash:
                                item.Price = 0;
                                break;
                        }
                    }
                    itemNote.Items.Add(item);
                }
                report.Items.Add(itemNote);
            }
            return report;
        }

        private IReport GetReportByLinen(IReport baseReport)
        {
            var reportBase = (NotesReportBase)baseReport;

            var report = new NoteByLinenReport(reportBase);

            var byLinenReportItems = new List<NotesReportRowItem>();

            foreach (var reportNoteItem in reportBase.Items)
            {
                report.DeliveryKg += reportNoteItem.DeliveryNoteWeight;
                report.CollectionWeight += reportNoteItem.CollectionNoteWeight;
                report.NoteNumber += reportNoteItem.Name + "\r";

                foreach (var itemRow in reportNoteItem.Items)
                {
                    var price = 0;

                    if (itemRow.ClientReceivedQuantity != null)
                    {
                        price = (int) (itemRow.Price * itemRow.ClientReceivedQuantity);
                    }

                    var item = byLinenReportItems.FirstOrDefault(x => x.Id == itemRow.Id);

                    if (item == null)
                    {
                        byLinenReportItems.Add(new NotesReportRowItem()
                        {
                            Id = itemRow.Id,
                            Name = itemRow.Name,
                            DepartmentName = reportNoteItem.DepartmentName,
                            DeliveryQuantity = itemRow.DeliveryQuantity,
                            CollectionQuantity = itemRow.CollectionQuantity,
                            ClientReceivedQuantity = itemRow.ClientReceivedQuantity,
                            Price = price
                        });
                        continue;
                    }

                    foreach (var rowReport in byLinenReportItems.Where(x => x.Id == itemRow.Id))
                    {
                        rowReport.DeliveryQuantity += itemRow.DeliveryQuantity;
                        rowReport.CollectionQuantity += itemRow.CollectionQuantity;
                        rowReport.ClientReceivedQuantity += itemRow.ClientReceivedQuantity;
                        rowReport.Price += price;
                        break;
                    }
                }
            }
            report.ItemRows = byLinenReportItems;
            return report;
        }

        private IReport GetReportByTotal(IReport baseReport)
        {
            var reportBase = (NotesReportBase)baseReport;
            var report = new NoteByTotalReport(reportBase) { Items = new List<NotesReportNoteItem>() };

            foreach (var noteHeader in reportBase.Items)
            {
                report.DeliveryKg += noteHeader.DeliveryNoteWeight;
                report.CollectionWeight += noteHeader.CollectionNoteWeight;

                var item = new NotesReportNoteItem()
                {
                    Name = noteHeader.Name,
                    DepartmentName = noteHeader.DepartmentName,
                    DepartmentId = noteHeader.DepartmentId,
                    Id = noteHeader.Id,
                };

                foreach (var noteRow in noteHeader.Items)
                {
                    item.CollectionQuantity += noteRow.CollectionQuantity;
                    item.DeliveryQuantity += noteRow.DeliveryQuantity;
                    item.ClientReceivedQuantity += noteRow.ClientReceivedQuantity;

                    if (noteRow.ClientReceivedQuantity != null)
                        item.Price += noteRow.Price * (double) noteRow.ClientReceivedQuantity;
                }
                report.Items.Add(item);
            }
            return report;
        }

        private IReport GetReportByDepartment(IReport baseReport)
        {
            var reportBase = (NotesReportBase)baseReport;
            var report = new NoteByNoteReport(reportBase);

            foreach (var noteHeader in reportBase.Items)
            {
                report.Items.Add(noteHeader);

                foreach (var itemRow in noteHeader.Items)
                {
                    itemRow.NoteId = noteHeader.Id;
                    report.ReportNoteRows.Add(itemRow);
                }
            }
            return report;
        }
    }
}
