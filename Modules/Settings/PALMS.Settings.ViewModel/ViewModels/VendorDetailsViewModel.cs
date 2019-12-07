using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Settings.ViewModel.Common;
using PALMS.Settings.ViewModel.EntityViewModels;
using PALMS.Settings.ViewModel.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;


namespace PALMS.Settings.ViewModel.ViewModels
{
    public class VendorDetailsViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public string Name => "PrimeInfo";
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;
        public ManualResetEvent Plc1Thread { get; set; }
        public ManualResetEvent DialogThread { get; set; }
        public ManualResetEvent UpdateLinenThread { get; set; }

#region parameters

        private int _plc1Error, _plc2Error, _plc3Error;
        private FinsTcp _plc1;
        private FinsTcp _belt1;
        private FinsTcp _belt2;
        private string _localIp;
        private string _plc3Ip;
        private string _plc2Ip;
        private string _plc1Ip;
        private int _getBelt1SlotNumb;
        private int _getBelt2SlotNumb;
        private ObservableCollection<ConveyorItemViewModel> _beltItems;
        private bool _isAutoMode;
        private int _setBelt1SlotNumb;
        private int _setBelt2SlotNumb;
        private RfidCommon _impinj;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private bool _isItemPrepared;
        private int? _waitingLinenId;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientStaffEntityViewModel> _staff;
        private ObservableCollection<string> _tags;
        private bool _isAutoPackMode;
        private ClientStaffEntityViewModel _selectedStaff;
        private ObservableCollection<PackedLinenViewModel> _packedLinens;
        private PackedLinenViewModel _selectedPackedLinen;
        private ObservableCollection<ClientStaffEntityViewModel> _sortedStaff;

        public ObservableCollection<ClientStaffEntityViewModel> SortedStaff
        {
            get => _sortedStaff;
            set => Set(() => SortedStaff, ref _sortedStaff, value);
        }
        public PackedLinenViewModel SelectedPackedLinen
        {
            get => _selectedPackedLinen;
            set => Set(() => SelectedPackedLinen, ref _selectedPackedLinen, value);
        }
        public ObservableCollection<PackedLinenViewModel> PackedLinens
        {
            get => _packedLinens;
            set => Set(() => PackedLinens, ref _packedLinens, value);
        }
        public ClientStaffEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public bool IsAutoPackMode
        {
            get => _isAutoPackMode;
            set => Set(() => IsAutoPackMode, ref _isAutoPackMode, value);
        }
        public ObservableCollection<string> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
        }
        public ObservableCollection<ClientStaffEntityViewModel> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public int? WaitingLinenId
        {
            get => _waitingLinenId;
            set => Set(() => WaitingLinenId, ref _waitingLinenId, value);
        }
        public bool IsItemPrepared
        {
            get => _isItemPrepared;
            set => Set(() => IsItemPrepared, ref _isItemPrepared, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(() => ClientLinens, ref _clientLinens, value);
        }
        public RfidCommon Impinj
        {
            get => _impinj;
            set => Set(() => Impinj, ref _impinj, value);
        }
        public int SetBelt2SlotNumb
        {
            get => _setBelt2SlotNumb;
            set => Set(() => SetBelt2SlotNumb, ref _setBelt2SlotNumb, value);
        }
        public int SetBelt1SlotNumb
        {
            get => _setBelt1SlotNumb;
            set => Set(() => SetBelt1SlotNumb, ref _setBelt1SlotNumb, value);
        }
        public bool IsAutoMode
        {
            get => _isAutoMode;
            set => Set(() => IsAutoMode, ref _isAutoMode, value);
        }
        public ObservableCollection<ConveyorItemViewModel> BeltItems
        {
            get => _beltItems;
            set => Set(() => BeltItems, ref _beltItems, value);
        }
        public int GetBelt2SlotNumb
        {
            get => _getBelt2SlotNumb;
            set => Set(() => GetBelt2SlotNumb, ref _getBelt2SlotNumb, value);
        }
        public int GetBelt1SlotNumb
        {
            get => _getBelt1SlotNumb;
            set => Set(() => GetBelt1SlotNumb, ref _getBelt1SlotNumb, value);
        }
        public int Plc3Error
        {
            get => _plc3Error;
            set => Set(() => Plc3Error, ref _plc3Error, value);
        }
        public int Plc2Error
        {
            get => _plc2Error;
            set => Set(() => Plc2Error, ref _plc2Error, value);
        }
        public int Plc1Error
        {
            get => _plc1Error;
            set => Set(() => Plc1Error, ref _plc1Error, value);
        }
        public string LocalIp
        {
            get => _localIp;
            set => Set(() => LocalIp, ref _localIp, value);
        }
        public FinsTcp Plc1
        {
            get => _plc1;
            set => Set(() => Plc1, ref _plc1, value);
        }
        public FinsTcp Belt1
        {
            get => _belt1;
            set => Set(() => Belt1, ref _belt1, value);
        }
        public FinsTcp Belt2
        {
            get => _belt2;
            set => Set(() => Belt2, ref _belt2, value);
        }
        public string Plc1Ip
        {
            get => _plc1Ip;
            set => Set(() => Plc1Ip, ref _plc1Ip, value);
        }
        public string Plc2Ip
        {
            get => _plc2Ip;
            set => Set(() => Plc2Ip, ref _plc2Ip, value);
        }
        public string Plc3Ip
        {
            get => _plc3Ip;
            set => Set(() => Plc3Ip, ref _plc3Ip, value);
        }

        public ObservableCollection<ConveyorItemViewModel> Belt1Items => SortBeltItems(1);
        public ObservableCollection<ConveyorItemViewModel> Belt2Items => SortBeltItems(2);

        public RelayCommand ConnectConveyorCommand { get; }
        public RelayCommand StartConveyorCommand { get; }
        public RelayCommand StopConveyorCommand { get; }
        public RelayCommand AutoModeCommand { get; }
        public RelayCommand ResetClothCountCommand { get; }
        public RelayCommand SendToBelt1Command { get; }
        public RelayCommand SendToBelt2Command { get; }
        public RelayCommand AutoPackModeCommand { get; }
        public RelayCommand ManualPackCommand { get; }
        public RelayCommand PackClothCommand { get; }
        public RelayCommand RemoveAllSLotsCommand { get; }
        public RelayCommand RemovePackedLinenCommand { get; }
        public RelayCommand RemovePackedLinensCommand { get; }
        public RelayCommand GetStaffListCommand { get; }
        public RelayCommand UpdateWaitingSlotCommand { get; }

        #endregion

        public async Task InitializeAsync()
        {
            var linen = await _dataService.GetAsync<ClientLinen>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => ClientLinens = linens.ToObservableCollection());

            var master = await _dataService.GetAsync<MasterLinen>();
            var masters = master.Select(x => new MasterLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => MasterLinens = masters.ToObservableCollection());

            var staff = await _dataService.GetAsync<ClientStaff>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Staff = staffs.ToObservableCollection());

            var conveyorItem = await _dataService.GetAsync<ConveyorItem>();
            var conveyorItems = conveyorItem.Select(x => new ConveyorItemViewModel(x));
            _dispatcher.RunInMainThread(() => BeltItems = conveyorItems.ToObservableCollection());

            BeltItems.ForEach(SubscribeItem);
            SortStaff();

            foreach (var item in BeltItems.Where(x=>x.ClientLinenId != null))
            {
                var clientLinen = ClientLinens.FirstOrDefault(x => x.Id == item.ClientLinenId);

                item.Tag = clientLinen?.Tag;
                item.StaffId = clientLinen?.StaffId;
            }
        }

        public VendorDetailsViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService, IResolver resolver)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            SendToBelt1Command = new RelayCommand(ManualSendToBelt1);
            SendToBelt2Command = new RelayCommand(ManualSendToBelt2);
            ConnectConveyorCommand = new RelayCommand(ConnectConveyor);
            StartConveyorCommand = new RelayCommand(StartConveyor);
            StopConveyorCommand = new RelayCommand(StopConveyor);
            AutoModeCommand = new RelayCommand(AutoMode);
            ResetClothCountCommand = new RelayCommand(ResetClothCount);
            AutoPackModeCommand = new RelayCommand(AutoPacking);
            ManualPackCommand = new RelayCommand(ManualTakeCloth);
            PackClothCommand = new RelayCommand((PackClothHard));
            RemoveAllSLotsCommand = new RelayCommand(RemoveAllBeltItems);
            RemovePackedLinenCommand = new RelayCommand(RemovePackedLinen);
            RemovePackedLinensCommand = new RelayCommand(RemovePackedLinens);
            GetStaffListCommand = new RelayCommand(SortStaff);
            UpdateWaitingSlotCommand = new RelayCommand(UpdateWaitingSlot);

            InitializeAsync();

            Plc1Ip = "192.168.250.1";
            Plc2Ip = "192.168.250.2";
            Plc3Ip = "192.168.250.3";
            LocalIp = GetLocalIp();

            Plc1Error = 99;
            Plc2Error = 99;
            Plc3Error = 99;
            Plc1Thread = new ManualResetEvent(false);
            DialogThread = new ManualResetEvent(false);
            UpdateLinenThread = new ManualResetEvent(false);
            Impinj = new RfidCommon();
            PropertyChanged += OnPropertyChanged;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedStaff))
            {
                SelectBeltItemsForPacking();
            }

            if (e.PropertyName == nameof(IsAutoPackMode))
            {
                if (IsAutoPackMode)
                {
                    Task.Factory.StartNew(RunAutoPacking);
                }
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ConveyorItemViewModel)) return;

            if (e.PropertyName == nameof(ConveyorItemViewModel.ClientLinenId))
            {
                var beltItem = sender as ConveyorItemViewModel;

                if (beltItem.ClientLinenId != null)
                {
                    var linen = ClientLinens.FirstOrDefault(x => x.Id == beltItem.ClientLinenId);

                    beltItem.StaffId = linen?.StaffId;
                    beltItem.Tag = linen?.Tag;
                }
                else
                {
                    beltItem.StaffId = null;
                    beltItem.Tag = null;
                }

                SaveConveyorItem(beltItem);

                RaisePropertyChanged(() => Belt1Items);
                RaisePropertyChanged(() => Belt2Items);
                SortStaff();
            }
        }

        private void UnSubscribeItem(ConveyorItemViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(ConveyorItemViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }
        
#region Conveyor Start

        public void StartConveyor()
        {
            Plc1.Start();
            Belt1.Start();
            Belt2.Start();

            Task.Factory.StartNew(RunCircleCheckCloth);
        }

        public void StopConveyor()
        {
            Plc1.Stop();
            Belt1.Stop();
            Belt2.Stop();
        }

        public void ConnectConveyor()
        {
            Plc1 = new FinsTcp(LocalIp, Plc1Ip, 9600);
            Belt1 = new FinsTcp(LocalIp, Plc2Ip, 9600);
            Belt2 = new FinsTcp(LocalIp, Plc3Ip, 9600);

            CheckConveyorConnection();
        }

        private void CheckConveyorConnection()
        {
            Plc1Error = Plc1.conn(LocalIp, Plc1Ip, 9600);
            Plc2Error = Belt1.conn(LocalIp, Plc2Ip, 9600);
            Plc3Error = Belt2.conn(LocalIp, Plc3Ip, 9600);

            UpdateWaitingSlot();

            Belt1.SetModel(0);
            Belt1.GetBasePoint();
            Belt1.GetHanginpoint();

            Belt2.SetModel(0);
            Belt2.GetBasePoint();
            Belt2.GetHanginpoint();

            RaisePropertyChanged(() => Belt1Items);
            RaisePropertyChanged(() => Belt2Items);
        }

        private string GetLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        #endregion


#region Data Methods

        private void SaveConveyorItem(ConveyorItemViewModel item)
        {
            if (item.HasChanges())
            {
                item.AcceptChanges();
                _dataService.AddOrUpdateAsync(item.OriginalObject);
            }
        }

        private void SaveClientLinenStatus(ClientLinenEntityViewModel item)
        {
            if (item.HasChanges())
            {
                item.AcceptChanges();
                _dataService.AddOrUpdateAsync(item.OriginalObject);
            }
        }

        private void SetDataLinenPacked(List<ConveyorItemViewModel> beltItems)
        {
            if(beltItems == null || beltItems.Count == 0) return;

            foreach (var item in beltItems)
            {
                item.ClientLinenId = null;
                item.StaffId = null;
                item.Tag = null;
            }

            beltItems.ForEach(AddPackedLinen);
            SortStaff();
        }

        private void RemoveAllBeltItems()
        {
            var items = GetHangedBeltItems().ToList();

            foreach (var item in items)
            {
                item.ClientLinenId = null;
                item.StaffId = null;
                item.Tag = null;
            }
        }

        private void AddPackedLinen(ConveyorItemViewModel conveyorItem)
        {
            if (conveyorItem == null) return;

            var clientLinen = ClientLinens.FirstOrDefault(x => x.Id == conveyorItem.ClientLinenId);
            var masterLinen = MasterLinens?.FirstOrDefault(x => x.Id == clientLinen?.MasterLinenId);

            var newPackedLinen = new PackedLinenViewModel()
            {
                OriginalObject = clientLinen?.OriginalObject,
                Id = clientLinen.Id,
                Name = masterLinen.Name,
                ParentId = clientLinen.StaffId,
            };
            PackedLinens.Add(newPackedLinen);

            if (!PackedLinens.Any(x => x.Id == clientLinen.StaffId && x.ParentId == null))
            {
                var staff = Staff.FirstOrDefault(x => x.Id == clientLinen.StaffId);
                var newPackedStaff = new PackedLinenViewModel()
                {
                    OriginalObject = staff?.OriginalObject,
                    Id = staff.Id,
                    Name = staff.Name,
                    ParentId = null,
                };
                PackedLinens.Add(newPackedStaff);
            }
        }

        private void RemovePackedLinen()
        {
            if (SelectedPackedLinen == null) return;

            PackedLinens.Remove(SelectedPackedLinen);
        }

        private void RemovePackedLinens()
        {
            PackedLinens = new ObservableCollection<PackedLinenViewModel>();
        }

        private async void UpdateClientLinenEntity()
        {
            ClientLinens = new ObservableCollection<ClientLinenEntityViewModel>();
            var linen = await _dataService.GetAsync<ClientLinen>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => ClientLinens = linens.ToObservableCollection());

            UpdateLinenThread.Set();
        }

        private void SetWaitingLinen(int? linenId)
        {
            WaitingLinenId = linenId;
            IsItemPrepared = true;
        }

        private void SetDataLinenHangedToSlot(int beltNumb, int slotNumb)
        {
            var beltItem = BeltItems.FirstOrDefault(x => x.BeltNumber == beltNumb && x.SlotNumber == slotNumb);

            if (beltItem != null) beltItem.ClientLinenId = WaitingLinenId;

            IsItemPrepared = false;
            WaitingLinenId = null;
        }

        #endregion

#region Common Methods

        private ObservableCollection<ConveyorItemViewModel> SortBeltItems(int beltNumb)
        {
            var beltItems = BeltItems?.Where(x => x.HasItem && x.BeltNumber == beltNumb).ToObservableCollection();
            var ordered = beltItems?.OrderBy(x => x.SlotNumber).ToObservableCollection();
            return ordered;
        }

        private void SortStaff()
        {
            var items = new ObservableCollection<ClientStaffEntityViewModel>();

            if (BeltItems == null || BeltItems.Count == 0) return;

            foreach (var beltItem in BeltItems?.Where(x=> x.StaffId != null))
            {
                if(items.Any(x=> x.Id == beltItem.StaffId)) { continue;}

                items.Add(Staff.FirstOrDefault(x=> x.Id == beltItem.StaffId));
            }

            SortedStaff = items;
        }

        private ObservableCollection<ConveyorItemViewModel> GetHangedBeltItems()
        {
            var items = new ObservableCollection<ConveyorItemViewModel>();

            items.AddRange(BeltItems.Where(x => x.HasItem));

            return items;
        }

        private void SelectBeltItemsForPacking()
        {
            if(SelectedStaff == null) return;

            BeltItems.Where(x=> x.IsSelected).ForEach(x=> x.IsSelected = false);

            foreach (var beltItem in BeltItems.Where(x=> x.StaffId == SelectedStaff.Id))
            {
                beltItem.IsSelected = true;
            }
        }

        private void ResetClothCount()
        {
            Plc1.ResetWaitHangNum();
            IsItemPrepared = false;
        }

        private bool IsSlotHasItem(int beltNumb, int slotNumb)
        {
            if (slotNumb == 0) return false;

            var first = BeltItems.FirstOrDefault(x => x.BeltNumber == beltNumb && x.SlotNumber == slotNumb);

            return first != null && first.HasItem;
        }

        private bool IsBeltFull(int beltNumb)
        {
            var beltItems = BeltItems.Where(x => x.BeltNumber == beltNumb);

             return beltItems.All(x => x.HasItem);
        }

        private void UpdateWaitingSlot()
        {
            GetBelt1SlotNumb = Belt1.GetNowPoint();
            GetBelt2SlotNumb = Belt2.GetNowPoint();
        }

        #endregion

#region Plc1 Waiting 

        private void RunCircleCheckCloth()
        {
            while (true)
            {
                Thread.Sleep(500);

                if (IsItemPrepared)
                {
                    if (IsAutoMode)
                    {
                        RunAutoMode();
                    }
                    continue;
                }

                if (!Plc1.GetClotheReady()) continue;

                var waitingCount = Plc1.GetWaitHangNum();
                if (waitingCount > 1)
                {
                    DialogThread.Reset();
                    ShowDialogWaitingNumb();
                    DialogThread.WaitOne();
                    continue;
                }
                    
                var tag = CheckLinenRfid();

                if (String.IsNullOrWhiteSpace(tag))
                    continue;

                var linen = CheckLinenByTag(tag);
                if (linen == null)
                    continue;

                SetWaitingLinen(linen.Id);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// вызвать метод считывателя и проверика
        /// </summary>
        public string CheckLinenRfid()
        {
            Tags = new AsyncObservableCollection<string>();
            Impinj.ReadDuringTime(2500);
            Tags = Impinj.GetAntennaTags(1).ToObservableCollection();

            if (Tags.Count == 0)
            {
                DialogThread.Reset();
                ShowDialogTagNumbZero();
                DialogThread.WaitOne();

                return null;
            }

            if (Tags.Count > 1)
            {
                DialogThread.Reset();
                ShowDialogTagNumbMore();
                DialogThread.WaitOne();
                return null;
            }

            return Tags.FirstOrDefault();
        }

        /// <summary>
        /// метод проверяет наличия белья в списке. если бельё не найдено то вызывает окно для добавление нового белья
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private ClientLinenEntityViewModel CheckLinenByTag(string tag)
        {
            var clientLinen = ClientLinens?.FirstOrDefault(x => x.Tag == tag);
            //check tag in existing in list of linen, if false give option to add item
            
            if (clientLinen == null)
            {
                UpdateLinenThread.Reset();

                if (ShowDialogAddLinen())
                {
                   UpdateClientLinenEntity();
                }

                UpdateLinenThread.WaitOne();

                CheckLinenByTag(tag);
            }
            return clientLinen;
        }
        
        #endregion

#region Waiting ShowDialogs

        private void ShowDialogWaitingNumb()
        {
            _dispatcher.RunInMainThread(() =>
            {
                if (_dialogService.ShowWarnigDialog(
                    "There are more then 1 hanger in belt sorting point\n Please remove all hangers and pass again " +
                    "\n\n Press ok once all done"))
                {
                    ResetClothCount();
                }

                DialogThread.Set();
            });
        }

        private bool ShowDialogAddLinen()
        {
            DialogThread.Reset();

            var linenAdded = false;
            _dispatcher.RunInMainThread(() =>
            {
                if (_dialogService.ShowQuestionDialog("No linen with current Tag \n Want to add Linen?"))
                {
                    var addNew = _resolverService.Resolve<AddNewLinenViewModel>();

                    addNew?.InitializeAsync();
                    var showDialog = _dialogService.ShowDialog(addNew);
                    if (!showDialog) return;

                    linenAdded = true;
                }

                DialogThread.Set();
            });

            DialogThread.WaitOne();

            return linenAdded;
        }

        private void ShowDialogTagNumbMore()
        {
            _dispatcher.RunInMainThread(() =>
            {
                _dialogService.ShowWarnigDialog("More then 1 chip in Antenna 1");

                DialogThread.Set();
            });

        }

        private void ShowDialogTagNumbZero()
        {
            _dispatcher.RunInMainThread(() =>
            {
                _dialogService.ShowWarnigDialog("No Tag in linen");

                DialogThread.Set();
            });
        }
        #endregion

#region Hanging Method

        private void SendToBelt(int beltNumb, int slotNumb)
        {
            FinsTcp belt = null;
            switch (beltNumb)
            {
                case 1:
                    belt = Belt1;
                    break;
                case 2:
                    belt = Belt2;
                    break;
            }

            Plc1.Sorting(beltNumb);
            HangToBeltSlot(belt, slotNumb);

            SetDataLinenHangedToSlot(beltNumb, slotNumb);
        }

        private void HangToBeltSlot(FinsTcp belt, int slotNumb)
        {
            PrepareBeltSlot(belt, slotNumb);

            // начала загрузки в слот
            var isHangWorking = false;
            while (!isHangWorking)
            {
                belt.Hang_In();
                Thread.Sleep(500);
                isHangWorking = belt.GetClotheinhook();
            }

            var getClothInHook = false;
            while (!getClothInHook)
            {
                Thread.Sleep(300);
                getClothInHook = belt.GetClotheinhook();
            }
        }

        private void PrepareBeltSlot(FinsTcp belt, int slotNumb)
        {
            // Подготовка слота 
            if (belt.GetNowPoint() != slotNumb)
            {
                belt.GetModel();
                belt.SetNowPoint(slotNumb);
                belt.GotoPoint();

                // ожыдание окончание подготовки слота в линии
                while (belt.DialState())
                {
                    Thread.Sleep(500);
                }
            }
        }

        #endregion

#region Hanging Manual/Auto Mode

        private void ManualSendToBelt1()
        {
            if (SetBelt1SlotNumb > 600 || SetBelt1SlotNumb < 2)
            {
                _dialogService.ShowInfoDialog("Slot number must be between 2 and 600");
                return;
            }
            ManualSendToBelt(1, SetBelt1SlotNumb);
        }

        private void ManualSendToBelt2()
        {
            if (SetBelt1SlotNumb > 776 || SetBelt1SlotNumb < 2)
            {
                _dialogService.ShowInfoDialog("Slot number must be between 2 and 776");
                return;
            }
            ManualSendToBelt(2, SetBelt1SlotNumb);
        }

        private void ManualSendToBelt(int beltNumb, int slotNumb)
        {
            if (IsItemPrepared == false)
            {
                _dialogService.ShowInfoDialog("Linen is not ready");
                return;
            }

            if (IsSlotHasItem(beltNumb, SetBelt1SlotNumb))
            {
                _dialogService.ShowInfoDialog("Selected slot is not empty");
                return;
            }

            SendToBelt(beltNumb, slotNumb);
        }

        private void AutoMode()
        {
            if (IsAutoPackMode)
            {
                _dialogService.ShowWarnigDialog("Auto Packing mode is ON!!");
                return;
            }
            IsAutoMode = !IsAutoMode;
        }

        private void RunAutoMode()
        {
            if (!IsBeltFull(1))
            {
                var currentSlot = Belt1.GetNowPoint();
                while (true)
                {
                    if (currentSlot <= 1 || currentSlot >= 601)
                        currentSlot = 2;

                    if (IsSlotHasItem(1, currentSlot))
                    {
                        currentSlot++;
                        continue;
                    }
                    SendToBelt(1, currentSlot);
                    break;
                }
            }

            if (!IsBeltFull(2))
            {
                var currentSlot = Belt2.GetNowPoint();
                while (true)
                {
                    if (currentSlot <= 1 || currentSlot >= 776)
                        currentSlot = 2;

                    if (IsSlotHasItem(2, currentSlot))
                    {
                        currentSlot++;
                        continue;
                    }
                    SendToBelt(2, currentSlot);
                    break;
                }
            }
        }

        #endregion

#region Packing Methods

        private void TakeClothFromBelt(FinsTcp belt, List<ConveyorItemViewModel> items)
        {
            var linenString = GetStringUniformSlots(items);

            // Take out clothes
            belt.TakeOutClothes(linenString);

            //Working state of the clothes taking device
            var takeOutClothState = false;
            while (!takeOutClothState)
            {
                Thread.Sleep(500);
                takeOutClothState = belt.GetTakeOutClothesState();
            }

            SetDataLinenPacked(items);

            //Thread.Sleep(2000);
            PackClothHard();
        }

        private void PackClothHard()
        {
            Plc1.Packclothes();
        }

        private string GetStringUniformSlots(List<ConveyorItemViewModel> linens)
        {
            var slotList = "";

            foreach (var linen in linens)
            {
                slotList += $"{linen.SlotNumber}, ";
            }

            slotList = slotList.Substring(0, slotList.Length - 2);

            return slotList;
        }

        #endregion

#region Paking Manual/Auto Mode
        
        private void ManualTakeCloth()
        {
            //TODO: сделать паралельный сбор белья

            var items1 = Belt1Items.Where(x => x.IsSelected).ToList();
            ManualTakeClothBelt(Belt1, items1);

            var items2 = Belt2Items.Where(x => x.IsSelected).ToList();
            ManualTakeClothBelt(Belt2, items2);
        }

        private void ManualTakeClothBelt(FinsTcp belt, List<ConveyorItemViewModel> items)
        {
            if (items == null || items.Count == 0)
            {
                _dialogService.ShowInfoDialog("No linen was selected");
                return;
            }

            TakeClothFromBelt(belt, items);
        }

        private void AutoPacking()
        {
            if (IsAutoMode)
            {
                _dialogService.ShowWarnigDialog("Auto Hanging Mode is ON!");
                return;
            }
            IsAutoPackMode = !IsAutoPackMode;
        }

        private void RunAutoPacking()
        {
            while (IsAutoPackMode)
            {
                var staffId = GetStaffList();
                if (staffId == null)
                {
                    IsAutoPackMode = false;
                    break;
                }

                var linens = BeltItems?.Where(x => x.StaffId == staffId).ToList();

                if(linens == null || linens.Count == 0) continue;

                var slotsBelt1 = linens.Where(x => x.BeltNumber == 1).ToList();
                var slotsBelt2 = linens.Where(x => x.BeltNumber == 2).ToList();

                //TODO: запустить два метода линии 1 и 2 в паралельных потоках
                if (slotsBelt1.Count != 0)
                {
                    TakeClothFromBelt(Belt1, slotsBelt1);
                }

                if (slotsBelt2.Count != 0)
                {
                    TakeClothFromBelt(Belt1, slotsBelt2);
                }
            }
        }

        private int? GetStaffList()
        {
            var beltItems = GetHangedBeltItems();

            if (beltItems == null || beltItems.Count == 0)
                return null;

            var staffId = beltItems.First(x => x.StaffId != null).StaffId;
            return staffId;
        }

        #endregion

    }
}
