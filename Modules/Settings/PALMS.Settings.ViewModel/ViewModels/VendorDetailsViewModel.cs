using System;
using System.Collections.Concurrent;
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
        public string Error { get; }
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
        public ManualResetEvent RfidThread { get; set; }


        #region parameters

        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

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
        private string _clothToTakeOut;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private bool _itemReadyToPass;
        private ClientLinenEntityViewModel _waitingLinen;
        private ConveyorItemViewModel _hangingLinen;


        public ClientLinenEntityViewModel WaitingLinen
        {
            get => _waitingLinen;
            set => Set(() => WaitingLinen, ref _waitingLinen, value);
        }
        public bool ItemReadyToPass
        {
            get => _itemReadyToPass;
            set => Set(() => ItemReadyToPass, ref _itemReadyToPass, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(() => ClientLinens, ref _clientLinens, value);
        }
        public string ClothToTakeOut
        {
            get => _clothToTakeOut;
            set => Set(() => ClothToTakeOut, ref _clothToTakeOut, value);
        }
        public RfidCommon Impinj
        {
            get => _impinj;
            set => Set(() => Impinj, ref _impinj, value);
        }
        public ConveyorItemViewModel HangingLinen
        {
            get => _hangingLinen;
            set => Set(() => HangingLinen, ref _hangingLinen, value);
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

        #endregion


        public async Task InitializeAsync()
        {
            var linen = await _dataService.GetAsync<ClientLinen>(x=> x.MasterLinen);
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => ClientLinens = linens.ToObservableCollection());
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

            InitializeAsync();

            Plc1Ip = "192.168.250.1";
            Plc2Ip = "192.168.250.2";
            Plc3Ip = "192.168.250.3";
            LocalIp = GetLocalIp();

            Plc1Error = 99;
            Plc2Error = 99;
            Plc3Error = 99;
            Plc1Thread = new ManualResetEvent(true);
            RfidThread = new ManualResetEvent(true);
            AddBeltItems();
            Impinj = new RfidCommon();
            PropertyChanged += OnPropertyChanged;
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsAutoMode))
            {
                if (IsAutoMode)
                {
                    Task.Factory.StartNew(RunAutoMode);
                }
            }
        }

        private ObservableCollection<ConveyorItemViewModel> SortBeltItems(int beltNumb)
        {
            return BeltItems?.Where(x => x.BeltNumber == beltNumb)?.ToObservableCollection();
        }

#region Conveyor Start
        public void StartConveyor()
        {
            Plc1.Start();
            Belt1.Start();
            Belt2.Start();

            Task.Factory.StartNew(CheckCloth);
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

            GetBelt1SlotNumb = Belt1.GetNowPoint();
            GetBelt2SlotNumb = Belt2.GetNowPoint();

            Belt1.GetBasePoint();
            Belt1.GetHanginpoint();
            Belt1.GetNowPoint();

            Belt2.GetBasePoint();
            Belt2.GetHanginpoint();
            Belt2.GetNowPoint();
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

        private void AddBeltItems()
        {
            BeltItems = new ObservableCollection<ConveyorItemViewModel>();

            for (var i = 1; i <= 600; i++)
            {
                BeltItems.Add(new ConveyorItemViewModel()
                {
                    BeltNumber = 1,
                    SlotNumber = i,
                    IsEmpty = true,
                });
            }

            for (var i = 1; i <= 780; i++)
            {
                BeltItems.Add(new ConveyorItemViewModel()
                {
                    BeltNumber = 2,
                    SlotNumber = i,
                });
            }
        }

        private void ResetClothCount()
        {
            //Plc1.ResetWaitHangNum();
            //ItemReadyToPass = false;

            var addNew = _resolverService.Resolve<AddNewLinenViewModel>();

            addNew.InitializeAsync();
            var showDialog = _dialogService.ShowDialog(addNew);
            if (!showDialog) return;
        }

        #endregion

#region Plc1 Waiting 

        private void CheckCloth()
        {
            while (true)
            {
                if (ItemReadyToPass)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                if (Plc1.GetClotheReady())
                {
                    CheckClothReady();
                }
            }
        }

        private void CheckClothReady()
        {
            var i = Plc1.GetWaitHangNum();

            if (i > 1)
            {
                ShowDialogWaitingNumb();
            }

            // проверить и вызвать метод считывателя. 
            CheckLinenRfid();
        }

        private void ShowDialogWaitingNumb()
        {
            _dispatcher.RunInMainThread(() =>
            {
                Plc1Thread.Reset();

                if (_dialogService.ShowWarnigDialog(
                    "There are more then 1 hanger in belt sorting point\n Please remove all hangers and pass again " +
                    "\n\n Press ok once all done"))
                {
                    ResetClothCount();
                }

                Plc1Thread.Set();
            });

            Plc1Thread.WaitOne();
        }

        #endregion

#region Rfid Reader

        public void CheckLinenRfid()
        {
            _data.Clear();

            _data = Impinj.GetSortedTags(1000);
            var tagReport = _data.FirstOrDefault(x => x.Key == 1).Value.Keys;

            if (tagReport.Count > 1)
            {
                ShowDialogTagNumbMore();
                CheckLinenRfid();
            }

            if (tagReport.Count == 0)
            {
                ShowDialogTagNumbZero();
                CheckLinenRfid();
            }

            CheckLinen(tagReport.FirstOrDefault());
        }

        private void CheckLinen(string tag)
        {
            var clientLinen = ClientLinens?.FirstOrDefault(x => x.Tag == tag);

            //check tag in existing list of linen, if false give option to add item

            if (clientLinen == null)
            {
                ShowDialogAddLinen();
                RfidThread.Reset();
                RfidThread.WaitOne();

                CheckLinen(tag);
            }

            WaitingLinen = clientLinen;
            ItemReadyToPass = true;
        }

        public void ShowDialogAddLinen()
        {
            _dispatcher.RunInMainThread(() =>
            {
                RfidThread.Reset();

                if (_dialogService.ShowQuestionDialog("No linen with current Tag \n Want to add Linen?"))
                {
                    var addNew = _resolverService.Resolve<AddNewLinenViewModel>();

                    addNew.InitializeAsync();
                    var showDialog = _dialogService.ShowDialog(addNew);
                    if (!showDialog) return;

                    ClientLinens.AddRange(addNew.GetNewClient());
                }

                RfidThread.Set();
            });

            Plc1Thread.WaitOne();
        }

        public void ShowDialogTagNumbMore()
        {
            _dispatcher.RunInMainThread(() =>
            {
                RfidThread.Reset();

                _dialogService.ShowWarnigDialog("More then 1 chip in Antenna 1");

                RfidThread.Set();
            });

            RfidThread.WaitOne();
        }

        public void ShowDialogTagNumbZero()
        {
            _dispatcher.RunInMainThread(() =>
            {
                RfidThread.Reset();

                _dialogService.ShowWarnigDialog("No Tag in linen");

                RfidThread.Set();
            });

            RfidThread.WaitOne();
        }
        #endregion

#region Hanging Method

        private void SendToBelt(int beltNumb, int slotNumb)
        {
            FinsTcp belt = null;

            if (beltNumb == 1)
                belt = Belt1;
            else if (beltNumb == 2) belt = Belt2;
            if(belt == null) return;

            SetHangingItem(beltNumb, slotNumb);

            Plc1.Sorting(beltNumb);

            HangToBeltSlot(belt, slotNumb);

            HangingLinen = null;
        }

        private void HangToBeltSlot(FinsTcp belt, int slotNumb)
        {
            // Подготовка слота 
            if (belt.GetNowPoint() != slotNumb)
            {
                belt.SetNowPoint(slotNumb);
                Thread.Sleep(500);
                belt.GotoPoint();

                // ожыдание окончание подготовки слота в линии
                while (belt.DialState())
                {
                    Thread.Sleep(500);
                }
            }

            // начала загрузки в слот
            while (!belt.GetClotheInHook())
            {
                belt.Hang_In();
                Thread.Sleep(500);
            }

            // ожыдание окончание загрузки в слот
            while (belt.GetClotheInHook())
            {
                Thread.Sleep(500);
            }
        }

        private void SetHangingItem(int beltNumb, int slotNumb)
        {
            var beltItem = BeltItems.FirstOrDefault(x => x.BeltNumber == beltNumb && x.SlotNumber == slotNumb);

            beltItem?.Update(WaitingLinen);

            HangingLinen = beltItem;

            WaitingLinen = null;
            ItemReadyToPass = false;
        }

        #endregion

        #region Manual/Auto Mode

        private void ManualSendToBelt1()
        {
            if (ItemReadyToPass == false)
            {
                _dialogService.ShowInfoDialog("No Linen");
                return;
            }

            //перед отправкой проверить на заполненость линии и слота
            if (Belt1Items[SetBelt1SlotNumb-1].IsEmpty)
            {
                SendToBelt(1, SetBelt1SlotNumb);
                return;
            }

            _dialogService.ShowInfoDialog("Selected slot is not empty");
        }

        private void ManualSendToBelt2()
        {

        }

        private void AutoMode()
        {
            IsAutoMode = !IsAutoMode;
        }

        private void RunAutoMode()
        {
            while (IsAutoMode)
            {
                if (!ItemReadyToPass)
                {
                    Thread.Sleep(500); continue;
                }
                var currentSlot = 0;

                //Belt 1 проверка слота
                if (Belt1Items.Any(x=> x.IsEmpty))
                {
                    currentSlot = Belt1.GetNowPoint();

                    while (Belt1Items[currentSlot-1].IsEmpty == false)
                    {
                        currentSlot++;
                        if (currentSlot > 600) currentSlot = 1;
                    }
                    SendToBelt(1, currentSlot);
                }

                //Belt 2 проверка слота
                //else if (Belt2Items.Any(x => x.IsEmpty))
                //{
                //    currentSlot = Belt2.GetNowPoint();

                //    while (Belt2Items[currentSlot-1].IsEmpty == false)
                //    {
                //        currentSlot++;
                //        if (currentSlot > 780) currentSlot = 1;
                //    }
                //    SendToBelt(2, currentSlot);
                //}
            }
        }
        #endregion

#region Packing Uniform

        private void TakeClothBelt1()
        {
            if (String.IsNullOrEmpty(ClothToTakeOut)) return;

            TakeCloth(Belt1, ClothToTakeOut);
        }

        private void TakeClothBelt2()
        {
            if (String.IsNullOrEmpty(ClothToTakeOut)) return;

            TakeCloth(Belt2, ClothToTakeOut);
        }

        private void PackCloth()
        {
            Plc1.Packclothes();
        }

        private void TakeCloth(FinsTcp belt, string linenList)
        {
            // Take out clothes
            belt.TakeOutClothes(linenList);

            //Working state of the clothes taking device
            while (belt.GetTakeOutClothesState())
            {
                Thread.Sleep(500);
            }
            
        }


        private void AutoPacking()
        {

        }

        private void GetStaffList()
        {

        }

        private string GetStaffUniformSlots()
        {
            var list = "";

            

            return list;
        }
        #endregion

    }
}
