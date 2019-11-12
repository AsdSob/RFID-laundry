using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.TPS.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PlcControl;

namespace PALMS.TPS.ViewModel
{
    public class TpsViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly ICanExecuteMediator _canExecuteMediator;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;
        private ObservableCollection<CClientViewModel> _clients;
        private ObservableCollection<CLinenViewModel> _linens;
        private ObservableCollection<CClientLinenViewModel> _clientLinens;
        private FinsTcp _plc1;
        private FinsTcp _plc2;
        private FinsTcp _plc3;
        private string _localIp;
        private string _plcIp1;
        private string _plcIp2;
        private string _plcIp3;
        private string _plcError1;
        private string _plcError2;
        private string _plcError3;
        private ObservableCollection<BeltItemViewModel> _belt1Items;
        private ObservableCollection<BeltItemViewModel> _belt2Items;
        private ObservableCollection<BeltItemViewModel> _packedItems;
        private CClientLinenViewModel _waitingLinen;
        private int _belt1SlotNumber;
        private int _belt2SlotNumber;
        private int _setBelt1SlotNumb;
        private int _setBelt2SlotNumb;

        public FinsTcp plc1test { get; set; }
        public int SetBelt2SlotNumb
        {
            get => _setBelt2SlotNumb;
            set => Set(ref _setBelt2SlotNumb, value);
        }
        public int SetBelt1SlotNumb
        {
            get => _setBelt1SlotNumb;
            set => Set(ref _setBelt1SlotNumb, value);
        }
        public int Belt2SlotNumber
        {
            get => _belt2SlotNumber;
            set => Set(ref _belt2SlotNumber, value);
        }
        public int Belt1SlotNumber
        {
            get => _belt1SlotNumber;
            set => Set(ref _belt1SlotNumber, value);
        }
        public CClientLinenViewModel WaitingLinen
        {
            get => _waitingLinen;
            set => Set(ref _waitingLinen, value);
        }
        public ObservableCollection<BeltItemViewModel> PackedItems
        {
            get => _packedItems;
            set => Set(ref _packedItems, value);
        }
        public ObservableCollection<BeltItemViewModel> Belt2Items
        {
            get => _belt2Items;
            set => Set(ref _belt2Items, value);
        }
        public ObservableCollection<BeltItemViewModel> Belt1Items
        {
            get => _belt1Items;
            set => Set(ref _belt1Items, value);
        }
        public string PlcError3
        {
            get => _plcError3;
            set => Set(ref _plcError3, value);
        }
        public string PlcError2
        {
            get => _plcError2;
            set => Set(ref _plcError2, value);
        }
        public string PlcError1
        {
            get => _plcError1;
            set => Set(ref _plcError1, value);
        }
        public string PlcIp3
        {
            get => _plcIp3;
            set => Set(ref _plcIp3, value);
        }
        public string PlcIp2
        {
            get => _plcIp2;
            set => Set(ref _plcIp2, value);
        }
        public string PlcIp1
        {
            get => _plcIp1;
            set => Set(ref _plcIp1, value);
        }
        public string LocalIp
        {
            get => _localIp;
            set => Set(ref _localIp, value);
        }
        public FinsTcp Plc3
        {
            get => _plc3;
            set => Set(ref _plc3, value);
        }
        public FinsTcp Plc2
        {
            get => _plc2;
            set => Set(ref _plc2, value);
        }
        public FinsTcp Plc1
        {
            get => _plc1;
            set => Set(ref _plc1, value);
        }
        public ObservableCollection<CClientLinenViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(ref _clientLinens, value);
        }
        public ObservableCollection<CLinenViewModel> Linens
        {
            get => _linens;
            set => Set(ref _linens, value);
        }
        public ObservableCollection<CClientViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public RelayCommand ConnectionCommand { get; }
        public RelayCommand ConnectionStopCommand { get; }
        public RelayCommand StaffIdCommand { get; }
        public RelayCommand ManualHangBelt1 { get; }
        public RelayCommand ManualHangBelt2 { get; }

        
        public TpsViewModel (IDispatcher dispatcher, ICanExecuteMediator canExecuteMediator, IResolver resolver, IDialogService dialogService, IDataService dataService)
        {
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            ManualHangBelt1 = new RelayCommand(ManualMode1);
            ManualHangBelt2 = new RelayCommand(ManualMode2);
            ConnectionCommand = new RelayCommand(Start);

            PropertyChanged += OnPropertyChanged;

            PlcIp1 = "192.168.250.1";
            PlcIp2 = "192.168.250.2";
            PlcIp3 = "192.168.250.3";
            LocalIp = "192.168.250.201";

        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        public async Task InitializeAsync()
        {

        }

        public void CheckConnection()
        {
            PlcError1 = Plc1.conn(LocalIp, PlcIp1, 9600).ToString();
            PlcError2 = Plc2.conn(LocalIp, PlcIp2, 9600).ToString();
            PlcError3 = Plc3.conn(LocalIp, PlcIp3, 9600).ToString();
        }

        public void Start()
        {
            Plc1 = new FinsTcp(LocalIp, PlcIp1, 9600);
            Plc2 = new FinsTcp(LocalIp, PlcIp2, 9600);
            Plc3 = new FinsTcp(LocalIp, PlcIp3, 9600);

            Plc1.Start();
            Plc2.Start();
            Plc3.Start();

            CheckConnection();
        }

        public void Stop()
        {
            Plc1.Stop();
            Plc2.Stop();
            Plc3.Stop();

            CheckConnection();
        }

        public void Reset()
        {
            Plc2.Reset();
            Plc3.Reset();

            CheckConnection();
        }

        public void Clear()
        {
            Plc2.Clear();
            Plc3.Clear();

            CheckConnection();
        }

        public bool CheckClothReady()
        {
            bool clotheReady = Plc1.GetClotheReady();

            return clotheReady;
        }

        public void SendToLine(int line)
        {
            if(line == 0) return;

            Plc1.Sorting(line);
        }

        public int GetWaitHangNum(FinsTcp plc)
        {
            if (plc == null) return 0;

            var i = plc.GetWaitHangNum();

            return i;
        }

        public void ManualMode1()
        {
            _dispatcher.RunInMainThread(() => Plc1.GetClotheReady());
            
            if (Belt1Items.Any(x => x.SlotNumber == Belt1SlotNumber))
            {
                _dialogService.ShowWarnigDialog("This slot is not empty");
                return;
            }

            SendToLine(1);
            Plc2.HangUpToPoint(Belt1SlotNumber);
            Belt1Items.FirstOrDefault(x => x.SlotNumber == Belt1SlotNumber)?.Update(WaitingLinen);
            WaitingLinen.StatusId = (int) LinenStatus.Conveyor;
        }

        public void ManualMode2()
        {

        }

        public void AutoMode()
        {
            if (Belt1Items.Any(x=> x.OriginalObject == null))
            {
                SendToLine(1);
            }
            else
            {
                SendToLine(2);
            }


        }
    }
}
