using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.InvoiceModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Data.Objects.Payment;
using PALMS.Data.Objects.Received_data;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.Invoices.ViewModel.Service;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using ClientViewModel = PALMS.Reports.ViewModel.EntityViewModel.ClientViewModel;

namespace PALMS.Reports.ViewModel.Windows
{
    public class RevenueWindowViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private readonly IExcelReportService _excelReportService;
        private readonly PriceCounting _priceCounting;

        public Action<bool> CloseAction { get; set;}

        private List<ClientViewModel> _clients;
        private List<Department> _departments;
        private List<DepartmentContract> _departmentContracts;
        private List<NoteHeader> _noteHeaders;
        private List<LaundryKg> _laundryKgs;
        private PrimeInfo _primeInfo;
        private List<UnitReportViewModel> _departmentTypes;
        private List<TaxAndFees> _taxAndFees;
        private List<ExtraCharge> _extraCharges;
        private List<Invoice> _invoices;
        private List<RevenueGroupData> _revenueReportData;

        public List<RevenueGroupData> RevenueReportData
        {
            get => _revenueReportData;
            set => Set(ref _revenueReportData, value);
        }
        public List<Invoice> Invoices
        {
            get => _invoices;
            set => Set(ref _invoices, value);
        }
        public List<ExtraCharge> ExtraCharges
        {
            get => _extraCharges;
            set => Set(ref _extraCharges, value);
        }
        public List<TaxAndFees> TaxAndFees
        {
            get => _taxAndFees;
            set => Set(ref _taxAndFees, value);
        }
        public List<UnitReportViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(ref _departmentTypes, value);
        }
        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public List<LaundryKg> LaundryKgs
        {
            get => _laundryKgs;
            set => Set(ref _laundryKgs, value);
        }
        public List<NoteHeader> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public List<DepartmentContract> DepartmentContracts
        {
            get => _departmentContracts;
            set => Set(ref _departmentContracts, value);
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


        public bool BySoiledKg { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public RelayCommand PrintCommand { get; }

        public RevenueWindowViewModel(IResolver resolver, IDataService dataService, IDispatcher dispatcher, IDialogService dialog, IExcelReportService excelReportService, PriceCounting priceCounting)
        {
            _excelReportService = excelReportService ?? throw new ArgumentNullException(nameof(excelReportService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _priceCounting = priceCounting ?? throw new ArgumentNullException(nameof(priceCounting));

            PrintCommand = new RelayCommand(Print);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
                var primeInfo = await _dataService.GetAsync<PrimeInfo>();
                _dispatcher.RunInMainThread(() => PrimeInfo = primeInfo.FirstOrDefault());

                var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
                var clients = client.Select(x => new ClientViewModel(x));
                _dispatcher.RunInMainThread(() => Clients = clients.OrderBy(x => x.ShortName).ToList());

                var department = await _dataService.GetAsync<Department>();
                _dispatcher.RunInMainThread(() => Departments = department.ToList());

                var invoice = await _dataService.GetAsync<Invoice>();
                _dispatcher.RunInMainThread(() => Invoices = invoice.ToList());

                var taxAndFee = await _dataService.GetAsync<TaxAndFees>();
                _dispatcher.RunInMainThread(() => TaxAndFees = taxAndFee.ToList());

                var extraCharge = await _dataService.GetAsync<ExtraCharge>();
                _dispatcher.RunInMainThread(() => ExtraCharges = extraCharge.ToList());

                var departmentContracts = await _dataService.GetAsync<DepartmentContract>();
                _dispatcher.RunInMainThread(() => DepartmentContracts = departmentContracts.ToList());

                var noteHeaders = await _dataService.GetAsync<NoteHeader>(x=> x.NoteRows);
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToList());

                var laundryKg = await _dataService.GetAsync<LaundryKg>();
                _dispatcher.RunInMainThread(() => LaundryKgs = laundryKg.ToList());

                DateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);

                DateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);

                var departmentType = EnumExtentions.GetValues<DepartmentTypeEnum>();
                DepartmentTypes = new List<UnitReportViewModel>();
                foreach (var depa in departmentType)
                {
                    var depType = new UnitReportViewModel(depa.Id, depa.Name);
                    depType.IsSelected = true;
                    DepartmentTypes.Add(depType);
                }

            BySoiledKg = true;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #region Calculate Revenue Data

        public async Task CalcRevenueGroupData()
        {
            RevenueReportData = new List<RevenueGroupData>
            {
                new RevenueGroupData {Name = "Exist Clients"},
            };

            RevenueReportData[0] = SetGroupItems(RevenueReportData[0]);

            //Add Cut of if Required
        }

        public RevenueGroupData SetGroupItems(RevenueGroupData group)
        {
            var groups = group;
            var dates = GetDateDate();
            var range = dates.Length;
            var id = 1;
            foreach (var client in GetClients())
            {
                var item = new RevenueGroupItem()
                {
                    Id = id++,
                    Name = client.Name,
                    SoiledKg = new double[range],
                    CleanKg = new double[range],
                    Amount = new double[range],
                    SalesKg = new double[range],
                    CutOfDate = new double[range],
                    CutOfDatePrevious = new double[range]
                };

                for (int i = 0; i < range; i++)
                {
                    var date = DateStart.AddMonths(i);
                    var amount = CalcInvoice(client.OriginalObject, date);

                    item.CutOfDatePrevious[i] = GetCutOfDatePrevious(client.OriginalObject, date, amount);

                    item.CutOfDate[i] = GetCutOfDate(client.OriginalObject, date);

                    item.Amount[i] = GetAmount(client.OriginalObject, date, amount);
                    item.Amount[i] += item.CutOfDate[i];


                    item.SoiledKg[i] = GetSoiledKg(client.OriginalObject, date);
                    item.CleanKg[i] = GetCleanKg(client.OriginalObject, date);

                    if (BySoiledKg)
                    {
                        if (Equals(item.SoiledKg[i] , 0.0))
                        {
                            item.SalesKg[i] = 0;
                        }
                        else
                        {
                            item.SalesKg[i] = item.Amount[i] / item.SoiledKg[i];
                        }
                    }
                    else
                    {
                        if (Equals(item.CleanKg[i], 0.0))
                        {
                            item.SalesKg[i] = 0;
                        }
                        else
                        {
                            item.SalesKg[i] = item.Amount[i] / item.CleanKg[i];
                        }
                    }
                }
                groups.Items.Add(item);
            }

            return groups;
        }

        public double GetAmount(Client client, DateTime date, double invoiceAmount)
        {
            if (client.ClientInfo.Start == 1 && client.ClientInfo.End == 31)
            {
                return invoiceAmount;
            }

            int dateRange = GetInvoiceMonthDays(client, date);

            var amount = (invoiceAmount / dateRange) * client.ClientInfo.End;
            return amount;
        }



        public double GetCutOfDate(Client client, DateTime date)
        {
            //calculate next month invoice, divide to amount and return cut of date
            double cutOfDate = 0.0;

            if (client.ClientInfo.Start == 1 && client.ClientInfo.Start == 31)
            {
                return cutOfDate;
            }

            var amount = CalcInvoice(client, date.AddMonths(1));

            int dateRange = GetInvoiceMonthDays(client, date.AddMonths(1));

            cutOfDate = amount / dateRange * (DateTime.DaysInMonth(date.Year, date.Month) - client.ClientInfo.End);

            return cutOfDate;
        }

        public double GetCutOfDatePrevious(Client client, DateTime date, double invoiceAmount)
        {
            //calculate days, divide to amount and return previous month amount
            double cutOfDatePrevious = 0.0;

            if (client.ClientInfo.Start == 1 && client.ClientInfo.Start == 31)
            {
                return cutOfDatePrevious;
            }

            int dateRange = GetInvoiceMonthDays(client, date);

            cutOfDatePrevious = invoiceAmount / dateRange * (dateRange - client.ClientInfo.Start +1);

            return cutOfDatePrevious;
        }

        public int GetInvoiceMonthDays(Client client, DateTime date)
        {
            int dateRange;

            if (date.Month == 1)
            {
                dateRange = 31;
            }
            else if(client.ClientInfo.Start == 1 && client.ClientInfo.End == 31)
            {
                dateRange = DateTime.DaysInMonth(date.Year, date.Month);
            }
            else
            {
                var start = new DateTime(date.Year, date.Month, client.ClientInfo.End).DayOfYear;
                var end = new DateTime(date.Year, date.Month - 1, client.ClientInfo.Start).DayOfYear;
                dateRange = start - end + 1;
            }

            return dateRange;
        }

        public double GetSoiledKg(Client client, DateTime date)
        {
            var kg = new double();
            var laundryKg = LaundryKgs.Where(x =>
                x.ClientId == client.Id && 
                x.KgTypeId == (int) KgTypeEnum.Soiled && 
                x.WashingDate.Year == date.Year &&
                x.WashingDate.Month == date.Month);

            foreach (var soiledKg in laundryKg)
            {
                kg += soiledKg.ExtFnB;
                kg += soiledKg.ExtGuest;
                kg += soiledKg.ExtLinen;
                kg += soiledKg.ExtManager;
                kg += soiledKg.ExtUniform;
                kg += soiledKg.Tunnel1;
                kg += soiledKg.Tunnel2;
            }

            return kg;
        }

        public double GetCleanKg(Client client, DateTime date)
        {
            var kg = new double();
            var laundryKg = LaundryKgs.Where(x =>
                x.ClientId == client.Id &&
                x.KgTypeId == (int)KgTypeEnum.Clean &&
                x.WashingDate.Year == date.Year &&
                x.WashingDate.Month == date.Month);

            foreach (var cleanKg in laundryKg)
            {
                kg += cleanKg.ExtFnB;
                kg += cleanKg.ExtGuest;
                kg += cleanKg.ExtLinen;
                kg += cleanKg.ExtManager;
                kg += cleanKg.ExtUniform;
                kg += cleanKg.Tunnel1;
                kg += cleanKg.Tunnel2;
            }
            return kg;
        }

        #endregion


        #region Get Basic Data

        public double CalcInvoice(Client client, DateTime date)
        {
            var amount = new double();
            var invoices = Invoices.Where(x =>
                x.ClientId == client.Id && x.DateEnd.Year == date.Year && x.DateEnd.Month == date.Month).ToList();

            foreach (var inv in invoices)
            {
                var invoiceSum = new InvoiceSumViewModel(inv);

                invoiceSum.Departments = Departments.Where(x => x.ClientId == client.Id).ToList();
                invoiceSum.DepartmentContracts = DepartmentContracts.Where(x => x.InvoiceId == invoiceSum.Id).ToList();

                foreach (var noteHeader in NoteHeaders.Where(x => x.InvoiceId == invoiceSum.Id))
                {
                    invoiceSum.NoteHeaders.Add(noteHeader);
                    invoiceSum.NoteRows.AddRange(noteHeader.NoteRows.Where(x => x.DeletedDate == null));
                }

                var taxes = TaxAndFees.Where(x => x.InvoiceId == invoiceSum.Id).ToList();
                var extraCharges = ExtraCharges.Where(x => x.InvoiceId == invoiceSum.Id).ToList();

                invoiceSum = _priceCounting.GetInvoiceSum(invoiceSum, taxes, extraCharges);

                amount += invoiceSum.Amount + invoiceSum.VatAmount;
            }

            return amount;
        }

        public DateTime[] GetDateDate()
        {

                var dates = new DateTime[(DateEnd.Year - DateStart.Year) * 12 + DateEnd.Month - DateStart.Month + 1];

                for (int i = 0; i < dates.Length; i++)
                {
                    dates[i] = DateStart.AddMonths(i);
                }
                return dates;
            //if (ByMonth)
            //{

            //}
            //else
            //{
            //    var dates = new DateTime[DateEnd.Year - DateStart.Year + 1];

            //    for (int i = 0; i < dates.Length; i++)
            //    {
            //        dates[i] = DateStart.AddYears(i);
            //    }
            //    return dates;
            //}
        }

        public List<ClientViewModel> GetClients()
        {
            var clients = Clients.Where(x => x.IsSelected).ToList();

            if (clients.Count != 0) return clients;

            _dialogService.ShowInfoDialog("Non of the clients was selected");
            return null;
        }

#endregion

        public async void Print()
        {
            if (DateEnd < DateStart)
            {
                _dialogService.ShowInfoDialog("Please select correct DATE range");
                return;
            }

            var data = new ExcelData()
            {
                Name = "Revenue Report",
                ReportType = ReportType.RevenueExcel,
                Days = GetDateDate(),
                Description = "Sales Report PAL",
                PrintDate = DateTime.Now,
            };

            try
            {
                await CalcRevenueGroupData();
                data.RevenueGroupDatas = RevenueReportData;
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


            SaveExcel(data);
        }

        private void SaveExcel(ExcelData data)
        {
            if (data == null) return;

            var savePath = _dialogService.ShowSaveFileDialog("Excel file (*.xlsx)|*.xlsx",
                $"{data.Name}_{DateTime.Now:yyyyddMMHHmm}");
            if (string.IsNullOrEmpty(savePath)) return;

            _excelReportService.SaveAsAsync(data, savePath);

            var fileInfo = new FileInfo(savePath);
            _dialogService.ShowInfoDialog($"Report saved{Environment.NewLine}{Environment.NewLine}{fileInfo.Name}");
        }
    }
}
