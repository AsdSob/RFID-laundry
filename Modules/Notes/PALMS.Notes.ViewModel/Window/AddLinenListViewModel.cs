using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.LinenModel;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Notes.ViewModel.Window
{
    public class AddLinenListViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private ObservableCollection<LinenListViewModel> _linensList;
        private ObservableCollection<MasterLinenViewModel> _masterLinens;
        private DepartmentViewModel _selectedDepartment;
        private MasterLinenViewModel _selectedMasterLinen;
        private LinenListViewModel _selectedLinenList;
        public bool IsChanged { get; set; }

        public LinenListViewModel SelectedLinenList
        {
            get => _selectedLinenList;
            set => Set(ref _selectedLinenList, value);
        }
        public MasterLinenViewModel SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(ref _selectedMasterLinen, value);
        }
        public DepartmentViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<MasterLinenViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public ObservableCollection<LinenListViewModel> LinensList
        {
            get => _linensList;
            set => Set(ref _linensList, value);
        }

        public ObservableCollection<MasterLinenViewModel> SortedMasterLinen => SortMasterLinen();
        public ObservableCollection<LinenListViewModel> SortedLinenList => SortLinenList();

        public RelayCommand AddLinenListCommand { get; }
        public RelayCommand SetLinenListCommand { get; }
        public RelayCommand CloseCommand { get; }

        public async Task InitializeAsync(DepartmentViewModel department)
        {
            SelectedDepartment = department;

            var masterLinen = await _dataService.GetAsync<MasterLinen>();
            var masterLinens = masterLinen.Select(x => new MasterLinenViewModel(x));
            _dispatcher.RunInMainThread(() => MasterLinens = masterLinens.ToObservableCollection());

            var linenList = await _dataService.GetAsync<LinenList>(x=> x.DepartmentId == SelectedDepartment.Id);
            var linensList = linenList.Select(x => new LinenListViewModel(x));
            _dispatcher.RunInMainThread(() => LinensList = linensList.ToObservableCollection());

            LinensList.ForEach(x => x.Name = MasterLinens.FirstOrDefault(y=> x.MasterLinenId == y.Id)?.Name);
            IsChanged = false;
        }

        public AddLinenListViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentException(nameof(dispatcher));

            AddLinenListCommand = new RelayCommand(AddLinenList, () => SelectedMasterLinen != null);
            SetLinenListCommand = new RelayCommand(SetLinenList, () => SelectedLinenList != null);
            CloseCommand = new RelayCommand(Close);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedMasterLinen))
            {
                AddLinenListCommand.RaiseCanExecuteChanged();
                SetLinenListCommand.RaiseCanExecuteChanged();
            }
        }

        public async void AddLinenList()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to add {SelectedMasterLinen.Name} ? "))
                return;

            var linenList = LinensList.FirstOrDefault(x => x.MasterLinenId == SelectedMasterLinen.Id);

            if (linenList != null)
            {
                linenList.Active = true;
            }
            else
            {
                linenList = new LinenListViewModel()
                {
                    ClientId = SelectedDepartment.ClientId,
                    MasterLinenId = SelectedMasterLinen.Id,
                    Active = true,
                    Name = SelectedMasterLinen.Name,
                    DepartmentId = SelectedDepartment.Id,
                    UnitTypeId = 1,
                };
                LinensList.Add(linenList);
            }

            linenList.AcceptChanges();
            await _dataService.AddOrUpdateAsync(linenList.OriginalObject);

            RaisePropertyChanged(()=> SortedMasterLinen);
            RaisePropertyChanged(()=> SortedLinenList);
        }

        public void SetLinenList()
        {
            CloseAction?.Invoke(true);
        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to close window ? "))
            return;

            CloseAction?.Invoke(IsChanged);
        }

        public ObservableCollection<MasterLinenViewModel> SortMasterLinen()
        {
            var sortedLinenList = MasterLinens?.Where(x => SortedLinenList.All(y => y.MasterLinenId != x.Id)).ToObservableCollection();

            return sortedLinenList?.OrderBy(x=> x.Name).ToObservableCollection();
        }

        public ObservableCollection<LinenListViewModel> SortLinenList()
        {
            var linens = LinensList.Where(x => x.Active).ToObservableCollection();

            return linens;
        }

        public LinenListViewModel GetLinenList()
        {
            return SelectedLinenList;
        }

    }
}
