using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Model.InvoiceReports;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Invoices.ViewModel.Windows
{
    public class AnnexWindowViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IDispatcher _dispatcher;
        private readonly IExcelReportService _excelReportService;
        private readonly IReportService _reportService;

        private Client _selectedClient;
        private List<DepartmentViewModel> _departments;
        private List<NoteHeaderViewModel> _noteHeaders;
        private List<DepartmentViewModel> _selectedDepartments;
        private List<LinenList> _linenLists;
        private List<NoteRow> _noteRows;
        private bool _showExpress;
        private bool _showReWash;
        private List<UnitViewModel> _deliveryTypes;
        private List<UnitViewModel> _serviceTypes;
        private Department _selectedDepartment;
        private int _days;

        public int Days
        {
            get => _days;
            set => Set(ref _days, value);
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public int? SelectedInvoiceId { get; set; }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public bool ShowReWash
        {
            get => _showReWash;
            set => Set(ref _showReWash, value);
        }
        public bool ShowExpress
        {
            get => _showExpress;
            set => Set(ref _showExpress, value);
        }
        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public List<DepartmentViewModel> SelectedDepartments
        {
            get => _selectedDepartments;
            set => Set(ref _selectedDepartments, value);
        }
        public List<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public List<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public Action<bool> CloseAction { get; set; }
        private bool ByCollectionDay { get; set; }

        public RelayCommand CloseCommand { get; }
        public RelayCommand<object> PrintCommand { get; }
        public RelayCommand PrintAnnexTotalCommand { get; }

        public AnnexWindowViewModel(IReportService reportService, IDialogService dialogService, IDataService dataService, IDispatcher dispatcher, IExcelReportService excelReport)
        {
            _excelReportService = excelReport ?? throw new ArgumentNullException(nameof(excelReport));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));

            CloseCommand = new RelayCommand(Close);
            PrintCommand = new RelayCommand<object>(Print);
            PrintAnnexTotalCommand = new RelayCommand(AnnexTotal);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync(Client selectedClient, List<NoteHeader> noteHeaders, int? invoiceId, bool byCollectionDay )
        {
            SelectedClient = selectedClient;
            SelectedInvoiceId = invoiceId;
            NoteHeaders = noteHeaders.Select(x=> new NoteHeaderViewModel(x)).ToList();
            ByCollectionDay = byCollectionDay;

            if(SelectedClient == null) return;

            _dialogService.ShowBusy();

            try
            {
                if (SelectedClient.ClientInfo == null)
                {
                    var info = await _dataService.GetAsync<ClientInfo>(x => x.Client.Id == SelectedClient.Id && x.InvoiceId == null);
                    var clientInfo= info.First();
                    _dispatcher.RunInMainThread(() => SelectedClient.ClientInfo = clientInfo);
                }

                if (NoteHeaders == null || NoteHeaders.Count <= 0)
                {
                    var note = await _dataService.GetAsync<NoteHeader>(x => x.ClientId == SelectedClient.Id && x.InvoiceId == SelectedInvoiceId);
                    var notes = note.Select(x=> new NoteHeaderViewModel(x));
                    _dispatcher.RunInMainThread(() => NoteHeaders = notes.ToList());
                }

                var noteRows = new List<NoteRow>();
                foreach (var note in NoteHeaders)
                {
                    var noteRow = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    noteRows.AddRange(noteRow);
                }
                _dispatcher.RunInMainThread(() => NoteRows = noteRows);

                var linen = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                var linens = linen;
                _dispatcher.RunInMainThread(() => LinenLists = linens);

                var department = await _dataService.GetAsync<Department>(x=> x.ClientId == SelectedClient.Id);
                var departments = department.Select(x=> new DepartmentViewModel(x));
                _dispatcher.RunInMainThread(() => Departments = departments.ToList());

                var days = ByCollectionDay
                    ? NoteHeaders.Max(x => x.CollectionDate.DayOfYear) - NoteHeaders.Min(x => x.CollectionDate.DayOfYear) + 1
                    : NoteHeaders.Max(x => x.DeliveryDate.DayOfYear) - NoteHeaders.Min(x => x.DeliveryDate.DayOfYear) + 1;

                Days = days;
                ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>().ToList();
                DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>().ToList();
            }

            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartments))
            {
                PrintCommand.RaiseCanExecuteChanged();
            }
        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                return;
            CloseAction?.Invoke(false);
        }

        private List<int> GetDeliveryTypes()
        {
            var deliveryType = new List<int>
            {
                1
            };

            if (ShowExpress)
                deliveryType.Add(2);

            if (ShowReWash)
                deliveryType.Add(3);

            return deliveryType;
        }

        private ExcelData GenerateReport(int param)
        {
            var annexType = param;
            SelectedDepartments = Departments.Where(x => x.IsSelected).ToList();
            
            if (!SelectedDepartments.Any() || !NoteHeaders.Any())
            {
                _dialogService.ShowInfoDialog("None of the departments/note was selected");
                return null;
            }

            var excelReport = SetValues();

            switch (annexType)
            {
                case 1:
                    excelReport = GetTotalAnnex(excelReport);
                    break;

                case 2:
                    excelReport = GetCollectionAnnex(excelReport);
                    break;

                case 3:
                    excelReport = GetServiceAnnex(excelReport);
                    break;
            }
            return excelReport;
        }

        private ExcelData SetValues()
        {
            if (Days > 31) throw new ArgumentOutOfRangeException(nameof(Days), "from 28 to 31");

            var excelData = new ExcelData
            {
                Name = SelectedClient.Name,
                Description = "",
                Month = NoteHeaders.Max(x => x.DeliveryDate).ToString("MMMM"),
                NoteType = "",
                Days = GetNoteDays(Days),
                Groups = new List<AnnexGroupData>()
            };
            return excelData;
        }

        public ExcelData GetTotalAnnex(ExcelData data)
        {
            var excelData = data;
            var deliveryTypes = GetDeliveryTypes();

            excelData.ReportType = ReportType.AnnexTotal;

            foreach (var department in SelectedDepartments.OrderBy(x => x.Name))
            {
                var linens = GetNoteLinens(department);
                if (linens.Count == 0) continue;

                excelData.Description += " - " + department.Name;

                foreach (var deliveryType in deliveryTypes.OrderBy(x=> x))
                {
                    excelData.Groups.Add(GetTotalAnnexItem(department, linens, excelData, deliveryType));
                }
            } return excelData;
        }

        public ExcelData GetCollectionAnnex(ExcelData data)
        {
            var excelData = data;
            var deliveryTypes = GetDeliveryTypes();

            excelData.ReportType = ReportType.AnnexCollection;

            foreach (var department in SelectedDepartments.OrderBy(x => x.Name))
            {
                var linens = GetNoteLinens(department);
                if(linens.Count==0) continue;
                excelData.Description += " - " + department.Name;

                foreach (var deliveryType in deliveryTypes.OrderBy(x => x))
                {
                    excelData.Groups.Add(GetCollectionAnnexItem(department, linens, excelData, deliveryType));
                }
            }
            return excelData;

        }

        public ExcelData GetServiceAnnex(ExcelData data)
        {
            var excelData = data;
            var deliveryTypes = GetDeliveryTypes();

            excelData.ReportType = ReportType.AnnexService;

            foreach (var department in SelectedDepartments.OrderBy(x => x.Name))
            {
                var linens = GetNoteLinens(department);
                if (linens.Count == 0) continue;

                excelData.Description += " - " + department.Name;

                foreach (var deliveryType in deliveryTypes.OrderBy(x => x))
                {
                    excelData.Groups.Add(GetAllServiceAnnexItem(department, linens, excelData, deliveryType));
                }
            }
            return excelData;
        }

        private AnnexGroupData GetTotalAnnexItem(DepartmentViewModel department, List<LinenList> linens, ExcelData excelData, int deliveryId)
        {
            var name = department.Name;
            if (department.ParentId != null)
            {
                name += $" - {Departments.FirstOrDefault(x => department.ParentId == x.Id)?.Name} ";
            }

            var group = new AnnexGroupData()
            {
                Name = name + $"( {DeliveryTypes.FirstOrDefault(x => x.Id == deliveryId)?.Name} )",
                Items = new List<AnnexGroupItem>(),
                Notes = SetGroupNotes(department, excelData, deliveryId),
                WeightDelivery = SetGroupWeight(department, excelData, deliveryId, true),
                WeightCollection = SetGroupWeight(department, excelData, deliveryId, false),
                PricePerKg = SelectedClient.ClientInfo.WeighPrice,
            };

            foreach (var linen in linens)
            {
                var groupItem = SetGroupItem(linen, deliveryId, department);

                for (var i = 0; i < Days; i++)
                {
                    if (ByCollectionDay)
                    {
                        foreach (var note in NoteHeaders.Where(x =>
                            x.DeliveryTypeId == deliveryId &&
                            x.CollectionDate.DayOfYear == excelData.Days[i].DayOfYear &&
                            x.DepartmentId == department.Id))
                        {
                            foreach (var noteRow in NoteRows.Where(x =>
                                x.NoteHeaderId == note.Id && x.LinenListId == linen.Id))
                            {
                                groupItem.Value3[i] += noteRow.ClientReceivedQty;
                            }
                        }
                    }
                    else
                    {
                        foreach (var note in NoteHeaders.Where(x =>
                            x.DeliveryTypeId == deliveryId &&
                            x.DeliveryDate.DayOfYear == excelData.Days[i].DayOfYear &&
                            x.DepartmentId == department.Id))
                        {
                            foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == note.Id && x.LinenListId == linen.Id))
                            {
                                groupItem.Value3[i] += noteRow.ClientReceivedQty;
                            }
                        }
                    }
                }
                group.Items.Add(groupItem);
            }

            return group;
        }

        private AnnexGroupData GetCollectionAnnexItem(DepartmentViewModel department, List<LinenList> linens, ExcelData excelData, int deliveryId)
        {
            var name = department.Name;
            if (department.ParentId != null)
            {
                name += $" - {Departments.FirstOrDefault(x => department.ParentId == x.Id)?.Name} ";
            }

            var group = new AnnexGroupData()
            {
                Name = name + $"( {DeliveryTypes.FirstOrDefault(x => x.Id == deliveryId)?.Name} )",
                Items = new List<AnnexGroupItem>(),
                Notes = SetGroupNotes(department, excelData, deliveryId),
                WeightDelivery = SetGroupWeight(department, excelData, deliveryId, true),
                WeightCollection = SetGroupWeight(department, excelData, deliveryId, false),
                PricePerKg = SelectedClient.ClientInfo.WeighPrice,
            };

            foreach (var linen in linens)
            {
                var groupItem = SetGroupItem(linen, deliveryId, department);

                for (var i = 0; i < Days; i++)
                {
                    if (ByCollectionDay)
                    {
                        foreach (var note in NoteHeaders.Where(x =>
                            x.DeliveryTypeId == deliveryId &&
                            x.CollectionDate.DayOfYear == excelData.Days[i].DayOfYear &&
                            x.DepartmentId == department.Id))
                        {
                            foreach (var noteRow in NoteRows.Where(x =>
                                x.NoteHeaderId == note.Id && x.LinenListId == linen.Id))
                            {
                                groupItem.Value1[i] += noteRow.PrimeCollectionQty;
                                groupItem.Value3[i] += noteRow.ClientReceivedQty;
                            }
                        }
                    }
                    else
                    {
                        foreach (var note in NoteHeaders.Where(x =>
                            x.DeliveryTypeId == deliveryId &&
                            x.DeliveryDate.DayOfYear == excelData.Days[i].DayOfYear &&
                            x.DepartmentId == department.Id))
                        {
                            foreach (var noteRow in NoteRows.Where(x =>
                                x.NoteHeaderId == note.Id && x.LinenListId == linen.Id))
                            {
                                groupItem.Value1[i] += noteRow.PrimeCollectionQty;
                                groupItem.Value3[i] += noteRow.ClientReceivedQty;
                            }
                        }
                    }
                }
                group.Items.Add(groupItem);
            }

            return group;
        }

        private AnnexGroupData GetAllServiceAnnexItem(DepartmentViewModel department, List<LinenList> linens, ExcelData excelData, int deliveryId)
        {
            var name = department.Name;
            if (department.ParentId != null)
            {
                name += $" - {Departments.FirstOrDefault(x => department.ParentId == x.Id)?.Name} ";
            }

            var group = new AnnexGroupData()
            {
                Name = name + $"( {DeliveryTypes.FirstOrDefault(x => x.Id == deliveryId)?.Name} )",
                Items = new List<AnnexGroupItem>(),
                Notes = SetGroupNotes(department, excelData, deliveryId),
                WeightDelivery = SetGroupWeight(department, excelData, deliveryId, true),
                WeightCollection = SetGroupWeight(department, excelData, deliveryId, false),
                PricePerKg = SelectedClient.ClientInfo.WeighPrice,
            };

            foreach (var linen in linens)
            {
                var groupItem = SetGroupItem(linen, deliveryId, department);

                for (var i = 0; i < Days; i++)
                {
                    if (ByCollectionDay)
                    {
                        foreach (var note in NoteHeaders.Where(x =>
                            x.DeliveryTypeId == deliveryId &&
                            x.CollectionDate.DayOfYear == excelData.Days[i].DayOfYear &&
                            x.DepartmentId == department.Id))
                        {
                            foreach (var noteRow in NoteRows.Where(x =>
                                x.NoteHeaderId == note.Id && x.LinenListId == linen.Id))
                            {
                                switch (noteRow.ServiceTypeId)
                                {
                                    case (int)ServiceTypeEnum.DryCleaning:
                                        groupItem.Value1[i] += noteRow.ClientReceivedQty;
                                        break;

                                    case (int)ServiceTypeEnum.Laundry:
                                        groupItem.Value2[i] += noteRow.ClientReceivedQty;
                                        break;

                                    case (int)ServiceTypeEnum.Pressing:
                                        groupItem.Value3[i] += noteRow.ClientReceivedQty;
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var note in NoteHeaders.Where(x =>
                            x.DeliveryTypeId == deliveryId &&
                            x.DeliveryDate.DayOfYear == excelData.Days[i].DayOfYear &&
                            x.DepartmentId == department.Id))
                        {
                            foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == note.Id && x.LinenListId == linen.Id))
                            {
                                switch (noteRow.ServiceTypeId)
                                {
                                    case (int)ServiceTypeEnum.DryCleaning:
                                        groupItem.Value1[i] += noteRow.ClientReceivedQty;
                                        break;

                                    case (int)ServiceTypeEnum.Laundry:
                                        groupItem.Value2[i] += noteRow.ClientReceivedQty;
                                        break;

                                    case (int)ServiceTypeEnum.Pressing:
                                        groupItem.Value3[i] += noteRow.ClientReceivedQty;
                                        break;
                                }
                            }
                        }
                    }


                }
                group.Items.Add(groupItem);
            }

            return group;
        }

        private AnnexGroupItem SetGroupItem(LinenList linen, int deliveryType, DepartmentViewModel department)
        {
            var groupItem = new AnnexGroupItem()
            {
                Name = linen.MasterLinen.Name,
                PriceDryCleaning = linen.DryCleaning,
                PriceLaundry = linen.Laundry,
                PricePressing = linen.Pressing,
                Value2 = new double[Days],
                Value1 = new double[Days],
                Value3 = new double[Days],
            };

            if (deliveryType == (int) DeliveryTypeEnum.Express)
            {
                groupItem.PriceDryCleaning += groupItem.PriceDryCleaning * department.Express;
                groupItem.PriceLaundry += groupItem.PriceLaundry * department.Express;
                groupItem.PricePressing += groupItem.PricePressing * department.Express;
            }

            if (deliveryType == (int) DeliveryTypeEnum.ReWash)
            {
                groupItem.PriceDryCleaning = 0;
                groupItem.PriceLaundry = 0;
                groupItem.PricePressing = 0;
            }

            return groupItem;
        }

        private string[] SetGroupNotes(DepartmentViewModel dep, ExcelData excel, int deliveryType)
        {
            var department = dep;
            var notes = new string[Days];
            var deliveryId = deliveryType;
            var excelData = excel;

            if (ByCollectionDay)
            {
                for (var i = 0; i < Days; i++)
                {
                    foreach (var note in NoteHeaders.Where(x =>
                        x.DeliveryTypeId == deliveryId &&
                        x.CollectionDate.DayOfYear == excelData.Days[i].DayOfYear &&
                        x.DepartmentId == department.Id))
                    {
                        notes[i] += $"#{note.Id} ";
                    }
                }
            }
            else
            {
                for (var i = 0; i < Days; i++)
                {
                    foreach (var note in NoteHeaders.Where(x =>
                        x.DeliveryTypeId == deliveryId &&
                        x.DeliveryDate.DayOfYear == excelData.Days[i].DayOfYear &&
                        x.DepartmentId == department.Id))
                    {
                        notes[i] += $"#{note.Id} ";
                    }
                }
            }
            return notes;
        }

        private double[] SetGroupWeight(DepartmentViewModel dep, ExcelData excel, int deliveryType, bool deliveryWeight)
        {
            var department = dep;
            var weightCollection = new double[Days];
            var weightDelivery = new double[Days];
            var deliveryId = deliveryType;
            var excelData = excel;

            if (ByCollectionDay)
            {
                for (var i = 0; i < Days; i++)
                {
                    foreach (var note in NoteHeaders.Where(x =>
                        x.DeliveryTypeId == deliveryId &&
                        x.CollectionDate.DayOfYear == excelData.Days[i].DayOfYear &&
                        x.DepartmentId == department.Id))
                    {
                        weightCollection[i] += note.CollectionWeight;
                        weightDelivery[i] += note.DeliveryWeight;
                    }
                }
            }
            else
            {
                for (var i = 0; i < Days; i++)
                {
                    foreach (var note in NoteHeaders.Where(x =>
                        x.DeliveryTypeId == deliveryId &&
                        x.DeliveryDate.DayOfYear == excelData.Days[i].DayOfYear &&
                        x.DepartmentId == department.Id))
                    {
                        weightCollection[i] += note.CollectionWeight;
                        weightDelivery[i] += note.DeliveryWeight;
                    }
                }
            }

            if (deliveryWeight)
                return weightDelivery;
            else
                return weightCollection;
        }

        private DateTime[] GetNoteDays(int days)
        {
            if (days > 31) return null;

            var monthDays = new DateTime[days];

            monthDays[0] = ByCollectionDay
                ? NoteHeaders.Min(x => x.CollectionDate)
                : NoteHeaders.Min(x => x.DeliveryDate);

            for (var i = 1; i < days; i++)
            {
                monthDays[i] = monthDays[0].AddDays(i);
            }
            return monthDays;
        }

        private List<LinenList> GetNoteLinens(DepartmentViewModel department)
        {
            var linens = new List<LinenList>();
            foreach (var note in NoteHeaders.Where(x=> x.DepartmentId == department.Id))
            {
                foreach (var noteRow in NoteRows.Where(x=> x.NoteHeaderId == note.Id).OrderBy(x=> x.LinenListId))
                {
                    if (linens.Any(x => x.Id == noteRow.LinenListId))
                    {
                        continue;
                    }
                    linens.Add(LinenLists.FirstOrDefault(x => x.Id == noteRow.LinenListId));
                }
            }
            return linens.OrderBy(x=> x.MasterLinen.Name).ToList();
        }


        private void Print(object param)
        {
            var printParam = int.Parse(param.ToString());

            var savePath = _dialogService.ShowSaveFileDialog("Excel file (*.xlsx)|*.xlsx",
                $"{SelectedClient.Name}_Annex_{DateTime.Now:yyyyddMMHHmmss}");
            if (string.IsNullOrEmpty(savePath)) return;

            var reportData = GenerateReport(printParam);

            if(reportData == null)return;

            _excelReportService.SaveAsAsync(reportData, savePath);

            var fileInfo = new FileInfo(savePath);
            _dialogService.ShowInfoDialog($"Report saved{Environment.NewLine}{Environment.NewLine}{fileInfo.Name}");
        }

        private void AnnexTotal()
        {
            if(SelectedDepartment == null) return;

            var report = new AnnexTotalReport()
            {
                Items = new List<AnnexTotalItemReport>(),
                DepartmentName = SelectedDepartment.Name,
                ClientName = SelectedClient.Name,
            };


            _reportService.ShowReportPreview(report);
        }
    }
}