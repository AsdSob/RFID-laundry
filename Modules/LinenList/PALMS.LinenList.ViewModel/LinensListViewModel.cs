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
using PALMS.Data.Objects.LinenModel;
using PALMS.LinenList.ViewModel.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.LinenList.ViewModel
{
    public class LinensListViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IResolver _resolverService;
        private readonly IDialogService _dialogService;
        private List<Client> _clients;
        private ObservableCollection<MasterLinensViewModel> _masterLinens;
        private ObservableCollection<LinenListModelViewModel> _fullLinenList;
        private List<Department> _departments;
        private List<FamilyLinen> _familyLinens;
        private List<LinenType> _typeLinens;
        private Client _selectedClient;
        private MasterLinensViewModel _selectedMasterLinen;
        private LinenListModelViewModel _selectedLinenListModel;
        private Department _selectedDepartment;

        private readonly SemaphoreSlim _saveLockSemaphore = new SemaphoreSlim(1, 1);

        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public ObservableCollection<MasterLinensViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public ObservableCollection<LinenListModelViewModel> FullLinenList
        {
            get => _fullLinenList;
            set => Set(ref _fullLinenList, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<LinenType> TypeLinens
        {
            get => _typeLinens;
            set => Set(ref _typeLinens, value);
        }
        public List<FamilyLinen> FamilyLinens
        {
            get => _familyLinens;
            set => Set(ref _familyLinens, value);
        }

        public List<UnitViewModel> LinenUnits { get; set; }

        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public MasterLinensViewModel SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(ref _selectedMasterLinen, value);
        }
        public LinenListModelViewModel SelectedLinenListModel
        {
            get => _selectedLinenListModel;
            set => Set(ref _selectedLinenListModel, value);
        }
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }

        public ObservableCollection<LinenListModelViewModel> SortedLinenList => SortLinenList();
        public ObservableCollection<MasterLinensViewModel> SortedMasterLinen => SortMasterLinen();
        public ObservableCollection<Department> SortedDepartments => SortDepartments();

        public RelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand DeActivateCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand LeasingLinenEdit { get; }
        public RelayCommand RemoveDepartmentLinensCommand { get; }
        public RelayCommand CopyParentLinenCommand { get; }
        public RelayCommand CopyParentLinenPriceCommand { get; }
        public RelayCommand ChangeNoteLinenCommand { get; }
        public RelayCommand CopyLaundryPriceCommand { get; }
        public RelayCommand UnUsedLinenCommand { get; }
        public RelayCommand ChangeLinenWeightCommand { get; }

        public LinensListViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService, IResolver resolver)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddCommand = new RelayCommand(Add, CanAdd);
            RemoveCommand = new RelayCommand(Remove, CanRemove);
            DeActivateCommand = new RelayCommand(DeActivateLinen, CanRemove);
            CopyCommand = new RelayCommand(Copy,()=> SelectedDepartment != null);
            LeasingLinenEdit = new RelayCommand(LeasingLinenWindow, ()=> SelectedDepartment != null);
            ChangeNoteLinenCommand = new RelayCommand(ChangeNoteLinenWindow, ()=> SelectedClient != null);
            CopyParentLinenCommand = new RelayCommand(CopyParentLinen, ()=> SelectedDepartment != null && !SelectedDepartment.ParentId.HasValue);
            RemoveDepartmentLinensCommand = new RelayCommand(RemoveDepartmentLinens, ()=> SelectedDepartment !=null);
            CopyParentLinenPriceCommand = new RelayCommand(CopyPrice, () => SelectedDepartment != null && !SelectedDepartment.ParentId.HasValue);
            CopyLaundryPriceCommand = new RelayCommand(CopyLaundryPrice, () => SelectedDepartment != null);
            UnUsedLinenCommand = new RelayCommand(UnUsedLinen);
            ChangeLinenWeightCommand = new RelayCommand(RunCode);

            LinenUnits = EnumExtentions.GetValues<LinenUnitEnum>();

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            var client = await _dataService.GetAsync<Client>();
            _dispatcher.RunInMainThread(() => Clients = client.ToList());

            var masterLinen = await _dataService.GetAsync<MasterLinen>();
            var masterLinens = masterLinen.Select(x => new MasterLinensViewModel(x));
            _dispatcher.RunInMainThread(() => MasterLinens = masterLinens.ToObservableCollection());

            var department = await _dataService.GetAsync<Department>();
            _dispatcher.RunInMainThread(() => Departments = department.ToList());

            var linenList = await _dataService.GetAsync<Data.Objects.LinenModel.LinenList>();
            var linenLists = linenList.Select(x => new LinenListModelViewModel(x));
            _dispatcher.RunInMainThread(() => FullLinenList = linenLists.ToObservableCollection());

            FullLinenList.ForEach(SubscribeItem);
            FullLinenList.CollectionChanged += FullLinenListOnCollectionChanged;

            var familiLinen = await _dataService.GetAsync<FamilyLinen>();
            _dispatcher.RunInMainThread(() => FamilyLinens = familiLinen.ToList());

            var typeLinen = await _dataService.GetAsync<LinenType>();
            _dispatcher.RunInMainThread(() => TypeLinens = typeLinen.ToList());
        }

        private bool CanAdd()
        {
            return SelectedDepartment != null;
        }

        private bool CanRemove()
        {
            return SelectedLinenListModel != null;
        }

        private void UnSubscribeItem(LinenListModelViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(LinenListModelViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!(sender is LinenListModelViewModel item)) return;

            if(e.PropertyName == nameof(item.Laundry))
            {
                EqualPriceChange(item);
            }

            Save(item);
        }

        private ObservableCollection<LinenListModelViewModel> SortLinenList()
        {
            if (SelectedDepartment == null) return null;

            var linenList = FullLinenList?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

            return linenList;
        }

        private ObservableCollection<MasterLinensViewModel> SortMasterLinen()
        {
            if (SortedLinenList == null || SortedLinenList.Count == 0) return MasterLinens;

            var masterLinen = MasterLinens?.Where(x => SortedLinenList.All(y => y.MasterLinenId != x.Id)).ToObservableCollection();

            return masterLinen;
        }

        private ObservableCollection<Department> SortDepartments()
        {
            if (SelectedClient == null) return null;

            return  Departments.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();
        }

        public void EqualPriceChange(LinenListModelViewModel item)
        {
            if(SelectedDepartment.DepartmentType == (int)DepartmentTypeEnum.GuestLaundry) return;
            item.DryCleaning = item.Laundry;
            item.Pressing = item.Laundry;
        }

        private async void Save(LinenListModelViewModel item)
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedLinenList);
                RaisePropertyChanged(() => SortedMasterLinen);
                LeasingLinenEdit.RaiseCanExecuteChanged();
                CopyCommand.RaiseCanExecuteChanged();
                RemoveDepartmentLinensCommand.RaiseCanExecuteChanged();
                CopyParentLinenCommand.RaiseCanExecuteChanged();
                CopyParentLinenPriceCommand.RaiseCanExecuteChanged();
                CopyLaundryPriceCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
                ChangeNoteLinenCommand.RaiseCanExecuteChanged();
            }
        }

        public void CopyLaundryPrice()
        {
            if(SortedLinenList == null || SortedLinenList.Count == 0)
                return;

            if (!_dialogService.ShowQuestionDialog("Do you want to copy Laundry price to Pressing and Dry Cleaning"))
                return;

            foreach (var linenList in SortedLinenList)
            {
                linenList.Pressing = linenList.Laundry;
                linenList.DryCleaning = linenList.Laundry;
            }
        }

        private void FullLinenListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<LinenListModelViewModel>())
                {
                    SubscribeItem(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<LinenListModelViewModel>())
                {
                    UnSubscribeItem(item);
                }
            }
        }

        private void Add()
        {
            if (SortedMasterLinen.Count != 0 && (SelectedMasterLinen != null))
            {
                var fullLinenList = new LinenListModelViewModel
                {
                    ClientId = SelectedClient.Id,
                    MasterLinenId = SelectedMasterLinen.Id,
                    Active = true,
                    DepartmentId = SelectedDepartment.Id,
                    UnitTypeId = 1,
                };

                if (SelectedMasterLinen?.Weight == null)
                {
                    fullLinenList.Weight = 0;
                }
                else
                {
                    fullLinenList.Weight = (int) SelectedMasterLinen?.Weight;
                }

                FullLinenList.Add(fullLinenList);
                RaisePropertyChanged(() => SortedLinenList);
                RaisePropertyChanged(() => SortedMasterLinen);
                Save(fullLinenList);
            }
        }

        private void DeActivateLinen()
        {
            if (!_dialogService.ShowQuestionDialog($"De-Activate the client '{MasterLinens.FirstOrDefault(x => x.Id == SelectedLinenListModel.MasterLinenId)?.Name}' ?"))
                return;

                SelectedLinenListModel.Active = false;
        }

        private async void Remove()
        {
            if (!_dialogService.ShowQuestionDialog($"Remove the client '{MasterLinens.FirstOrDefault(x => x.Id == SelectedLinenListModel.MasterLinenId)?.Name}' ?"))
                return;

            _dialogService.ShowBusy();

            try
            {
                if (!SelectedLinenListModel.IsNew)
                    await _dataService.DeleteAsync(SelectedLinenListModel.OriginalObject);

                FullLinenList.Remove(SelectedLinenListModel);
            }
            finally
            {
                _dialogService.HideBusy();
            }

            RaisePropertyChanged(() => SortedLinenList);
            RaisePropertyChanged(() => SortedMasterLinen);
        }

        private async void RemoveDepartmentLinens()
        {
            if (!_dialogService.ShowQuestionDialog($"Remove the '{SelectedDepartment.Name}' ALL linens ?"))
                return;

            _dialogService.ShowBusy();

                try
                {
                    foreach (var linen in SortedLinenList)
                    {
                        if (!linen.IsNew)
                            await _dataService.DeleteAsync(linen.OriginalObject);

                        FullLinenList.Remove(linen);
                    }
                }
                finally { }
                _dialogService.HideBusy();

            RaisePropertyChanged(() => SortedLinenList);
            RaisePropertyChanged(() => SortedMasterLinen);
        }

        private async void LeasingLinenWindow()
        {
            if (SelectedClient == null)
                return;

            var chargeViewModel = _resolverService.Resolve<LeasingLinenViewModel>();
            await chargeViewModel.InitializeAsync(SortedLinenList, MasterLinens);
            _dialogService.ShowDialog(chargeViewModel);
        }

        private async void ChangeNoteLinenWindow()
        {
            if(SelectedClient == null) return;

            var chargeViewModel = _resolverService.Resolve<ChangeNoteLinenViewModel>();
            await chargeViewModel.InitializeAsync(SelectedClient);
            _dialogService.ShowDialog(chargeViewModel);
        }

        private async void Copy()
        {
            if (SelectedDepartment == null)
                return;

            var copyDepartmentSource = _resolverService.Resolve<SelectCopySourceViewModel>();

            await copyDepartmentSource.InitializeAsync(Clients, Departments, FullLinenList, MasterLinens);
            var showDialog = _dialogService.ShowDialog(copyDepartmentSource);

            if (showDialog)
            {
                var masterLinenIds = copyDepartmentSource.GetMasterLinens();

                if (masterLinenIds == null) return;

                foreach (var masterLinenId in masterLinenIds)
                {
                    if (SortedLinenList?.FirstOrDefault(x => x.MasterLinenId == masterLinenId) != null)
                        continue;

                    var fullLinenList = new LinenListModelViewModel
                    {
                        ClientId = SelectedClient.Id,
                        MasterLinenId = masterLinenId,
                        Active = true,
                        DepartmentId = SelectedDepartment.Id,
                        UnitTypeId = 1,
                    };

                    if (MasterLinens.FirstOrDefault(x => x.Id == masterLinenId)?.Weight == null)
                    {
                        fullLinenList.Weight = 0;
                    }
                    else
                    {
                        fullLinenList.Weight = (int) MasterLinens.FirstOrDefault(x => x.Id == masterLinenId).Weight;
                    }

                    Save(fullLinenList);
                    FullLinenList.Add(fullLinenList);
                }

                RaisePropertyChanged(() => SortedLinenList);
                RaisePropertyChanged(() => SortedMasterLinen);
            }
        }

        public void CopyParentLinen()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to COPY {SelectedDepartment.Name} linen to sub departments?"))
                return;
            var subDepartments = Departments.Where(x => x.ParentId == SelectedDepartment.Id).ToList();
            var linenList = FullLinenList.Where(x => x.DepartmentId == SelectedDepartment.Id).ToList();

            foreach (var subDep in subDepartments)
            {
                var sortedLinenList = new List<LinenListModelViewModel>();
                var newLinenList = new List<LinenListModelViewModel>();
                var existLinenList = FullLinenList.Where(x => x.DepartmentId == subDep.Id).ToList();

                if (!existLinenList.Any())
                {
                    sortedLinenList.AddRange(linenList);
                }
                else
                {
                    foreach (var linen in linenList)
                    {
                        var existLinen = existLinenList.FirstOrDefault(x => x.MasterLinenId == linen.MasterLinenId);
                        if (existLinen != null) continue;
                        sortedLinenList.Add(linen);
                    }
                }

                foreach (var linen in sortedLinenList)
                {
                    newLinenList.Add(new LinenListModelViewModel()
                    {
                        Active = linen.Active,
                        ClientId = linen.ClientId,
                        DepartmentId = subDep.Id,
                        DryCleaning = linen.DryCleaning,
                        Laundry = linen.Laundry,
                        MasterLinenId = linen.MasterLinenId,
                        Pressing = linen.Pressing,
                        UnitTypeId = linen.UnitTypeId,
                        Weight = linen.Weight,
                    });
                }
                newLinenList.ForEach(Save);
                FullLinenList.AddRange(newLinenList);
            }
            RaisePropertyChanged(() => SortedLinenList);
            RaisePropertyChanged(() => SortedMasterLinen);
        }

        public void CopyPrice()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to COPY {SelectedDepartment.Name} linen PRICE to sub departments?"))
                return;

            var subDepartments = Departments.Where(x => x.ParentId == SelectedDepartment.Id).ToList();
            var linenList = FullLinenList.Where(x => x.DepartmentId == SelectedDepartment.Id).ToList();

            foreach (var subDep in subDepartments)
            {
                foreach (var linen in linenList)
                {
                    foreach (var subLinen in FullLinenList.Where(x =>
                        x.DepartmentId == subDep.Id && x.MasterLinenId == linen.MasterLinenId))
                    {
                        subLinen.Pressing = linen.Pressing;
                        subLinen.Laundry = linen.Laundry;
                        subLinen.DryCleaning = linen.DryCleaning;

                        Save(subLinen);
                    }
                }
            }

        }

        private async void UnUsedLinen()
        {
            var unUsedLinen = _resolverService.Resolve<UnUsedLinenViewModel>();

            await unUsedLinen.InitializeAsync(SelectedClient);
            _dialogService.ShowDialog(unUsedLinen);

            await InitializeAsync();
            RaisePropertyChanged(() => SortedLinenList);
            RaisePropertyChanged(() => SortedMasterLinen);
        }

        public void RunCode()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to copy Laundry price to Pressing and Dry Cleaning"))
                return;

            foreach (var department in Departments.Where(x => x.DepartmentType != (int)DepartmentTypeEnum.GuestLaundry))
            {
                var linens = FullLinenList.Where(x => x.DepartmentId == department.Id);
                foreach (var linen in linens)
                {
                    if (Equals(linen.Pressing, linen.Laundry) && Equals(linen.Laundry, linen.DryCleaning))
                        continue;

                    linen.Pressing = linen.Laundry;
                    linen.DryCleaning = linen.Laundry;
                }
            }
        }
    }
}
