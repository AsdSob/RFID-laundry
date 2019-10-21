using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.InvoiceModel;
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
    public class InvoiceEditViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public string Name => "History";
        private readonly IResolver _resolverService;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private readonly PriceCounting _priceCounting;
        private readonly IReportService _reportService;

        private PrimeInfo _primeInfo;
        private Client _selectedClient;
        private List<Client> _clients;
        private InvoiceSumViewModel _selectedInvoice;
        private ObservableCollection<InvoiceSumViewModel> _invoices;
        private List<LinenList> _linenLists;

        private List<NoteHeaderViewModel> _noteHeaders;
        private List<NoteRow> _noteRows;
        private List<Department> _departments;
        private ObservableCollection<DepartmentContractViewModel> _departmentContracts;
        private ObservableCollection<TaxAndFeeViewModel> _taxAndFees;
        private ObservableCollection<ExtraChargeViewModel> _extraCharges;

        private List<InvoiceItems> _noteSummaryItems;

        private InvoiceItems _selectedNoteSummaryItem;
        private bool _showExpress;
        private InvoiceSumViewModel _invoiceSum;
        private ObservableCollection<InvoiceItems> _previewCharges;
        private bool _byCollectionDate;
        private double _grandTotal;

        public double GrandTotal
        {
            get => _grandTotal;
            set => Set(ref _grandTotal, value);
        }
        public bool ByCollectionDate
        {
            get => _byCollectionDate;
            set => Set(ref _byCollectionDate, value);
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
        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }

        public bool ShowExpress
        {
            get => _showExpress;
            set => Set(ref _showExpress, value);
        }

        public InvoiceItems SelectedNoteSummaryItem
        {
            get => _selectedNoteSummaryItem;
            set => Set(ref _selectedNoteSummaryItem, value);
        }

        public List<InvoiceItems> NoteSummaryItems
        {
            get => _noteSummaryItems;
            set => Set(ref _noteSummaryItems, value);
        }

        public ObservableCollection<ExtraChargeViewModel> ExtraCharges
        {
            get => _extraCharges;
            set => Set(ref _extraCharges, value);
        }

        public InvoiceSumViewModel SelectedInvoice
        {
            get => _selectedInvoice;
            set => Set(ref _selectedInvoice, value);
        }

        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }

        public ObservableCollection<TaxAndFeeViewModel> TaxAndFees
        {
            get => _taxAndFees;
            set => Set(ref _taxAndFees, value);
        }

        public ObservableCollection<DepartmentContractViewModel> DepartmentContracts
        {
            get => _departmentContracts;
            set => Set(ref _departmentContracts, value);
        }

        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public ObservableCollection<InvoiceSumViewModel> Invoices
        {
            get => _invoices;
            set => Set(ref _invoices, value);
        }

        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }

        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }

        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public List<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }

        public ObservableCollection<InvoiceSumViewModel> SortedInvoices =>
            Invoices?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public RelayCommand VoidInvoiceCommand { get; }
        public RelayCommand CoverPageCommand { get; }
        public RelayCommand AnnexWindowCommand { get; }
        public RelayCommand AddChargeCommand { get; }
        public RelayCommand GenerateCommand { get; }
        public RelayCommand NoteEditCommand { get; }
        public RelayCommand ChangeLinenPriceCommand { get; }
        public RelayCommand ChangeNotesLinenCommand { get; }
        public RelayCommand ShowInvoiceCommand { get; }
        public RelayCommand DepartmentDetailsCommand { get; }


        public async Task InitializeAsync()
        {
            var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
            _dispatcher.RunInMainThread(() => Clients = client.OrderBy(x => x.ShortName).ToList());

            var prime = await _dataService.GetAsync<PrimeInfo>();
            var primeInfo = prime.FirstOrDefault();
            _dispatcher.RunInMainThread(() => PrimeInfo = primeInfo);

            var invoice = await _dataService.GetAsync<Invoice>();
            var invoices = invoice.Select(x => new InvoiceSumViewModel(x));
            _dispatcher.RunInMainThread(() => Invoices = invoices.ToObservableCollection());
        }

        public InvoiceEditViewModel(IReportService reportService, IResolver resolverService, IDataService dataService,
            IDispatcher dispatcher, IDialogService dialogService, PriceCounting priceCounting)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _priceCounting = priceCounting ?? throw new ArgumentNullException(nameof(priceCounting));
            _resolverService = resolverService ?? throw new ArgumentNullException(nameof(resolverService));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));

            GenerateCommand = new RelayCommand(GetInvoice, () => SelectedClient != null);
            AddChargeCommand = new RelayCommand(AddExtraCharge, () => SelectedClient != null);
            VoidInvoiceCommand = new RelayCommand(VoidInvoice, () => SelectedInvoice != null);
            CoverPageCommand = new RelayCommand(GetCoverPageReport);
            AnnexWindowCommand = new RelayCommand(AnnexWindow, () => SelectedClient != null);
            NoteEditCommand = new RelayCommand(NoteEdit, () => SelectedNoteSummaryItem?.OriginalObject is NoteHeader);
            ChangeLinenPriceCommand = new RelayCommand(ChangeLinenPrice, () => SelectedClient != null);
            ChangeNotesLinenCommand = new RelayCommand(ChangeNoteLinen, () => SelectedClient != null);
            ShowInvoiceCommand = new RelayCommand(GetInvoice);
            DepartmentDetailsCommand = new RelayCommand(DepartmentDetails, () => SelectedClient != null);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedInvoices);
                ByCollectionDate = (bool) SelectedClient?.ClientInfo.ByCollectionDate;
            }

            if (e.PropertyName == nameof(SelectedInvoice))
            {
                VoidInvoiceCommand.RaiseCanExecuteChanged();
                GenerateCommand.RaiseCanExecuteChanged();
                AddChargeCommand.RaiseCanExecuteChanged();
                ChangeLinenPriceCommand.RaiseCanExecuteChanged();
                ChangeNotesLinenCommand.RaiseCanExecuteChanged();
                DepartmentDetailsCommand.RaiseCanExecuteChanged();
                AnnexWindowCommand.RaiseCanExecuteChanged();
                GetInvoice();
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

        private async void GetInvoice()
        {
            if (SelectedClient == null || SelectedInvoice == null)
            {
                ClearInvoiceView();
                return;
            }

            _dialogService.ShowBusy();

            try
            {
                var department = await _dataService.GetAsync<Department>();
                _dispatcher.RunInMainThread(() => Departments = department.ToList());

                var linen = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                _dispatcher.RunInMainThread(() => LinenLists = linen.ToList());

                var extra = await _dataService.GetAsync<ExtraCharge>(x => x.InvoiceId == SelectedInvoice.Id);
                var extraCharge = extra.Select(x => new ExtraChargeViewModel(x));
                _dispatcher.RunInMainThread(() => ExtraCharges = extraCharge.ToObservableCollection());

                var departmentContract =
                    await _dataService.GetAsync<DepartmentContract>(x => x.InvoiceId == SelectedInvoice.Id);
                var departmentContracts = departmentContract.Select(x => new DepartmentContractViewModel(x));
                _dispatcher.RunInMainThread(() => DepartmentContracts = departmentContracts.ToObservableCollection());

                var taxAndFee = await _dataService.GetAsync<TaxAndFees>(x => x.InvoiceId == SelectedInvoice.Id);
                var taxAndFees = taxAndFee.Select(x => new TaxAndFeeViewModel(x));
                _dispatcher.RunInMainThread(() => TaxAndFees = taxAndFees.ToObservableCollection());

                var noteHeader = await _dataService.GetAsync<NoteHeader>(x =>x.InvoiceId == SelectedInvoice.Id);
                var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x)).ToList();
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders);

                var noteRows = new List<NoteRow>();
                foreach (var note in NoteHeaders)
                {
                    var row = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    noteRows.AddRange(row);
                }
                noteRows.ForEach(x => x.LinenList = LinenLists.FirstOrDefault(y => y.Id == x.LinenListId));
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToList());

                CalculateInvoice();
                CalcGrandTotal();
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

        private void CalcGrandTotal()
        {
            GrandTotal = 0;
            var previews = PreviewCharges.Where(x => x.IsSelected).ToList();

            foreach (var preview in previews)
            {
                GrandTotal += preview.Amount - preview.DiscountAmount;
            }

            GrandTotal += GrandTotal * PrimeInfo.VAT;
        }

        private void CalculateInvoice()
        {
            if (SelectedClient == null || SelectedInvoice == null) return;

            _dialogService.ShowBusy();

            try
            {
                InvoiceSum = new InvoiceSumViewModel(SelectedInvoice.OriginalObject)
                {
                    DateStart = SelectedInvoice.DateStart,
                    DateEnd = SelectedInvoice.DateEnd,
                    ClientId = SelectedInvoice.ClientId,
                    Vat = SelectedInvoice.Vat,
                    Name = SelectedInvoice.Name,
                    Id = SelectedInvoice.Id,
                    NoteHeaders = NoteHeaders.Select(x => x.OriginalObject).ToList(),
                    NoteRows = NoteRows,
                    Departments = Departments.Where(x=> x.ClientId == SelectedClient.Id).ToList(),
                    DepartmentContracts = DepartmentContracts.Select(x=>x.OriginalObject).ToList(),
                };

                var extraCharge = ExtraCharges.Select(x=> x.OriginalObject).ToList();
                var taxAndFee = TaxAndFees.Select(x => x.OriginalObject).ToList();

                InvoiceSum = _priceCounting?.GetInvoiceSum(InvoiceSum, taxAndFee, extraCharge);
                NoteSummaryItems = _priceCounting.GetNotesSummary().ToList();
                PreviewCharges = _priceCounting.GetInvoicePreViews(taxAndFee, extraCharge).ToObservableCollection();
                PreviewCharges.ForEach(x => x.IsSelected = true);

                CalcGrandTotal();
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

        private async void VoidInvoice()
        {
            if (SelectedInvoice == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to VOID Invoice {SelectedInvoice.Name} ? \n Note!!! all charges ( Extra Charge, Contracts, Tax and Fees) will be DELETED ")
            )
                return;

            //// Void NoteHeaders
            var noteHeaders = NoteHeaders.Where(x => x.InvoiceId == SelectedInvoice.Id).ToObservableCollection();
            if (noteHeaders.Count > 0 && noteHeaders != null)
            {
                noteHeaders.ForEach(x => x.InvoiceId = null);
                noteHeaders.ForEach(x => x.NoteStatus = (int) NoteStatusEnum.PreInvoice);
                noteHeaders.ForEach(x => x.AcceptChanges());

                await _dataService.AddOrUpdateAsync(noteHeaders.Select(x => x.OriginalObject));
            }

            //// Void Department Contracts
            var depContracts = DepartmentContracts.Where(x => x.InvoiceId == SelectedInvoice.Id).ToList();
            if (depContracts.Any() && depContracts != null)
            {
                foreach (var depCon in depContracts)
                {
                    await _dataService.DeleteAsync(depCon.OriginalObject);
                }
            }

            //// Void Tax and Fees
            var taxAndFees = TaxAndFees.Where(x => x.InvoiceId == SelectedInvoice.Id).ToList();
            if (taxAndFees.Any() && taxAndFees != null)
            {
                foreach (var taxAndFee in taxAndFees)
                {
                    await _dataService.DeleteAsync(taxAndFee.OriginalObject);
                }
            }

            //// Void Extra Charges
            var extraCharges = ExtraCharges.Where(x => x.InvoiceId == SelectedInvoice.Id).ToList();
            if (extraCharges.Any() && extraCharges != null)
            {
                foreach (var extraCharge in extraCharges)
                {
                    await _dataService.DeleteAsync(extraCharge.OriginalObject);
                }
            }

            await _dataService.DeleteAsync(SelectedInvoice.OriginalObject);
            Invoices.Remove(SelectedInvoice);

            ClearInvoiceView();
        }

        public void ClearInvoiceView()
        {
            InvoiceSum = null;
            RaisePropertyChanged(() => SortedInvoices);
            PreviewCharges.Clear();
            NoteSummaryItems.Clear();
            NoteHeaders.Clear();
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
                .Where(x => x.DeliveryTypeId == (int)DeliveryTypeEnum.Express).ToList());

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
                expressCharge.QtyClientReceived += note.QtyClientReceived;
            }

            PreviewCharges.Add(expressCharge);
        }

        private void RefreshInvoice(bool isChanged)
        {
            if (!isChanged) return;

            NoteHeaders?.Clear();
            NoteHeaders?.Clear();
            NoteRows?.Clear();
            InvoiceSum?.Cleanup();
            PreviewCharges?.Clear();
            NoteSummaryItems?.Clear();

            GetInvoice();
        }

        // окна для доп. информации
        public async void AddExtraCharge()
        {
            if (SelectedClient == null)
                return;

            var chargeViewModel = _resolverService.Resolve<ChargeViewModel>();

            await chargeViewModel.InitializeAsync(SelectedClient, SelectedInvoice.Id);
            var showDialog = _dialogService.ShowDialog(chargeViewModel);

            if (!showDialog) return;

            ExtraCharges = chargeViewModel.GetExtraCharges();
            TaxAndFees = chargeViewModel.GetTaxAndFees();
            CalculateInvoice();
        }
        public async void DepartmentDetails()
        {
            if (SelectedClient == null)
                return;

            var departmentDetails = _resolverService.Resolve<DepartmentDetailsViewModel>();

            await departmentDetails.InitializeAsync(SelectedClient, SelectedInvoice.Id);
            var showDialog = _dialogService.ShowDialog(departmentDetails);

            RefreshInvoice(showDialog);
        }

        public async void ChangeNoteLinen()
        {
            var changeNote = _resolverService.Resolve<ChangeNotesLinenViewModel>();
            await changeNote.InitializeAsync(SelectedClient, SelectedInvoice.Id);
            var showDialog = _dialogService.ShowDialog(changeNote);

            RefreshInvoice(showDialog);
        }

        public async void NoteEdit()
        {
            if (SelectedNoteSummaryItem.OriginalObject == null)
                return;

            var noteEditViewModel = _resolverService.Resolve<NoteEditViewModel>();
            var noteHeader = NoteHeaders.FirstOrDefault(x => x.Id == SelectedNoteSummaryItem.OriginalObject.Id);

            await noteEditViewModel.InitializeAsync(new NoteHeaderViewModel(noteHeader.OriginalObject));
            var showDialog = _dialogService.ShowDialog(noteEditViewModel);

            RefreshInvoice(showDialog);
        }

        public async void ChangeLinenPrice()
        {
            if (SelectedClient == null) return;

            var changeLinenPriceViewModel = _resolverService.Resolve<ChangeLinenPriceViewModel>();
            var client = SelectedClient;

            await changeLinenPriceViewModel.InitializeAsync(client, SelectedInvoice.Id);
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

            var report = new CoverPageReport()
            {
                Address = PrimeInfo.Address,
                ClientAddress = SelectedClient.ClientInfo.Address,
                Logo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo)),
                ClientName = SelectedClient.InvoiceName,
                ClientTRN = SelectedClient.ClientInfo.TRNNumber,
                InvoiceStart = SelectedInvoice.DateStart,
                InvoiceEnd = SelectedInvoice.DateEnd,
                InvoiceDate = SelectedInvoice.DateEnd,
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
            report.VatTotal = report.AmountTotal * PrimeInfo.VAT;

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
            var notes = NoteHeaders?.Select(x=> x.OriginalObject).ToList();

            await annexWindow.InitializeAsync(client, notes, null, ByCollectionDate);
            var showDialog = _dialogService.ShowDialog(annexWindow);

            RefreshInvoice(showDialog);
        }
    }
}
