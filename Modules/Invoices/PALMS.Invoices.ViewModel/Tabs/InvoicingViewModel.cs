using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Data.Objects.Payment;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.Invoices.ViewModel.Service;
using PALMS.Invoices.ViewModel.Windows;
using PALMS.Reports.Common;
using PALMS.Reports.Model.Invoice;
using PALMS.View.Common.Converters;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Invoices.ViewModel.Tabs
{
    public class InvoicingViewModel : ViewModelBase, ISettingsContent, IInitializationAsync, IClear
    {
        public string Name => "Generate Invoice";

        private readonly IResolver _resolverService;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private readonly IReportService _reportService;

        private readonly PriceCounting _priceCounting;

        private List<Client> _clients;
        private Client _selectedClient;
        private List<Department> _departments;
        private List<LinenList> _linenLists;
        private List<DepartmentContractViewModel> _departmentContracts;
        private ObservableCollection<ExtraChargeViewModel> _extraCharges;
        private ObservableCollection<TaxAndFeeViewModel> _taxAndFees;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<NoteRow> _noteRows;
        private List<NoteHeaderViewModel> _noteHeaders;

        private InvoiceSumViewModel _invoiceSum;
        private ObservableCollection<InvoiceSumViewModel> _invoices;
        private PrimeInfo _primeInfo;
        private DateTime _dateOfInvoice;

        private ObservableCollection<InvoiceItems> _noteSummaryItems;
        private InvoiceItems _selectedNoteSummaryItem;
        private ObservableCollection<InvoiceItems> _previewCharges;
        private bool _showExpress;
        private int _lastInvoiceId;
        private bool _byCollectionDate;
        private List<NoteHeaderViewModel> _notes;
        private double _grandTotal;

        public double GrandTotal
        {
            get => _grandTotal;
            set => Set(ref _grandTotal, value);
        }

        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public bool ByCollectionDate
        {
            get => _byCollectionDate;
            set => Set(ref _byCollectionDate, value);
        }
        public int LastInvoiceId
        {
            get => _lastInvoiceId;
            set => Set(ref _lastInvoiceId, value);
        }
        public bool ShowExpress
        {
            get => _showExpress;
            set => Set(ref _showExpress, value);
        }
        public ObservableCollection<InvoiceSumViewModel> Invoices
        {
            get => _invoices;
            set => Set(ref _invoices, value);
        }
        public List<DepartmentContractViewModel> DepartmentContracts
        {
            get => _departmentContracts;
            set => Set(ref _departmentContracts, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public List<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public List<NoteHeaderViewModel> Notes
        {
            get => _notes;
            set => Set(ref _notes, value);
        }
        public DateTime DateOfInvoice
        {
            get => _dateOfInvoice;
            set => Set(ref _dateOfInvoice, value);
        }
        public InvoiceItems SelectedNoteSummaryItem
        {
            get => _selectedNoteSummaryItem;
            set => Set(ref _selectedNoteSummaryItem, value);
        }
        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public ObservableCollection<InvoiceItems> PreviewCharges
        {
            get => _previewCharges;
            set => Set(ref _previewCharges, value);
        }
        public InvoiceSumViewModel InvoiceSum
        {
            get => _invoiceSum;
            set => Set(ref _invoiceSum, value);
        }
        public ObservableCollection<ExtraChargeViewModel> ExtraCharges
        {
            get => _extraCharges;
            set => Set(ref _extraCharges, value);
        }
        public ObservableCollection<TaxAndFeeViewModel> TaxAndFees
        {
            get => _taxAndFees;
            set => Set(ref _taxAndFees, value);
        }
        public ObservableCollection<InvoiceItems> NoteSummaryItems
        {
            get => _noteSummaryItems;
            set => Set(ref _noteSummaryItems, value);
        }
        public DateTime EndDate
        {
            get => _endDate;
            set => Set(ref _endDate, value);
        }
        public DateTime StartDate
        {
            get => _startDate;
            set => Set(ref _startDate, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CoverPageCommand { get; }
        public RelayCommand AnnexWindowCommand { get; }
        public RelayCommand AddChargeCommand { get; }
        public RelayCommand GenerateCommand { get; }
        public RelayCommand NoteEditCommand { get; }
        public RelayCommand ChangeLinenPriceCommand { get; }
        public RelayCommand ChangeNotesLinenCommand { get; }
        public RelayCommand GetNotesCommand { get; }
        public RelayCommand DepartmentDetailsCommand { get; }

        public InvoicingViewModel(IReportService reportService, IResolver resolverService, IDataService dataService,
            IDispatcher dispatcher, IDialogService dialogService, PriceCounting priceCounting)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _priceCounting = priceCounting ?? throw new ArgumentNullException(nameof(priceCounting));
            _resolverService = resolverService ?? throw new ArgumentNullException(nameof(resolverService));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));

            GenerateCommand = new RelayCommand(GenerateInvoice, () => SelectedClient != null);
            AddChargeCommand = new RelayCommand(AddExtraCharge, () => SelectedClient != null);
            SaveCommand = new RelayCommand(Save);
            CoverPageCommand = new RelayCommand(GetCoverPageReport);
            AnnexWindowCommand = new RelayCommand(AnnexWindow, () => SelectedClient != null);
            NoteEditCommand = new RelayCommand(NoteEdit, () => SelectedNoteSummaryItem?.OriginalObject is NoteHeader);
            ChangeLinenPriceCommand = new RelayCommand(ChangeLinenPrice, () => SelectedClient != null);
            ChangeNotesLinenCommand = new RelayCommand(ChangeNoteLinen, () => SelectedClient != null);
            GetNotesCommand = new RelayCommand(GetNotes);
            DepartmentDetailsCommand = new RelayCommand(DepartmentDetails, () => SelectedClient != null);

            PropertyChanged += OnPropertyChanged;
        }
        public async Task InitializeAsync()
        {
            PreviewCharges = new ObservableCollection<InvoiceItems>();
            NoteSummaryItems = new ObservableCollection<InvoiceItems>();
            InvoiceSum = new InvoiceSumViewModel();
            SelectedClient = null;

            _dialogService.ShowBusy();

            try
            {
                var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
                _dispatcher.RunInMainThread(() => Clients = client.OrderBy(x => x.ShortName).ToList());

                var prime = await _dataService.GetAsync<PrimeInfo>();
                _dispatcher.RunInMainThread(() => PrimeInfo = prime.FirstOrDefault());

                var invoice = await _dataService.GetAsync<Invoice>();
                var invoices = invoice.Select(x => new InvoiceSumViewModel(x));
                _dispatcher.RunInMainThread(() => Invoices = invoices.ToObservableCollection());

                ShowExpress = false;
                NoteHeaders?.Clear();
                NoteRows?.Clear();

                if (Invoices != null && Invoices.Count > 0) LastInvoiceId = Invoices.Max(x => x.Id);
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

        private async void GetNotes()
        {
            if (SelectedClient == null) return;

            _dialogService.ShowBusy();

            try
            {
                var department = await _dataService.GetAsync<Department>();
                _dispatcher.RunInMainThread(() => Departments = department.ToList());

                var linen = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                _dispatcher.RunInMainThread(() => LinenLists = linen.ToList());

                var departmentContract = await _dataService.GetAsync<DepartmentContract>(x => x.InvoiceId == null);
                var departmentContracts = departmentContract.Select(x => new DepartmentContractViewModel(x));
                _dispatcher.RunInMainThread(() => DepartmentContracts = departmentContracts.ToList());

                var taxAndFee = await _dataService.GetAsync<TaxAndFees>(x => x.InvoiceId == null && x.ClientId == SelectedClient.Id);
                var taxAndFees = taxAndFee.Where(x => x.InvoiceId == null).Select(x => new TaxAndFeeViewModel(x));
                _dispatcher.RunInMainThread(() => TaxAndFees = taxAndFees.ToObservableCollection());

                var noteHeader = await _dataService.GetAsync<NoteHeader>(x =>
                    x.InvoiceId == null &&
                    x.ClientId == SelectedClient.Id &&
                    x.NoteStatus == (int)NoteStatusEnum.PreInvoice ||
                    x.NoteStatus == (int)NoteStatusEnum.ClientNote);
                var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
                _dispatcher.RunInMainThread(() => Notes = noteHeaders.ToList());

                SetNotesForInvoicing();

                var noteRows = new List<NoteRow>();
                foreach (var note in NoteHeaders)
                {
                    var row = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    noteRows.AddRange(row);
                }
                noteRows.ForEach(x=> x.LinenList = LinenLists.FirstOrDefault(y=> y.Id == x.LinenListId));
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToList());

                GenerateInvoice();
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                InvoiceDateRangeSet();
                ByCollectionDate = (bool) SelectedClient?.ClientInfo.ByCollectionDate;

                if (SelectedClient == null) return;

                GenerateCommand.RaiseCanExecuteChanged();
                AddChargeCommand.RaiseCanExecuteChanged();
                ChangeLinenPriceCommand.RaiseCanExecuteChanged();
                ChangeNotesLinenCommand.RaiseCanExecuteChanged();
                DepartmentDetailsCommand.RaiseCanExecuteChanged();
                AnnexWindowCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedNoteSummaryItem))
            {
                NoteEditCommand.RaiseCanExecuteChanged();
            }
            if (e.PropertyName == nameof(ShowExpress))
            {
                ShowExpressCharge();
            }
        }

        private void UnSubscribeItem(InvoiceItems item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(InvoiceItems item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is InvoiceItems)) return;

            if (e.PropertyName == nameof(InvoiceItems.IsSelected))
            {
                RefreshTaxAndGrand();
                CountGrandTotal();
            }
        }

        public void RefreshTaxAndGrand()
        {
            PreviewCharges.ForEach(UnSubscribeItem);

            PreviewCharges = _priceCounting.RefreshTaxAndCharges(TaxAndFees.Select(x => x.OriginalObject).ToList(),
                PreviewCharges);

            PreviewCharges.ForEach(SubscribeItem);
        }

        public void CountGrandTotal()
        {
            GrandTotal = 0;
            var previews = PreviewCharges.Where(x => x.IsSelected).ToList();

            foreach (var preview in previews)
            {
                GrandTotal += preview.Amount - preview.DiscountAmount;
            }

            GrandTotal += GrandTotal * PrimeInfo.VAT;
        }

        public void InvoiceDateRangeSet()
        {
            if (SelectedClient?.ClientInfo == null)
                return;

            var month = DateTime.Today.Month;
            var year = DateTime.Today.Year;

            if (SelectedClient.ClientInfo.Start == 1 && SelectedClient.ClientInfo.End == 31)
            {
                if (month == 1)
                {
                    month = 12;
                    year--;
                }
                else
                {
                    month--;
                }

                StartDate = new DateTime(year, month, DateTime.MinValue.Day,
                    DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);

                EndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month),
                    DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);

                return;
            }

            if (SelectedClient.ClientInfo.Start > SelectedClient.ClientInfo.End)
            {

                EndDate = new DateTime(year, month, SelectedClient.ClientInfo.End,
                    DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);

                if (month == 1)
                {
                    month = 12;
                    year--;
                }
                else
                {
                    month--;
                }

                StartDate = new DateTime(year, month, SelectedClient.ClientInfo.Start,
                    DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);
                return;
            }

            StartDate = new DateTime(year, month, DateTime.MinValue.Day,
                DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);

            EndDate = new DateTime(year, month, DateTime.DaysInMonth(DateTime.Today.Year, month),
                DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
        }

        public async void Save()
        {
            if (InvoiceSum == null || InvoiceSum.NoteHeaders.Count == 0 || InvoiceSum.Departments.Count == 0) return;
            if (!_dialogService.ShowQuestionDialog("Do you want to Save Invoice ? "))
                return;


            _dialogService.ShowBusy();

            try
            {
                var departments = new List<Department>();
                foreach (var previewCharge in PreviewCharges.Where(x => x.IsSelected))
                {
                    if (!(previewCharge.OriginalObject is Department department)) continue;

                    departments.Add(department);

                    if (Departments.Any(x => x.ParentId == department.Id))
                    {
                        departments.AddRange(Departments.Where(x => x.ParentId == department.Id));
                    }
                }

                if (departments.Count == 0)
                {
                    _dialogService.ShowInfoDialog("Please select Departments for Invoicing");
                    return;
                }

                //// Save Invoice
                InvoiceSum.AcceptChanges();
                await _dataService.AddOrUpdateAsync(InvoiceSum.OriginalObject);

                //// Select NoteHeaders, Department Contracts
                var noteHeaders = new List<NoteHeaderViewModel>();
                var invoiceContracts = new List<DepartmentContractViewModel>();

                foreach (var department in departments)
                {
                    noteHeaders.AddRange(NoteHeaders.Where(x => x.DepartmentId == department.Id)
                        .ToObservableCollection());

                    foreach (var invContract in InvoiceSum.DepartmentContracts.Where(x => x.DepartmentId == department.Id))
                    {
                        var newContracts = new DepartmentContractViewModel
                        {
                            DepartmentId = invContract.DepartmentId,
                            FamilyLinenId = invContract.FamilyLinenId,
                            Quantity = invContract.Quantity,
                            Percentage = invContract.Percentage,
                            OrderNumber = invContract.OrderNumber,
                            Name = invContract.Name,
                            InvoiceId = InvoiceSum.OriginalObject.Id,
                        };
                        newContracts.AcceptChanges();
                        invoiceContracts.Add(newContracts);
                    }
                }

                if (noteHeaders.Count == 0)
                {
                    _dialogService.ShowInfoDialog("Selected Departments doesn't have notes");
                    return;
                }

                //// Save NoteHeaders
                foreach (var note in noteHeaders)
                {
                    note.InvoiceId = InvoiceSum.OriginalObject.Id;
                    note.NoteStatus = (int)NoteStatusEnum.Invoiced;
                    note.AcceptChanges();
                    await _dataService.AddOrUpdateAsync(note.OriginalObject);
                }

                //// Save Department Contracts
                if (invoiceContracts.Count > 0)
                {
                    invoiceContracts.ForEach(x => x.AcceptChanges());
                    await _dataService.AddOrUpdateAsync(invoiceContracts.Select(x => x.OriginalObject));
                }

                //// Save Tax and Fees

                if (TaxAndFees != null && TaxAndFees.Count > 0)
                {
                    var invoiceTaxAndFees = new ObservableCollection<TaxAndFeeViewModel>();
                    foreach (var taxAndFee in TaxAndFees?.Where(x => x.ClientId == SelectedClient.Id))
                    {
                        var newTaxAndFees = new TaxAndFeeViewModel()
                        {
                            ClientId = taxAndFee.ClientId,
                            UnitId = taxAndFee.UnitId,
                            Number = taxAndFee.Number,
                            OrderNumber = taxAndFee.OrderNumber,
                            Name = taxAndFee.Name,
                            InvoiceId = InvoiceSum.OriginalObject.Id,
                        };
                        newTaxAndFees.AcceptChanges();
                        invoiceTaxAndFees.Add(newTaxAndFees);
                    }
                    if (invoiceTaxAndFees.Count > 0)
                    {
                        await _dataService.AddOrUpdateAsync(invoiceTaxAndFees.Select(x => x.OriginalObject));
                    }
                }

                //// Save Extra Charges

                var extraCharges = ExtraCharges?.Where(x => x.ClientId == SelectedClient.Id).ToObservableCollection();

                if (extraCharges != null && extraCharges.Count > 0)
                {
                    extraCharges.ForEach(x => x.InvoiceId = InvoiceSum.OriginalObject.Id);
                    extraCharges.ForEach(x => x.AcceptChanges());
                    await _dataService.AddOrUpdateAsync(extraCharges.Select(x => x.OriginalObject));
                }

                LastInvoiceId = InvoiceSum.OriginalObject.Id;

                InvoiceSum = null;
                PreviewCharges.Clear();
                NoteSummaryItems.Clear();
                NoteHeaders = new List<NoteHeaderViewModel>();
                Notes.Clear();
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

        private void SetNotesForInvoicing()
        {
            if (Notes == null || !Notes.Any())
                return;

            NoteHeaders = ByCollectionDate

                ? Notes.Where(x => x.ClientId == SelectedClient.Id &&
                                   x.CollectionDate >= StartDate &&
                                   x.CollectionDate <= EndDate).ToList()

                : Notes.Where(x => x.ClientId == SelectedClient.Id &&
                                   x.DeliveryDate >= StartDate &&
                                   x.DeliveryDate <= EndDate).ToList();
        }

        private bool DateRange()
        {
            int days;
            if (!ByCollectionDate)
            {
                days = NoteHeaders.Max(x => x.DeliveryDate.DayOfYear) - NoteHeaders.Min(x => x.DeliveryDate.DayOfYear);
            }
            else
            {
                days = NoteHeaders.Max(x => x.CollectionDate.DayOfYear) - NoteHeaders.Min(x => x.CollectionDate.DayOfYear);
            }

            if (days <= 31) return true;

            _dialogService.ShowErrorDialog("Range of days cannot be more then 31");
            return false;
        }

        private void GenerateInvoice()
        {
            if (SelectedClient == null || NoteHeaders == null || NoteHeaders.Count == 0 || !DateRange()) return;

            _dialogService.ShowBusy();
            SetNotesForInvoicing();
            ExtraCharges?.Clear();

            try
            {
                InvoiceSum = new InvoiceSumViewModel
                {
                    DateStart = StartDate,
                    DateEnd = EndDate,
                    ClientId = SelectedClient.Id,
                    Vat = PrimeInfo.VAT,
                    Name = $"IN {LastInvoiceId + 1:D6} /{DateTime.Now:yy}",
                };
                DateOfInvoice = EndDate;

                foreach (var department in Departments.Where(x => x.ClientId == SelectedClient.Id))
                {
                    InvoiceSum.Departments.Add(department);

                    DepartmentContracts.Where(x => x.DepartmentId == department.Id).ForEach(x =>
                        InvoiceSum.DepartmentContracts.Add(x.OriginalObject));

                    foreach (var noteHeader in NoteHeaders.Where(x => x.DepartmentId == department.Id))
                    {
                        InvoiceSum.NoteHeaders.Add(noteHeader.OriginalObject);

                        foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id))
                        {
                            InvoiceSum.NoteRows.Add(noteRow);
                        }
                    }
                }

                var taxAndFee = TaxAndFees?.Where(x => x.ClientId == SelectedClient.Id).Select(x => x.OriginalObject)
                    .ToList();

                var extraCharge = ExtraCharges?.Select(x => x.OriginalObject).ToList();

                InvoiceSum = _priceCounting?.GetInvoiceSum(InvoiceSum, taxAndFee, extraCharge);
                NoteSummaryItems = _priceCounting.GetNotesSummary().ToObservableCollection();

                PreviewCharges = _priceCounting.GetInvoicePreViews(taxAndFee, extraCharge).ToObservableCollection();

                PreviewCharges.ForEach(x => x.IsSelected = true);
                PreviewCharges.ForEach(SubscribeItem);

                CountGrandTotal();
                ShowExpressCharge();
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

        public void ShowExpressCharge()
        {
            if (PreviewCharges == null || PreviewCharges.Count == 0)
                return;

            if (!ShowExpress)
            {
                PreviewCharges?.Remove(PreviewCharges?.FirstOrDefault(x => x.Name == "Express (Already included)"));
                return;
            }

            var expressNotes = _priceCounting.GetExpressNotes(InvoiceSum?.NoteHeaders
                .Where(x => x.DeliveryTypeId == (int) DeliveryTypeEnum.Express).ToList());

            var expressCharge = new InvoiceItems()
            {
                Name = "Express (Already included)",
                Id = PreviewCharges.Max(x => x.Id) + 1,
            };

            if (expressNotes == null) return;

            foreach (var note in expressNotes)
            {
                expressCharge.Amount += note.Amount;
                expressCharge.VatAmount = expressCharge.Amount * PrimeInfo.VAT;
            }

            PreviewCharges.Add(expressCharge);
        }

        private void RefreshInvoice(bool isChanged)
        {
            if (!isChanged) return;

            Notes?.Clear();
            NoteHeaders?.Clear();
            NoteRows?.Clear();
            InvoiceSum?.Cleanup();
            PreviewCharges?.Clear();
            NoteSummaryItems?.Clear();

            GetNotes();
        }

        // окна для доп. информации
        public async void AddExtraCharge()
        {
            if (SelectedClient == null)
                return;

            var chargeViewModel = _resolverService.Resolve<ChargeViewModel>();

            await chargeViewModel.InitializeAsync(SelectedClient, null);
            var showDialog = _dialogService.ShowDialog(chargeViewModel);

            if (!showDialog) return;

            ExtraCharges = chargeViewModel.GetExtraCharges();
            TaxAndFees = chargeViewModel.GetTaxAndFees();
            GenerateInvoice();
        }
        public async void DepartmentDetails()
        {
            if (SelectedClient == null)
                return;

            var departmentDetails = _resolverService.Resolve<DepartmentDetailsViewModel>();

            await departmentDetails.InitializeAsync(SelectedClient, null);
            var showDialog = _dialogService.ShowDialog(departmentDetails);

            RefreshInvoice(showDialog);
        }

        public async void ChangeNoteLinen()
        {
            var changeNote = _resolverService.Resolve<ChangeNotesLinenViewModel>();
            await changeNote.InitializeAsync(SelectedClient, null);
            var showDialog = _dialogService.ShowDialog(changeNote);

            RefreshInvoice(showDialog);
        }

        public async void NoteEdit()
        {
            if (SelectedNoteSummaryItem.OriginalObject == null)
                return;

            var noteEditViewModel = _resolverService.Resolve<NoteEditViewModel>();
            var noteHeader = NoteHeaders.FirstOrDefault(x => x.Id == SelectedNoteSummaryItem.OriginalObject.Id);

            await noteEditViewModel.InitializeAsync(noteHeader);
            var showDialog = _dialogService.ShowDialog(noteEditViewModel);

            RefreshInvoice(showDialog);
        }

        public async void ChangeLinenPrice()
        {
            if (SelectedClient == null) return;

            var changeLinenPriceViewModel = _resolverService.Resolve<ChangeLinenPriceViewModel>();
            var client = SelectedClient;

            await changeLinenPriceViewModel.InitializeAsync(client, null);
            var showDialog = _dialogService.ShowDialog(changeLinenPriceViewModel);

            RefreshInvoice(showDialog);
        }

        public void Clear()
        {

        }

        private void Print(IReport report)
        {
            if (report == null) return;

            _reportService.ShowReportPreview(report);
        }

        private void GetCoverPageReport()
        {
            if (InvoiceSum == null) return;

            if (!PreviewCharges.Any(x => x.IsSelected))
            {
                _dialogService.ShowInfoDialog("Select Departments");
                return;
            }

            var clientName = SelectedClient.InvoiceName;
            if (string.IsNullOrEmpty(clientName)) clientName = SelectedClient.Name;

            var report = new CoverPageReport()
            {
                Address = PrimeInfo.Address,
                ClientAddress = SelectedClient.ClientInfo.Address,
                Logo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo)),
                ClientName = clientName,
                ClientTRN = SelectedClient.ClientInfo.TRNNumber,
                InvoiceStart = StartDate,
                InvoiceEnd = EndDate,
                InvoiceDate = DateOfInvoice,
                TRN = PrimeInfo.TRNNumber,
                InvoiceNumber = InvoiceSum.Name,
                Items = new List<CoverPageItemReport>(),
                GrandTotal = 0,
                AmountTotal = 0,
                VatTotal = 0,
            };

            var id = 1;
            foreach (var preview in PreviewCharges.Where(x => x.IsSelected).OrderBy(x => x.OrderNumber))
            {
                report.Items.Add(new CoverPageItemReport()
                {
                    Id = id++,
                    Name = preview.Name,
                    Quantity = preview.QtyClientReceived,
                    VatAmount = preview.VatAmount,
                    AmountWithoutVat = preview.Amount,
                });

                report.AmountTotal += preview.Amount;
            }
            
            report.GrandTotal = GrandTotal;
            report.VatTotal = report.AmountTotal* PrimeInfo.VAT;

            report.GrandTotalStr = NumberToWordsConvertor.NumberToWords(report.GrandTotal);
            Print(report);
        }

        private async void AnnexWindow()
        {
            if (SelectedClient == null || NoteHeaders?.Count == 0 || NoteHeaders == null) return;

            var days = ByCollectionDate
                ? NoteHeaders.Max(x => x.CollectionDate.DayOfYear) - NoteHeaders.Min(x => x.CollectionDate.DayOfYear) +
                  1
                : NoteHeaders.Max(x => x.DeliveryDate.DayOfYear) - NoteHeaders.Min(x => x.DeliveryDate.DayOfYear) + 1;


            if (days > 31)
            {
                _dialogService.ShowInfoDialog("Note date range cannot be more then 31 days");
                return;
            }

            var annexWindow = _resolverService.Resolve<AnnexWindowViewModel>();
            var client = SelectedClient;
            var notes = NoteHeaders?.Select(x => x.OriginalObject).ToList();

            await annexWindow.InitializeAsync(client, notes, null, ByCollectionDate);
            var showDialog = _dialogService.ShowDialog(annexWindow);

            RefreshInvoice(showDialog);
        }
    }
}
