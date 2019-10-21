using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.Received_data;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Reports.ViewModel.Windows
{
    public class LaundryKgWindowViewModel: ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private readonly IExcelReportService _excelReportService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        public Action<bool> CloseAction { get; set; }
        private readonly SemaphoreSlim _saveLockSemaphore = new SemaphoreSlim(1, 1);

        private List<Client> _clients;
        private List<UnitViewModel> _departmentTypes;
        private ObservableCollection<LaundryKgViewModel> _laundryKgs;
        private LaundryKgViewModel _selectedLaundryKg;
        private Client _selectedClient;
        private List<UnitViewModel> _staffShifts;
        private List<UnitViewModel> _kgTypes;
        private List<UnitViewModel> _linenTypes;

        private int _selectedShiftId;
        private int _selectedKgTypeId;
        private DateTime _selectedWashingDate;
        private int _selectedLinenTypeId;

        public DateTime SelectedWashingDate
        {
            get => _selectedWashingDate;
            set => Set(ref _selectedWashingDate, value);
        }
        public int SelectedLinenTypeId
        {
            get => _selectedLinenTypeId;
            set => Set(ref _selectedLinenTypeId, value);
        }
        public int SelectedKgTypeId
        {
            get => _selectedKgTypeId;
            set => Set(ref _selectedKgTypeId, value);
        }
        public int SelectedShiftId
        {
            get => _selectedShiftId;
            set => Set(ref _selectedShiftId, value);
        }
        public List<UnitViewModel> LinenTypes
        {
            get => _linenTypes;
            set => Set(ref _linenTypes, value);
        }
        public List<UnitViewModel> KgTypes
        {
            get => _kgTypes;
            set => Set(ref _kgTypes, value);
        }
        public List<UnitViewModel> StaffShifts
        {
            get => _staffShifts;
            set => Set(ref _staffShifts, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public LaundryKgViewModel SelectedLaundryKg
        {
            get => _selectedLaundryKg;
            set => Set(ref _selectedLaundryKg, value);
        }
        public ObservableCollection<LaundryKgViewModel> LaundryKgs
        {
            get => _laundryKgs;
            set => Set(ref _laundryKgs, value);
        }
        public List<UnitViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(ref _departmentTypes, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public RelayCommand GetReportCommand { get; }


        public LaundryKgWindowViewModel(IResolver resolver, IDataService dataService, IDispatcher dispatcher, IDialogService dialog, IExcelReportService excelReport)
        {
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _excelReportService = excelReport ?? throw new ArgumentNullException(nameof(excelReport));

            GetReportCommand = new RelayCommand(GetReport);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var client = await _dataService.GetAsync<Client>( x=> x.Active);
                _dispatcher.RunInMainThread(() => Clients = client.OrderBy(x => x.ShortName).ToList());

                var laundryKg = await _dataService.GetAsync<LaundryKg>();
                var laundryKgs = laundryKg.Select(x => new LaundryKgViewModel(x));
                _dispatcher.RunInMainThread(() => LaundryKgs = laundryKgs.OrderBy(x => x.Id).ToObservableCollection());

                DepartmentTypes = EnumExtentions.GetValues<DepartmentTypeEnum>();
                StaffShifts = EnumExtentions.GetValues<StaffShiftEnum>();
                KgTypes = EnumExtentions.GetValues<KgTypeEnum>();
                LinenTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();

                SelectedWashingDate = DateTime.Now;
                SelectedKgTypeId = 1;
                SelectedLinenTypeId = 1;
                SelectedShiftId = 1;
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

        }

        public void GetReport()
        {
            //if (DateEnd < DateStart)
            //{
            //    _dialogService.ShowInfoDialog("Please enter proper dates");
            //    return;
            //}

            //var horizontal = SetHorizontalData();
            //var vertical = SetVerticalData();

            //var data = new ExcelData()
            //{
            //    Name = $"{SelectedX.Name} by {SelectedY.Name} {SelectedValueType.Name} report",
            //    PrintDate = DateTime.Now,
            //    ReportType = ReportType.SimpleExcel,
            //    HorizontalData = horizontal,
            //    VerticalData = vertical,
            //};
            //SaveExcel(data);
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
