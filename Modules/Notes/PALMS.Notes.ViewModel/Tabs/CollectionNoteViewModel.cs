using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.Notes.ViewModel.Window;
using PALMS.Reports.Common;
using PALMS.Reports.Model.NoteReports;
using PALMS.Reports.Model.Notes;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    public class CollectionNoteViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        private readonly IDispatcher _dispatcher;

        #region Svoystva i peremennie

        private readonly IDialogService _dialogService;
        private readonly NoteCommonMethods _commonMethods;
        private readonly IReportService _reportService;
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private Client _selectedClient;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private NoteHeaderViewModel _selectedNoteHeader;
        private ObservableCollection<NoteTreeItemViewModel> _treeItems;
        private NoteTreeItemViewModel _selectedTreeItem;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private List<Client> _clients;
        private List<UnitViewModel> _deliveryTypes;
        private List<UnitViewModel> _serviceTypes;
        private List<LinenList> _linenLists;
        private PrimeInfo _primeInfo;
        private NoteRowViewModel _selectedNoteRow;
        public string Name => "Collection Note";
        public int NoteStatus => 2;

        public int MaxNoteId { get; set; }

        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public ObservableCollection<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public ObservableCollection<NoteTreeItemViewModel> TreeItems
        {
            get => _treeItems;
            set => Set(ref _treeItems, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public NoteTreeItemViewModel SelectedTreeItem
        {
            get => _selectedTreeItem;
            set => Set(ref _selectedTreeItem, value);
        }

        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }

        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }

        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }

        public ObservableCollection<NoteRowViewModel> SortedNoteRows => NoteRows?
            .Where(x => x.OriginalObject?.NoteHeader == SelectedTreeItem?.OriginalObject).OrderBy(x=> x.LinenName).ToObservableCollection();

        public ObservableCollection<NoteTreeItemViewModel> SortedTreeItems =>
            TreeItems?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public RelayCommand AddNoteCommand { get; }
        public RelayCommand RemoveNoteCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand AddDepartmentLinenCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand RemoveNoteRowCommand { get; }
        public Action Saved { get; set; }


        #endregion

        public CollectionNoteViewModel(IDataService dataService, IDialogService dialogService, IDispatcher dispatcher, NoteCommonMethods commonMethods, IReportService reportService, IResolver resolver)
        {

            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _commonMethods = commonMethods ?? throw new ArgumentNullException(nameof(commonMethods));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddNoteCommand = new RelayCommand(CreateNoteHeader, () => SelectedTreeItem?.OriginalObject is Department);
            RemoveNoteCommand = new RelayCommand(RemoveNote, () => SelectedTreeItem?.OriginalObject is NoteHeader);
            ClearCommand = new RelayCommand(ClearQty, () => SelectedTreeItem?.OriginalObject is NoteHeader);
            SaveCommand = new RelayCommand(Save, () => SelectedTreeItem?.OriginalObject is NoteHeader);
            AddDepartmentLinenCommand = new RelayCommand(AddDepartmentLinen, () => SelectedTreeItem?.OriginalObject is Department);
            AddLinenCommand = new RelayCommand(AddNewLinen, () => SelectedTreeItem?.OriginalObject is NoteHeader);
            RemoveNoteRowCommand = new RelayCommand(RemoveNoteRow, () => SelectedNoteRow != null);
            PrintCommand = new RelayCommand(Print);

            NoteHeaders = new ObservableCollection<NoteHeaderViewModel>();
            NoteRows = new ObservableCollection<NoteRowViewModel>();

            Refresh();

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            Refresh();

            _dispatcher.RunInMainThread(() =>
            {
                TreeItems = _commonMethods.DepartmentTreeItems;
                Clients = _commonMethods.Clients;
                LinenLists = _commonMethods.LinenLists;
                ServiceTypes = _commonMethods.WashTypes;
                DeliveryTypes = _commonMethods.DeliveryTypes;
                PrimeInfo = _commonMethods.PrimeInfo;
            });

            TreeItems.CollectionChanged += TreeItemsOnCollectionChanged;

            MaxNoteId = _commonMethods.GetMaxNoteHeaderId();
            await Task.CompletedTask;

        }

        public void Refresh()
        {
            SelectedNoteHeader = null;
            SelectedClient = new Client();
            SelectedTreeItem = null;

            RaisePropertyChanged(()=> SortedNoteRows);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedTreeItems);
            }

            if (e.PropertyName == nameof(SelectedTreeItem))
            {
                if(SelectedTreeItem == null)
                    return;

                SetSelectedNoteHeader();
                RaisePropertyChanged(() => SortedNoteRows);

                AddNoteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                RemoveNoteCommand.RaiseCanExecuteChanged();
                ClearCommand.RaiseCanExecuteChanged();
                PrintCommand.RaiseCanExecuteChanged();
                AddDepartmentLinenCommand.RaiseCanExecuteChanged();
                AddLinenCommand.RaiseCanExecuteChanged();
            }
            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                RemoveNoteRowCommand.RaiseCanExecuteChanged();
            }
        }

        private async void AddNewLinen()
        {
            if (SelectedNoteHeader == null)
                return;

            var chargeViewModel = _resolverService.Resolve<AddLinenViewModel>();
            var sortedLinen = LinenLists.Where(x => x.DepartmentId == SelectedNoteHeader.DepartmentId)
                .ToList();

            await chargeViewModel.InitializeAsync(SelectedNoteHeader, sortedLinen);
            var showDialog = _dialogService.ShowDialog(chargeViewModel);

            if (!showDialog) return;

            var newNoteRowsToAdd = chargeViewModel.GetNotRows();

            newNoteRowsToAdd.OriginalObject.NoteHeader = SelectedNoteHeader.OriginalObject;

            NoteRows.Add(newNoteRowsToAdd);

            RaisePropertyChanged(() => SortedNoteRows);
        }

        public void RemoveNoteRow()
        {
            if (SelectedNoteRow == null)
                return;

            if (!_dialogService.ShowQuestionDialog($"Remove the note row '{SelectedNoteRow.LinenName}' ?"))
                return;

            NoteRows.Remove(SelectedNoteRow);
            RaisePropertyChanged(() => SortedNoteRows);
        }

        public void CreateNoteHeader()
        {
            if (SelectedTreeItem?.OriginalObject is Department)
            {
                var id = TreeItems.Max(x => x.Id);

                if (TreeItems.Count == 0)
                {
                    id = 0;
                }

                var newNoteHeader = new NoteHeaderViewModel
                {
                    ClientId = SelectedClient.Id,
                    DepartmentId = SelectedTreeItem.Id,
                    DeliveryTypeId = 1,
                    CollectionDate = DateTime.Now,
                    ExpressCharge = SelectedClient.ClientInfo.Express,
                    WeightPrice = SelectedClient.ClientInfo.WeighPrice,
                };

                MaxNoteId++;

                TreeItems.Add(new NoteTreeItemViewModel(newNoteHeader.OriginalObject)
                {
                    Id = ++id,
                    Name = $"PAL {MaxNoteId:D5} /{DateTime.Now:yy}",
                    ParentId = SelectedTreeItem.Id,
                    ClientId = SelectedClient.Id,
                });

                NoteHeaders.Add(newNoteHeader);
                CreateNoteRow(newNoteHeader);
            }
        }

        public void CreateNoteRow(NoteHeaderViewModel note)
        {
            var linens = new ObservableCollection<NoteRowViewModel>();

            foreach (var linenList in LinenLists.Where(x => x.DepartmentId == note.DepartmentId))
            {
                var linen = new NoteRowViewModel
                {
                    LinenName = linenList.MasterLinen.Name,
                    LinenListId = linenList.Id,
                    PriceUnit = linenList.UnitId,
                    ServiceTypeId = 1,
                    OriginalObject = {NoteHeader = note.OriginalObject},
                };
                linens.Add(linen);
            }

            NoteRows.AddRange(linens);
        }

        public void RemoveNote()
        {
            if(!_dialogService.ShowQuestionDialog($"Remove the Receipt note '{SelectedTreeItem.Name}' ?"))
                return;

            var id = SelectedTreeItem.Id;

            NoteHeaders.Remove(SelectedNoteHeader);
            TreeItems.Remove(SelectedTreeItem);
            MaxNoteId--;

            foreach (var noteRow in SortedNoteRows)
            {
                NoteRows.Remove(noteRow);
            }

            SortNoteId(id);
            SelectedTreeItem = null;
        }

        private void SortNoteId(int id)
        {
            foreach (var treeItem in TreeItems.Where(x=> x.Id > id))
            {
                if (treeItem.ParentId > id)
                    treeItem.ParentId--;

                treeItem.Id--;
                var name = Convert.ToInt32(treeItem.Name.Substring(4, 5)) -1;
                treeItem.Name = $"PAL {name:D5} /{DateTime.Now:yy}";
            }
        }

        private void Save()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to save note {SelectedTreeItem.Name} ?"))
                return;

            var noteRows = NoteRows
                .Where(x => x.OriginalObject.NoteHeader == SelectedNoteHeader.OriginalObject && x.PrimeCollectedQty != 0)
                .ToObservableCollection();

            noteRows.ForEach(x => _commonMethods.AddNoteRowPriceWeight(x));

            foreach (var noteRow in noteRows)
            {
                noteRow.OriginalObject.NoteHeader = SelectedNoteHeader.OriginalObject;
            }

            _commonMethods.Save(SelectedNoteHeader, noteRows);


            foreach (var sortedRows in SortedNoteRows)
            {
                NoteRows.Remove(sortedRows);
            }

            NoteHeaders.Remove(SelectedNoteHeader);
            TreeItems.Remove(SelectedTreeItem);
            SelectedTreeItem = null;
        }

        private void ClearQty()
        {
            if (!_dialogService.ShowQuestionDialog(
                $"Clear collection quantity, Comment and weight of '{SelectedTreeItem.Name}' ?"))
            {
                return;
            }

            foreach (var noteRow in NoteRows.Where(x => x.OriginalObject.NoteHeader == SelectedTreeItem.OriginalObject))
            {
                noteRow.PrimeCollectedQty = 0;
                noteRow.ServiceTypeId = 1;
            }

            SelectedNoteHeader.Comment = null;
            SelectedNoteHeader.CollectionWeight = 0;
        }

        public void SetSelectedNoteHeader()
        {
            if (!(SelectedTreeItem?.OriginalObject is NoteHeader))
            {
                SelectedNoteHeader = null;
                return;
            }

            SelectedNoteHeader = NoteHeaders.FirstOrDefault(x => x.OriginalObject == SelectedTreeItem.OriginalObject);
        }

        private void TreeItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => SortedTreeItems);
        }

        public async void AddDepartmentLinen()
        {
            if (!(SelectedTreeItem.OriginalObject is Department))
                return;

            var department = _commonMethods.Departments.FirstOrDefault(x => x.OriginalObject == SelectedTreeItem.OriginalObject);

            if(department == null) return;

            var addDepartmentLinen = _resolverService.Resolve<AddLinenListViewModel>();
            await addDepartmentLinen.InitializeAsync(department);

            var showDialog = _dialogService.ShowDialog(addDepartmentLinen);

            if (!showDialog) return;

            var newLinen = addDepartmentLinen.GetLinenList();

            ////TODO: Доработать в случае испольхования Collection Note сново
            
            RaisePropertyChanged(()=> SortedNoteRows);
        }

        private void Print()
        {

        }

    }
}

