using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ConsoleAppTimer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TestConveyor.Svoyo.EntityModels;
using TestConveyor.Svoyo.Services;

namespace TestConveyor.Svoyo
{
    public class Conveyor : ViewModelBase
    {
        #region parameters

        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

        private int Plc1Error, Plc2Error, Plc3Error;
        private TestTimer Tmr;

        private ImpinjSpeedway _reader;
        private ObservableCollection<ConveyorItemViewModel> _beltItems;
        private ConveyorItemViewModel _selectedConveyorItem;
        private string _plc1Ip;
        private string _plc2Ip;
        private string _plc3Ip;
        private string _localIp;
        private FinsTcp _plc1;
        private FinsTcp _belt1;
        private FinsTcp _belt2;
        private int _belt1SlotNumb;
        private int _belt2SlotNumb;
        private ClientLinenViewModel _passingItem;
        private ObservableCollection<ClientLinenViewModel> _clientLinens;
        private bool _isAutoMode;
        private Tuple<bool, ClientLinenViewModel> _hangingItem;

        public Tuple<bool, ClientLinenViewModel> HangingItem
        {
            get => _hangingItem;
            set => Set(ref _hangingItem, value);
        }
        public bool IsAutoMode
        {
            get => _isAutoMode;
            set => Set(ref _isAutoMode, value);
        }
        public ObservableCollection<ClientLinenViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(ref _clientLinens, value);
        }
        public ClientLinenViewModel PassingItem
        {
            get => _passingItem;
            set => Set(ref _passingItem, value);
        }
        public int Belt2SlotNumb
        {
            get => _belt2SlotNumb;
            set => Set(ref _belt2SlotNumb, value);
        }
        public int Belt1SlotNumb
        {
            get => _belt1SlotNumb;
            set => Set(ref _belt1SlotNumb, value);
        }
        public FinsTcp Belt2
        {
            get => _belt2;
            set => Set(ref _belt2, value);
        }
        public FinsTcp Belt1
        {
            get => _belt1;
            set => Set(ref _belt1, value);
        }
        public FinsTcp Plc1
        {
            get => _plc1;
            set => Set(ref _plc1, value);
        }
        public string LocalIp
        {
            get => _localIp;
            set => Set(ref _localIp, value);
        }
        public string Plc3Ip
        {
            get => _plc3Ip;
            set => Set(ref _plc3Ip, value);
        }
        public string Plc2Ip
        {
            get => _plc2Ip;
            set => Set(ref _plc2Ip, value);
        }
        public string Plc1Ip
        {
            get => _plc1Ip;
            set => Set(ref _plc1Ip, value);
        }
        public ConveyorItemViewModel SelectedConveyorItem
        {
            get => _selectedConveyorItem;
            set => Set(ref _selectedConveyorItem, value);
        }
        public ObservableCollection<ConveyorItemViewModel> BeltItems
        {
            get => _beltItems;
            set => Set(ref _beltItems, value);
        }
        public ImpinjSpeedway Reader
        {
            get => _reader;
            set => Set(ref _reader, value);
        }

        public ObservableCollection<ConveyorItemViewModel> Belt1Items => SortBeltItems(1);

        public ObservableCollection<ConveyorItemViewModel> Belt2Items => SortBeltItems(2);

        #endregion

        public RelayCommand ConnectConveyorCommand { get; }
        public RelayCommand StartConveyorCommand { get; }
        public RelayCommand StopConveyorCommand { get; }
        public RelayCommand AutoModeCommand { get; }
        public RelayCommand ResetClothCountCommand { get; }
        public RelayCommand<object> SendToBeltCommand { get; }

        public Conveyor()
        {
            SendToBeltCommand = new RelayCommand<object>(SendToBelt);
            ConnectConveyorCommand = new RelayCommand(ConnectConveyor);
            StartConveyorCommand = new RelayCommand(StartConveyor);
            StopConveyorCommand = new RelayCommand(StopConveyor);
            AutoModeCommand = new RelayCommand(AutoMode);
            ResetClothCountCommand = new RelayCommand(ResetClothCount);

            LocalIp = GetLocalIp();
            Tmr = new TestTimer();
        }

        private ObservableCollection<ConveyorItemViewModel> SortBeltItems(int beltNumb)
        {
            return BeltItems.Where(x=> x.BeltNumber == beltNumb).ToObservableCollection();
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
            Plc1Error = Plc1.conn(LocalIp, Plc1Ip, 9600);
            Plc2Error = Belt1.conn(LocalIp, Plc2Ip, 9600);
            Plc3Error = Belt2.conn(LocalIp, Plc3Ip, 9600);

            Belt1SlotNumb = Belt1.GetNowPoint();
            Belt2SlotNumb = Belt2.GetNowPoint();

            Belt1.GetBasePoint();
            Belt1.GetHanginpoint();
            Belt1.GetNowPoint();

            Belt2.GetBasePoint();
            Belt2.GetHanginpoint();
            Belt2.GetNowPoint();
        }

        public static string GetLocalIp()
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

        #region Plc1 Waiting and linen data
        public void CheckClothReady()
        {
            if (!Plc1.GetClotheReady())
                return;

            var i = Plc1.GetWaitHangNum();
            if (i > 1)
            {
                //TODO: show error msg. manual check the linen
            }

            ////CheckLinenRfid();
            // проверить и вызвать метод считывателя. 
        }

        public void CheckLinenRfid()
        {
            var tagReport = _data.FirstOrDefault(x => x.Key == 1).Value.Keys;

            if (tagReport.Count > 1)
            {
                //ToDo: show errror msg, manual check the linen
            }

            var clientLinen = ClientLinens.FirstOrDefault(x => x.RfidTag == tagReport.FirstOrDefault());

            //check tag in existing list of linen, if false give option to add item
            if (clientLinen == null)
            {
                //TODO: RfidTag doesnt exsit in DB, Ask to new add in DB?
            }
            else
            {
                PassingItem = clientLinen;
            }
        }

        #endregion

        #region Manual Mode
        private void SendToBelt(object param)
        {
            if (PassingItem == null)
            {
                //TODo: проверка на наличие готового белья
            }

            var beltNumb = int.Parse(param.ToString());

            //TODo: перед отправкой проверить на заполненость линии и слота

            if (beltNumb == 1 && Belt1Items.Any(x=> x.IsEmpty))
            {
                SendToBelt1(Belt1);
            }
            else if (beltNumb == 2 && Belt2Items.Any(x=> x.IsEmpty) )
            {
                SendToBelt1(Belt2);
            }
            else
            {
                //показать окно с увидомлением что все слоты заполнени
            }
        }

        private void SendToBelt1(FinsTcp belt)
        {
            // Подготовка слота 
            if (belt.GetNowPoint() != Belt1SlotNumb)
            {
                belt.SetNowPoint(Belt1SlotNumb);
                Tmr.WaitTime(500);
                belt.GotoPoint();

                // ожыдание окончание подготовки слота в линии
                while (belt.DialState())
                {
                    Tmr.WaitTime(500);
                }
            }
            Tmr.WaitTime(2000);

            // ожыдание начала загрузки в слот
            while (!belt.GetClotheInHook())
            {
                belt.Hang_In();
                Tmr.WaitTime(1000);
            }

            // ожыдание окончание загрузки в слот
            while (belt.GetClotheInHook())
            {
                Tmr.WaitTime(500);
            }
        }
        #endregion

        #region Auto Mode

        private void AutoMode()
        {
            IsAutoMode = !IsAutoMode;
        }

        private void RunAutoMode()
        {
            if(!IsAutoMode || PassingItem == null) return;

            while (IsAutoMode)
            {
                
            }




        }


        #endregion



        private void ResetClothCount()
        {
            Plc1.ResetWaitHangNum();
        }


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
