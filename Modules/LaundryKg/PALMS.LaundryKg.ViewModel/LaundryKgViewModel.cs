using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.LaundryKg.ViewModel.EntityViewModel;
using PALMS.LaundryKg.ViewModel.Windows;
using PALMS.Reports.Common;
using PALMS.Reports.Model.ReportTypes;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using LaundryKgModel = PALMS.Data.Objects.Received_data.LaundryKg;

namespace PALMS.LaundryKg.ViewModel
{
    public class LaundryKgViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private readonly IReportService _reportService;

        public Action<bool> CloseAction { get; set; }
        private readonly SemaphoreSlim _saveLockSemaphore = new SemaphoreSlim(1, 1);

        private List<Client> _clients;
        private List<UnitViewModel> _departmentTypes;
        private ObservableCollection<LaundryKgEntityModel> _laundryKgs;
        private LaundryKgEntityModel _selectedLaundryKg;
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
        public LaundryKgEntityModel SelectedLaundryKg
        {
            get => _selectedLaundryKg;
            set => Set(ref _selectedLaundryKg, value);
        }
        public ObservableCollection<LaundryKgEntityModel> LaundryKgs
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

        public RelayCommand DeleteCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DuplicateCommand { get; }
        public RelayCommand NoteReportCommand { get; }
        public RelayCommand ChangeDetailsCommand { get; }
        public RelayCommand PrintCommand { get; }

        public ObservableCollection<LaundryKgEntityModel> SortedLaundryKg => SortLaundryKg();

        public LaundryKgViewModel(IResolver resolver, IDataService dataService, IDispatcher dispatcher, IDialogService dialog, IReportService reportService)
        {
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));

            DeleteCommand = new RelayCommand(DeleteLaundryKg, () => SelectedLaundryKg != null);
            AddCommand = new RelayCommand(AddNewLaundryKg, () => SelectedClient != null);
            DuplicateCommand = new RelayCommand(DuplicateLaundryKg, () => SelectedLaundryKg != null);
            NoteReportCommand = new RelayCommand(NoteReportWindow);
            ChangeDetailsCommand = new RelayCommand(ChangeDetail);
            PrintCommand = new RelayCommand(Print);

            SelectedWashingDate = DateTime.Now;
            SelectedKgTypeId = 1;
            SelectedLinenTypeId = 1;
            SelectedShiftId = 1;

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var client = await _dataService.GetAsync<Client>(x => x.Active);
                _dispatcher.RunInMainThread(() => Clients = client.OrderBy(x => x.OrderNumber).ToList());

                var laundryKg = await _dataService.GetAsync<LaundryKgModel>();
                var laundryKgs = laundryKg.Select(x => new LaundryKgEntityModel(x));
                _dispatcher.RunInMainThread(() => LaundryKgs = laundryKgs.OrderBy(x => x.Id).ToObservableCollection());

                LaundryKgs.ForEach(SubscribeItem);
                LaundryKgs.CollectionChanged += LaundryKgOnCollectionChanged;

                DepartmentTypes = EnumExtentions.GetValues<DepartmentTypeEnum>();
                StaffShifts = EnumExtentions.GetValues<StaffShiftEnum>();
                KgTypes = EnumExtentions.GetValues<KgTypeEnum>();
                LinenTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
                LinenTypes.Remove(LinenTypes.FirstOrDefault(x => x.Id == (int) DeliveryTypeEnum.Express));

                RaisePropertyChanged(()=> SortedLaundryKg);
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
            switch (e.PropertyName)
            {
                case nameof(SelectedLinenTypeId):
                case nameof(SelectedShiftId):
                case nameof(SelectedWashingDate):
                case nameof(SelectedKgTypeId):
                    RaisePropertyChanged(() => SortedLaundryKg);
                    break;
                case nameof(SelectedLaundryKg):
                    DeleteCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        private void SubscribeItem(LaundryKgEntityModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void UnSubscribeItem(LaundryKgEntityModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is LaundryKgEntityModel item)) return;
            SaveLaundryKg(item);
        }

        private void LaundryKgOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<LaundryKgEntityModel>())
                {
                    SubscribeItem(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<LaundryKgEntityModel>())
                {
                    UnSubscribeItem(item);
                }
            }
        }

        public ObservableCollection<LaundryKgEntityModel> SortLaundryKg()
        {
            if (LaundryKgs == null) return null;
            var sortedLaundryKgs = LaundryKgs
                .Where(x => x.WashingDate.Year == SelectedWashingDate.Year &&
                            x.WashingDate.DayOfYear == SelectedWashingDate.DayOfYear &&
                            x.KgTypeId == SelectedKgTypeId &&
                            x.ShiftId == SelectedShiftId &&
                            x.LinenTypeId == SelectedLinenTypeId).ToObservableCollection();

            foreach (var client in Clients)
            {
                if (sortedLaundryKgs.Any(x => x.ClientId == client.Id))
                {
                    sortedLaundryKgs.FirstOrDefault(x => x.ClientId == client.Id).ClientOrderNumber =
                        client.OrderNumber;
                    continue;
                }

                var newLaundryKgs = new LaundryKgEntityModel()
                {
                    ClientId = client.Id,
                    KgTypeId = SelectedKgTypeId,
                    LinenTypeId = SelectedLinenTypeId,
                    ShiftId = SelectedShiftId,
                    WashingDate = SelectedWashingDate,
                    ClientOrderNumber = client.OrderNumber
                };

                LaundryKgs.Add(newLaundryKgs);
                sortedLaundryKgs.Add(newLaundryKgs);
            }

            return sortedLaundryKgs.OrderBy(x=> x.ClientOrderNumber).ToObservableCollection();
        }

        public void SaveLaundryKg(LaundryKgEntityModel item)
        {
            if (item == null || !item.HasChanges()) return;

            if (item.OriginalObject.IsNew)
            {
                if ((item.Tunnel1 + item.Tunnel2 + item.ExtFnB + item.ExtGuest + item.ExtLinen + item.ExtManager + item.ExtUniform) > 0)
                {
                    Save(item);
                }
            }
            else
            {
                Save(item);
            }
        }

        private async void Save(LaundryKgEntityModel item)
        {
            item.AcceptChanges();

            await _saveLockSemaphore.WaitAsync();

            try
            {
                await _dataService.AddOrUpdateAsync(item.OriginalObject);
            }
            finally
            {
                _saveLockSemaphore.Release();
            }
        }

        public void AddNewLaundryKg()
        {
            var newLaundryKg = new LaundryKgEntityModel()
            {
                ClientId = SelectedClient.Id,
                WashingDate = SelectedWashingDate,
                LinenTypeId = (int)DeliveryTypeEnum.Normal,
                KgTypeId = (int)KgTypeEnum.Clean,
                ShiftId = (int)StaffShiftEnum.Day,
            };

            LaundryKgs.Add(newLaundryKg);
        }

        public void DuplicateLaundryKg()
        {
            var laundryKg = SelectedLaundryKg;
            if (laundryKg == null) return;

            var newLaundryKg = new LaundryKgEntityModel()
            {
                ClientId = laundryKg.ClientId,
                WashingDate = laundryKg.WashingDate,
                ShiftId = laundryKg.ShiftId,
                KgTypeId = laundryKg.KgTypeId,
                LinenTypeId = laundryKg.LinenTypeId,
            };

            LaundryKgs.Add(newLaundryKg);
            RaisePropertyChanged(() => SortedLaundryKg);
            SelectedLaundryKg = newLaundryKg;
        }

        public void DeleteLaundryKg()
        {
            var laundryKg = SelectedLaundryKg;
            if (laundryKg == null) return;

            if (!_dialogService.ShowQuestionDialog("Do you want to DELETE selected row?")) return;

            laundryKg.Tunnel1 = 0.00;
            laundryKg.Tunnel2 = 0.00;
            laundryKg.ExtLinen = 0.00;
            laundryKg.ExtManager = 0.00;
            laundryKg.ExtLinen = 0.00;
            laundryKg.ExtGuest = 0.00;
            laundryKg.ExtFnB = 0.00;
        }

        public async void NoteReportWindow()
        {
            var noteReport = _resolverService.Resolve<NoteReportWindowViewModel>();

            try
            {
                await noteReport.InitializeAsync();
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

            var showDialog = _dialogService.ShowDialog(noteReport);

            if (showDialog) { }
        }

        private bool HasValue(ObservableCollection<LaundryKgEntityModel> laundryKgs)
        {
            if (laundryKgs == null || !laundryKgs.Any()) return false;

            var value = 0.00;

            foreach (var laundryKg in laundryKgs)
            {
                value += laundryKg.Tunnel1 + laundryKg.Tunnel2 + laundryKg.ExtLinen + laundryKg.ExtFnB +
                         laundryKg.ExtGuest + laundryKg.ExtManager + laundryKg.ExtUniform;
            }

            return value > 0;
        }

        public async void ChangeDetail()
        {
            var oldLaundryKgs = SortedLaundryKg;

            if (!HasValue(oldLaundryKgs))
            {
                _dialogService.ShowInfoDialog("There are no values not change");
                return;
            }

            var changeWindow = _resolverService.Resolve<ChangeDetailViewModel>();
            await changeWindow.InitializeAsync(SelectedShiftId, SelectedKgTypeId, SelectedWashingDate);
            var showDialog = _dialogService.ShowDialog(changeWindow);
            if (!showDialog)
            {
                return;
            }

            var newDate = changeWindow.GetChangedDate();
            var newShiftId = changeWindow.GetChangedShift();
            var newKgType = changeWindow.GetChangedKgType();

            var checkLaundryKgs = new ObservableCollection<LaundryKgEntityModel>();

            if (newDate.HasValue)
            {
                checkLaundryKgs = LaundryKgs.Where(x => x.WashingDate == newDate).ToObservableCollection();
            }

            if (newShiftId.HasValue)
            {
                checkLaundryKgs = checkLaundryKgs.Where(x => x.ShiftId == newShiftId).ToObservableCollection();
            }

            if (newKgType.HasValue)
            {
                checkLaundryKgs = checkLaundryKgs.Where(x => x.KgTypeId == newKgType).ToObservableCollection();
            }

            if (HasValue(checkLaundryKgs))
            {
                _dialogService.ShowInfoDialog("Details Cannot be changed, please check changed values");
                return;
            }

            foreach (var laundryKg in oldLaundryKgs)
            {
                if (newDate != null)
                {
                    laundryKg.WashingDate = (DateTime) newDate;
                }
                if (newKgType != null)
                {
                    laundryKg.KgTypeId = (int)newKgType;
                }
                if (newShiftId != null)
                {
                    laundryKg.ShiftId = (int)newShiftId;
                }
            }

            _dialogService.ShowInfoDialog("All changes applied");

            RaisePropertyChanged(()=> SortedLaundryKg);
        }


        public void Print()
        {
            var laundryKgs = SortedLaundryKg;

            var laundryKgReport = new LaundryKgReport()
            {
                WashingDate = SelectedWashingDate,
                LaundryKgType = KgTypes.FirstOrDefault(x=> x.Id == SelectedKgTypeId)?.Name,
                Shift = StaffShifts.FirstOrDefault(x=> x.Id == SelectedShiftId)?.Name,
                WashingType = LinenTypes.FirstOrDefault(x=> x.Id == SelectedLinenTypeId)?.Name,
                Items = new List<LaundryKgItems>(),
            };

            var id = 0;
            foreach (var kg in laundryKgs)
            {
                var item = new LaundryKgItems()
                {
                    Id = id++,
                    Tunnel1 = kg.Tunnel1,
                    Tunnel2 = kg.Tunnel2,
                    ExtLinen = kg.ExtLinen,
                    ExtFnB = kg.ExtFnB,
                    ExtGuest = kg.ExtGuest,
                    ExtUniform = kg.ExtUniform,
                    ClientName = Clients.FirstOrDefault(x=> x.Id == kg.ClientId)?.ShortName,
                    ExtManagement = kg.ExtManager,
                };
                laundryKgReport.Items.Add(item);
            }

            _reportService.ShowReportPreview(laundryKgReport);
        }
    }
}
