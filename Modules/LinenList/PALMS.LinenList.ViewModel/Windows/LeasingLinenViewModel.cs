using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.LinenModel;
using PALMS.LinenList.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.LinenList.ViewModel
{
    public class LeasingLinenViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;

        private ObservableCollection<LinenListModelViewModel> _linenLists;
        private ObservableCollection<MasterLinensViewModel> _masterLinens;
        private ObservableCollection<FullLeasingLinenViewModel> _leasingLinens;
        private ObservableCollection<FullLeasingLinenViewModel> _sortedLeasingLinen;

        public ObservableCollection<FullLeasingLinenViewModel> SortedLeasingLinen
        {
            get => _sortedLeasingLinen;
            set => Set(ref _sortedLeasingLinen, value);
        }
        public ObservableCollection<FullLeasingLinenViewModel> LeasingLinens
        {
            get => _leasingLinens;
            set => Set(ref _leasingLinens, value);
        }
        public ObservableCollection<MasterLinensViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public ObservableCollection<LinenListModelViewModel> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public Action<bool> CloseAction { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand CloseCommand { get; }

        public LeasingLinenViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            CloseCommand = new RelayCommand(Close);
        }

        public async Task InitializeAsync(ObservableCollection<LinenListModelViewModel> linenList,
            ObservableCollection<MasterLinensViewModel> masterLinens)
        {
            SortedLeasingLinen = new ObservableCollection<FullLeasingLinenViewModel>();
            LinenLists = linenList;
            MasterLinens = masterLinens;
            GetData();
        }

        public async void GetData()
        {
            var leasinglinen = await _dataService.GetAsync<LeasingLinen>();
            var leasingLinens = leasinglinen.Select(x => new FullLeasingLinenViewModel(x));
            _dispatcher.RunInMainThread(() => LeasingLinens = leasingLinens.ToObservableCollection());

            foreach (var linen in LinenLists)
            {
                var leasingLinen = LeasingLinens.FirstOrDefault(x => x.LinenListId == linen.Id);

                if (leasingLinen == null)
                {
                    SortedLeasingLinen.Add(new FullLeasingLinenViewModel()
                    {
                        LinenList = linen.OriginalObject,
                        OriginalPrice = 0,
                        LinenListId = linen.Id,
                        Name = MasterLinens?.FirstOrDefault(x => x.Id == linen.MasterLinenId)?.Name,
                    });
                }
                else
                {
                    leasingLinen.Name = MasterLinens?.FirstOrDefault(x => x.Id == linen.MasterLinenId)?.Name;
                    SortedLeasingLinen.Add(leasingLinen);
                }
            }
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                CloseAction?.Invoke(true);
        }

        public async void Save()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to save all changes ? "))
                return;
            var leasingLinens = SortedLeasingLinen.Where(x => x.OriginalPrice != 0).ToObservableCollection();
            leasingLinens?.ForEach(x=> x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(leasingLinens?.Select(x=> x.OriginalObject));

            CloseAction?.Invoke(true);
        }

        public void Cancel()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to cancel all changes? ")) return;

            LeasingLinens.ForEach(x=> x.Reset());
        }

    }
}
