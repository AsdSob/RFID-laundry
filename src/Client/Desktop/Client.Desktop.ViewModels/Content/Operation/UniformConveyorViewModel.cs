using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using PlcControl;

namespace Client.Desktop.ViewModels.Content.Operation
{
    public class UniformConveyorViewModel : ViewModelBase
    {
        #region parameters

        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

        private readonly IDialogService _dialogService;

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

        public UniformConveyorViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SendToBelt1Command = new RelayCommand (ManualSendToBelt(1));
            SendToBelt2Command = new RelayCommand (ManualSendToBelt(2));
            ConnectConveyorCommand = new RelayCommand(ConnectConveyor);
            StartConveyorCommand = new RelayCommand(StartConveyor);
            StopConveyorCommand = new RelayCommand(StopConveyor);
            AutoModeCommand = new RelayCommand(AutoMode);
            ResetClothCountCommand = new RelayCommand(ResetClothCount);

            Plc1Ip = "192.168.250.1";
            Plc2Ip = "192.168.250.2";
            Plc3Ip = "192.168.250.3";
            LocalIp = GetLocalIp();

            AddBeltItems();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsAutoMode))
            {
                //RunAutoMode();
            }

        }

        private ObservableCollection<ConveyorItemViewModel> SortBeltItems(int beltNumb)
        {
            return BeltItems?.Where(x => x.BeltNumber == beltNumb)?.ToObservableCollection();
        }

        public void StartConveyor()
        {
            Plc1.Start();
            Belt1.Start();
            Belt2.Start();
        }

        public void StopConveyor()
        {
            Plc1.Start();
            Belt1.Start();
            Belt2.Start();
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
            Plc1.conn(LocalIp, Plc1Ip, 9600);
            Belt1.conn(LocalIp, Plc2Ip, 9600);
            Belt2.conn(LocalIp, Plc3Ip, 9600);

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
            Plc1.ResetWaitHangNum();
        }

        #region Plc1 Waiting and linen data

        public void CheckClothReady()
        {
            if (!Plc1.GetClotheReady())
                return;

            var i = Plc1.GetWaitHangNum();

            if (i > 1)
            {
                if (_dialogService.ShowWarnigDialog(
                    "There are more then 1 hanger in belt sorting point\n Please remove all hangers and pass again \n\n Press ok once all done"))
                {
                    ResetClothCount();
                }
                else
                {
                    CheckClothReady();
                }
            }

            // проверить и вызвать метод считывателя. 
            ////CheckLinenRfid();
        }

        private bool CheckBeltSlot(int beltNumb, int slotNumb)
        {
            return beltNumb == 1
                ? Belt1Items.FirstOrDefault(x => x.SlotNumber == slotNumb).IsEmpty
                : Belt2Items.FirstOrDefault(x => x.SlotNumber == slotNumb).IsEmpty;
        }

        //public void CheckLinenRfid()
        //{
        //    var tagReport = _data.FirstOrDefault(x => x.Key == 1).Value.Keys;

        //    if (tagReport.Count > 1)
        //    {
        //        //ToDo: show errror msg, manual check the linen
        //    }

        //    var clientLinen = ClientLinens.FirstOrDefault(x => x.RfidTag == tagReport.FirstOrDefault());

        //    //check tag in existing list of linen, if false give option to add item
        //    if (clientLinen == null)
        //    {
        //        //TODO: RfidTag doesnt exsit in DB, Ask to new add in DB?
        //    }
        //    else
        //    {
        //        PassingItem = clientLinen;
        //    }
        //}
        #endregion

        #region Manual Mode

        private Action ManualSendToBelt(int i)
        {
            if (PassingItem == null)
            {
                _dialogService.ShowInfoDialog("No Linen");
            }

            var beltNumb = i;

            //перед отправкой проверить на заполненость линии и слота

            switch (beltNumb)
            {
                case 1 when CheckBeltSlot(beltNumb, SetBelt1SlotNumb):
                    SendToBelt(Belt1, SetBelt1SlotNumb);
                    break;
                case 2 when CheckBeltSlot(beltNumb, SetBelt2SlotNumb):
                    SendToBelt(Belt2, SetBelt2SlotNumb);
                    break;
            }

            return null;
        }

        private void SendToBelt(FinsTcp belt, int slotNumb)
        {
            // Подготовка слота 
            if (belt.GetNowPoint() != slotNumb)
            {
                belt.SetNowPoint(slotNumb);
                System.Threading.Thread.Sleep(500);
                belt.GotoPoint();

                // ожыдание окончание подготовки слота в линии
                while (belt.DialState())
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
            System.Threading.Thread.Sleep(2000);

            // начала загрузки в слот
            while (!belt.GetClotheInHook())
            {
                belt.Hang_In();
                System.Threading.Thread.Sleep(1000);
            }

            // ожыдание окончание загрузки в слот
            while (belt.GetClotheInHook())
            {
                System.Threading.Thread.Sleep(500);
            }
        }
        #endregion





        private void AddData(int antenna, string epc, DateTime time)
        {
            // проверка ест ли словарь антенны 
            if (!_data.TryGetValue(antenna, out ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>> val))
            {
                val = new ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>();

                _data.TryAdd(antenna, val);
            }

            // проверка на наличие чипа в словаре
            if (!val.TryGetValue(epc, out Tuple<DateTime?, DateTime?> times))
            {
                times = new Tuple<DateTime?, DateTime?>(time, null);
                val.TryAdd(epc, times);
            }
            else
            {
                val.TryUpdate(epc, new Tuple<DateTime?, DateTime?>(times.Item1, time), times);
                //данные можно сохранять в БД, но метке можно обнулить
            }

            // если метка повторно падает в считыватель, то нужно предыдущие данные сохранять в БД
        }
    }
}
