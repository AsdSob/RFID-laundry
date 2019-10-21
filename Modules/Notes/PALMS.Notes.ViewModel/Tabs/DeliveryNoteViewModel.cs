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
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;


namespace PALMS.Notes.ViewModel
{
    public class DeliveryNoteViewModel : ViewModelBase, IInitializationAsync
    {
        #region properties

        private Client _selectedClient;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private readonly NoteCommonMethods _commonMethods;
        private readonly IDispatcher _dispatcher;
        private readonly IReportService _reportService;
        private List<Client> _clients;
        private List<UnitViewModel> _deliveryTypes;
        private List<UnitViewModel> _serviceTypes;
        private List<LinenList> _linenLists;
        private PrimeInfo _primeInfo;
        private NoteHeaderViewModel _selectedNoteHeader;
        private List<DepartmentViewModel> _departments;
        private List<NoteHeaderViewModel> _selectedNoteHeaders;
        private NoteRowViewModel _selectedNoteRow;
        private bool _showPreInvoiceNotes;
        private List<UnitViewModel> _noteStatuses;
        private bool _showClientReceivedQty;
        private bool _showClientReceivedNotes;

        public bool ShowClientReceivedQty
        {
            get => _showClientReceivedQty;
            set => Set(ref _showClientReceivedQty, value);
        }
        public List<UnitViewModel> NoteStatuses
        {
            get => _noteStatuses;
            set => Set(ref _noteStatuses, value);
        }

        public bool ShowClientReceivedNotes
        {
            get => _showClientReceivedNotes;
            set => Set(ref _showClientReceivedNotes, value);
        }
        public bool ShowPreInvoiceNotes
        {
            get => _showPreInvoiceNotes;
            set => Set(ref _showPreInvoiceNotes, value);
        }
        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public List<NoteHeaderViewModel> SelectedNoteHeaders
        {
            get => _selectedNoteHeaders;
            set => Set(ref _selectedNoteHeaders, value);
        }
        public List<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
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

        public DateTime PrintTime { get; set; }

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => SortNoteHeaders();

        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRows();

        public RelayCommand ClearCommand { get; }
        public RelayCommand<object> PrintNoteCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand RemoveNoteCommand { get; }
        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand CopyLinenCommand { get; }
        public RelayCommand RemoveNoteRowCommand { get; }
        public RelayCommand AddNoteCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand DuplicateNoteRowCommand { get; }
        public RelayCommand DuplicateNotCommand { get; }
        public RelayCommand ShowQtyDifferenceCommand { get; }
        public RelayCommand ShowBalanceCommand { get; }
        public RelayCommand ChangeDepartmentCommand { get; }
        public RelayCommand GenerateCommand { get; }
        public RelayCommand DeleteNoteCommand { get; }
        public RelayCommand SaveNoteCommand { get; }


        #endregion

        public DeliveryNoteViewModel(IDialogService dialogService, IResolver resolverService,
            NoteCommonMethods commonMethods, IDispatcher dispatcher, IReportService reportService, IDataService dataService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolverService ?? throw new ArgumentNullException(nameof(resolverService));
            _commonMethods = commonMethods ?? throw new ArgumentNullException(nameof(commonMethods));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

            SaveChangesCommand = new RelayCommand(SaveChangesByCommand, () => SelectedNoteHeader != null);
            SaveNoteCommand = new RelayCommand(SaveChangesByKey, () => SelectedNoteHeader != null);
            AddNoteCommand = new RelayCommand(AddNewNote);
            RemoveNoteCommand = new RelayCommand(RemoveNote, () => SelectedNoteHeader != null);
            DeleteNoteCommand = new RelayCommand(DeleteNote, () => SelectedNoteHeader != null);
            PrintNoteCommand = new RelayCommand<object>(Print);
            AddLinenCommand = new RelayCommand(AddNewLinen, () => SelectedNoteHeader != null);
            RemoveNoteRowCommand = new RelayCommand(RemoveNoteRow, () => SelectedNoteRow != null);
            CopyLinenCommand = new RelayCommand(CopyLinen, () => SelectedNoteHeader != null);
            ClearCommand = new RelayCommand(ClearQty, () => SelectedNoteHeader != null);
            ChangeDepartmentCommand = new RelayCommand(ChangeDepartment, () => SelectedNoteHeader != null);
            GenerateCommand = new RelayCommand(GenerateToPreInvoice, () => SelectedNoteHeader != null);

            DuplicateNoteRowCommand = new RelayCommand(DuplicateNoteRow, () => SelectedNoteRow != null);
            DuplicateNotCommand = new RelayCommand(DuplicateNote, () => SelectedNoteHeader != null);
            ShowQtyDifferenceCommand = new RelayCommand(ShowQtyDifference, () => SelectedNoteHeader != null);
            ShowBalanceCommand = new RelayCommand(ShowBalance, () => SelectedNoteHeader != null);

            RefreshCommand = new RelayCommand(Refresh);

            ShowPreInvoiceNotes = false;
            ShowClientReceivedNotes = false;
            ShowClientReceivedQty = false;
            PrintTime = DateTime.Now;
            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            await GetBaseViewModels();
        }

        public async Task GetBaseViewModels()
        {
            _dialogService.ShowBusy();

            try
            {
                var prime = await _dataService.GetAsync<PrimeInfo>();
                var primeInfo = prime.FirstOrDefault();
                _dispatcher.RunInMainThread(() => PrimeInfo = primeInfo);

                var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
                var clients = client.Where(x => x.Active);
                _dispatcher.RunInMainThread(() => Clients = clients.OrderBy(x => x.ShortName).ToList());

                DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
                ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
                NoteStatuses = EnumExtentions.GetValues<NoteStatusEnum>();

                await GetLinenDepartment();
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

        public async Task GetLinenDepartment()
        {
            var linenList = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
            var linenLists = linenList.Where(x => x.Active);
            _dispatcher.RunInMainThread(() => LinenLists = linenLists.ToList());

            var department = await _dataService.GetAsync<Department>();
            var departments = department.Select(x => new DepartmentViewModel(x));
            _dispatcher.RunInMainThread(() => Departments = departments.ToList());
        }

        public async Task GetNoteHeaders()
        {
            if (SelectedClient == null) return;
            _dialogService.ShowBusy();

            try
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
                SelectedNoteHeader = null;
                NoteRows = new ObservableCollection<NoteRowViewModel>();

                var noteHeader =await _dataService.GetAsync<NoteHeader>(x => x.ClientId == SelectedClient.Id && x.NoteStatus != (int)NoteStatusEnum.Invoiced);
                var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());

                var noteRows = new List<NoteRowViewModel>();
                foreach (var note in NoteHeaders)
                {
                    note.SetName();
                    var noteRow = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    noteRows.AddRange(noteRow.Select(x => new NoteRowViewModel(x)));
                }
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());


                foreach (var noteRow in NoteRows)
                {
                    SetNoteRowColorStatus(noteRow);
                    SubscribeItem(noteRow);
                    noteRow.LinenName = LinenLists.FirstOrDefault(y => y.Id == noteRow.LinenListId)?.MasterLinen.Name;
                }

                NoteHeaders.ForEach(SetNoteColorStatus);
                NoteRows.CollectionChanged += NoteRowsCollectionChanged;

                RaisePropertyChanged(() => SortedNoteHeaders);
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

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                await GetNoteHeaders();
            }

            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                ShowQtyDifferenceCommand.RaiseCanExecuteChanged();
                ShowBalanceCommand.RaiseCanExecuteChanged();
                AddLinenCommand.RaiseCanExecuteChanged();
                RemoveNoteCommand.RaiseCanExecuteChanged();
                SaveChangesCommand.RaiseCanExecuteChanged();
                CopyLinenCommand.RaiseCanExecuteChanged();
                ClearCommand.RaiseCanExecuteChanged();
                ChangeDepartmentCommand.RaiseCanExecuteChanged();
                DuplicateNotCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => SortedNoteRows);
                GetCollectionSrNo();
                GenerateCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                RemoveNoteRowCommand.RaiseCanExecuteChanged();
                DuplicateNoteRowCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(ShowPreInvoiceNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(ShowClientReceivedNotes))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
        }

        private void UnSubscribeItem(NoteRowViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(NoteRowViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is NoteRowViewModel item)) return;
            if (SelectedNoteRow == null) return;

            if (item.PrimeCollectedQty + item.PrimeDeliveredQty + item.ClientReceivedQty > 0)
                SetNoteRowColorStatus(SelectedNoteRow);
            else
            {
                SelectedNoteRow.ColorStatus = 0;
            }

            switch (e.PropertyName)
            {
                // Set Note Color
                case nameof(SelectedNoteRow.PrimeDeliveredQty):
                    {
                        SelectedNoteRow.ClientReceivedQty = SelectedNoteRow.PrimeDeliveredQty;

                        if (SelectedNoteRow.PrimeDeliveredQty > 0)
                        {
                            SetNoteColorStatus(SelectedNoteHeader);
                        }
                        break;
                    }
                // Set Note Color
                case nameof(SelectedNoteRow.PrimeCollectedQty):
                    {
                        if (SelectedNoteRow.PrimeCollectedQty > 0)
                        {
                            SetNoteColorStatus(SelectedNoteHeader);
                        }
                        break;
                    }
                // Set Note Color
                case nameof(SelectedNoteRow.ClientReceivedQty):
                    {
                        if (SelectedNoteRow.ClientReceivedQty > 0)
                        {
                            SetNoteColorStatus(SelectedNoteHeader);
                        }
                        break;
                    }
                // Set Row Color and Price
                case nameof(SelectedNoteRow.ServiceTypeId):
                    {
                        if (item.OriginalObject.ServiceTypeId != item.ServiceTypeId)
                        {
                            var row = AddNoteRowPriceWeight(item);
                            item.Price = row.Price;
                        }

                        if (item.PrimeCollectedQty + item.PrimeDeliveredQty + item.ClientReceivedQty > 0)
                            SetNoteRowColorStatus(SelectedNoteRow);
                        break;
                    }
            }
        }

        public void SetNoteColorStatus(NoteHeaderViewModel note)
        {
            var noteHeader = note;
            double collectionQty = 0;
            double deliveryQty = 0;

            foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id))
            {
                collectionQty += noteRow.PrimeCollectedQty;
                deliveryQty += noteRow.PrimeDeliveredQty;
            }

            if (Math.Abs(collectionQty) <= 0 && Math.Abs(deliveryQty) <= 0)
            {
                noteHeader.ColorStatus = 1;
            }

            if (Math.Abs(collectionQty) > 0 && Math.Abs(deliveryQty) <= 0)
            {
                noteHeader.ColorStatus = 2;
            }

            if (Math.Abs(deliveryQty) > 0)
            {
                noteHeader.ColorStatus = 3;
            }

            if (note.NoteStatus == (int)NoteStatusEnum.PreInvoice)
            {
                noteHeader.ColorStatus = 4;
            }
        }

        public void SetNoteRowColorStatus(NoteRowViewModel row)
        {
            var noteRow = row;

            switch (noteRow.ServiceTypeId)
            {
                case (int)ServiceTypeEnum.Laundry:
                    noteRow.ColorStatus = 1;
                    break;

                case (int)ServiceTypeEnum.Pressing:
                    noteRow.ColorStatus = 2;
                    break;

                case (int)ServiceTypeEnum.DryCleaning:
                    noteRow.ColorStatus = 3;
                    break;
            }
        }

        public void SetCollectionSrNo(NoteHeaderViewModel note)
        {
            var collectionSrNo = GetBetween(note.Comment);

            if (string.IsNullOrEmpty(collectionSrNo))
            {
                if (string.IsNullOrEmpty(note.CollectionSrNo)) return;
                note.Comment = $"Bill.No. {note.CollectionSrNo}.             {note.Comment}";
            }
            else
            {
                if (note.CollectionSrNo == collectionSrNo) return;
                if (string.IsNullOrEmpty(note.CollectionSrNo)) return;

                note.Comment = note.Comment.Replace(collectionSrNo, note.CollectionSrNo);
            }
        }

        public void GetCollectionSrNo()
        {
            if (SelectedNoteHeader == null) return;

            var collectionSrNo = GetBetween(SelectedNoteHeader?.Comment);

            SelectedNoteHeader.CollectionSrNo = collectionSrNo;
        }

        public static string GetBetween(string strSource)
        {
            if (strSource == null) return null;

            const string strStart = "Bill.No. ";
            const string strEnd = ".";

            if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return null;

            var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
            var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);

            return strSource.Substring(start, end - start);
        }

        public NoteRowViewModel AddNoteRowPriceWeight(NoteRowViewModel row)
        {
            var noteRow = row;

            var linen = LinenLists?.FirstOrDefault(x => x.Id == noteRow.LinenListId);
            if (linen != null)
            {
                noteRow.Weight = linen.Weight;
                noteRow.PriceUnit = linen.UnitId;

                switch (noteRow.ServiceTypeId)
                {
                    case (int)ServiceTypeEnum.Laundry:
                        noteRow.Price = linen.Laundry;
                        break;

                    case (int)ServiceTypeEnum.DryCleaning:
                        noteRow.Price = linen.DryCleaning;
                        break;

                    case (int)ServiceTypeEnum.Pressing:
                        noteRow.Price = linen.Pressing;
                        break;
                }
            }
            return noteRow;
        }

        public ObservableCollection<NoteHeaderViewModel> SortNoteHeaders()
        {
            var noteHeaders = NoteHeaders
                ?.Where(x => x.ClientId == SelectedClient?.Id && x.NoteStatus == (int)NoteStatusEnum.DeliveryNote)
                .OrderByDescending(x => x.Id).ToObservableCollection();

            if (ShowClientReceivedNotes)
            {
                var notes = NoteHeaders
                    ?.Where(x => x.ClientId == SelectedClient?.Id && x.NoteStatus == (int)NoteStatusEnum.ClientNote)
                    .OrderByDescending(x => x.OriginalObject.Id);

                noteHeaders.AddRange(notes);
            }

            if (ShowPreInvoiceNotes)
            {
                var notes = NoteHeaders
                    ?.Where(x => x.ClientId == SelectedClient?.Id && x.NoteStatus == (int)NoteStatusEnum.PreInvoice)
                    .OrderByDescending(x => x.OriginalObject.Id);

                noteHeaders.AddRange(notes);
            }
            return noteHeaders;
        }

        public ObservableCollection<NoteRowViewModel> SortNoteRows()
        {
            return NoteRows?.Where(x => x.NoteHeaderId == SelectedNoteHeader?.Id).OrderBy(x => x.LinenName).ToObservableCollection();
        }

        private void NoteRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<NoteRowViewModel>())
                {
                    SubscribeItem(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<NoteRowViewModel>())
                {
                    UnSubscribeItem(item);
                }
            }
        }

        public void ClearQty()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to remove changes in Note {SelectedNoteHeader.Name} ?"))
                return;

            NoteRows.Where(y => y.NoteHeaderId == SelectedNoteHeader.Id).ForEach(x => x.Reset());

            SelectedNoteHeader.Reset();
        }

        private async void Refresh()
        {
            NoteHeaders?.Clear();
            NoteRows?.Clear();

            await GetLinenDepartment();
            await GetNoteHeaders();
        }

        private void ShowQtyDifference()
        {
            if (SelectedNoteHeader == null) return;
            if (!_dialogService.ShowQuestionDialog($" Show Collection and Delivery Difference of note {SelectedNoteHeader.Name} ?"))
                return;

            var noteRows = SortNoteRows();

            foreach (var noteRow in noteRows)
            {
                if ( Equals(noteRow.PrimeCollectedQty + noteRow.PrimeDeliveredQty, 0.0) || Equals(noteRow.PrimeCollectedQty , noteRow.PrimeDeliveredQty)) continue;

                string remark = null;

                if (noteRow.PrimeCollectedQty > noteRow.PrimeDeliveredQty)
                    remark = "Less " + (noteRow.PrimeCollectedQty - noteRow.PrimeDeliveredQty);

                if (noteRow.PrimeCollectedQty < noteRow.PrimeDeliveredQty)
                    remark = "Extra " + (noteRow.PrimeDeliveredQty - noteRow.PrimeCollectedQty);

                if (noteRow.Comment != null)
                {
                    noteRow.Comment += ", " + remark;
                }
                else
                {
                    noteRow.Comment = remark;
                }
            }
        }

        private void ShowBalance()
        {
            if (SelectedNoteHeader == null) return;
            if (!_dialogService.ShowQuestionDialog($" Show Collection and Delivery Difference of note {SelectedNoteHeader.Name} ?"))
                return;

            var noteRows = SortNoteRows();

            foreach (var noteRow in noteRows)
            {
                if (Equals(noteRow.PrimeCollectedQty + noteRow.PrimeDeliveredQty, 0.0) || Equals(noteRow.PrimeCollectedQty, noteRow.PrimeDeliveredQty)) continue;

                string remark = null;

                if (noteRow.PrimeCollectedQty > noteRow.PrimeDeliveredQty)
                    remark = "Balance " + (noteRow.PrimeCollectedQty - noteRow.PrimeDeliveredQty);

                if (noteRow.PrimeCollectedQty < noteRow.PrimeDeliveredQty)
                    remark = "Previous Balance " + (noteRow.PrimeDeliveredQty - noteRow.PrimeCollectedQty);

                if (noteRow.Comment != null)
                {
                    noteRow.Comment += ", " + remark;
                }
                else
                {
                    noteRow.Comment = remark;
                }
            }
        }

        private void SaveChanges(NoteHeaderViewModel noteHeader)
        {
            if(noteHeader == null) return;

            if (noteHeader.OriginalObject.NoteStatus >= (int)NoteStatusEnum.PreInvoice)
            {
                _dialogService.ShowInfoDialog($"{noteHeader.Name} Note is having Pre-invoice status and changes cannot be saved ");
                return;
            }

            Save(noteHeader);
        }

        private void SaveChangesByKey()
        {
            if(SelectedNoteHeader== null) return;
            Save(SelectedNoteHeader);

            _dialogService.ShowInfoDialog($"{SelectedNoteHeader.Name}  Note changes Saved");
        }

        private void SaveChangesByCommand()
        {
            if (SelectedNoteHeader == null) return;
            SaveChanges(SelectedNoteHeader);

            if (SelectedNoteHeader.OriginalObject.NoteStatus != (int)NoteStatusEnum.PreInvoice)
            {
                _dialogService.ShowInfoDialog($"{SelectedNoteHeader.Name}  Note changes Saved");
            }
        }

        public void Save(NoteHeaderViewModel noteHeader)
        {
            var noteRows = new ObservableCollection<NoteRowViewModel>();
            SetCollectionSrNo(noteHeader);
            foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id))
            {
                if (Equals(noteRow.PrimeCollectedQty + noteRow.PrimeDeliveredQty + noteRow.ClientReceivedQty, 0.0)) continue;
                noteRows.Add(noteRow);
            }

            SaveNoteHeader(noteHeader);
            SaveNoteRows(noteRows);
        }

        private async void SaveNoteHeader(NoteHeaderViewModel noteHeader)
        {
            if(noteHeader == null) return;
            if (!noteHeader.HasChanges()) return;

            noteHeader.AcceptChanges();
            await _dataService.AddOrUpdateAsync(noteHeader.OriginalObject);
        }

        private async void SaveNoteRows(ObservableCollection<NoteRowViewModel> noteRows)
        {
            if (noteRows == null || noteRows.Count < 1 || !noteRows.Any(x => x.HasChanges())) return;

            noteRows.ForEach(x => x.AcceptChanges());
            foreach (var noteRow in noteRows)
            {
                await _dataService.AddOrUpdateAsync(noteRow.OriginalObject);
            }
        }

        private void GenerateToPreInvoice()
        {
            var note = SelectedNoteHeader;

            if (note == null) return;
            if (note.NoteStatus != (int)NoteStatusEnum.ClientNote) return;

            if (!_dialogService.ShowQuestionDialog($" Do you want to move {SelectedNoteHeader.Name} to Pre-Invoice?"))
                return;
            note.NoteStatus = (int)NoteStatusEnum.PreInvoice;

            SaveChanges(note);
            RaisePropertyChanged(() => SortedNoteHeaders);
            RaisePropertyChanged(() => SortedNoteRows);
        }

        private void Generate(NoteHeaderViewModel noteHeader)
        {
            var note = noteHeader;

            if (note.NoteStatus == (int)NoteStatusEnum.DeliveryNote)
            {
                note.NoteStatus = (int)NoteStatusEnum.ClientNote;
                note.ColorStatus = 4;
                note.ClientReceivedDate = DateTime.Now;

                SaveChanges(note);
                RaisePropertyChanged(() => SortedNoteHeaders);
                RaisePropertyChanged(() => SortedNoteRows);
                return;
            }

            SaveChanges(note);
            RaisePropertyChanged(() => SortedNoteHeaders);
            SelectedNoteHeader = null;
        }

        public void CopyLinen()
        {
            if (!_dialogService.ShowQuestionDialog(" Do you want to copy collection quantity?"))
                return;

            foreach (var noteRow in SortedNoteRows.Where(x => ! Equals(x.PrimeCollectedQty, 0.0)))
            {
                noteRow.PrimeDeliveredQty = noteRow.PrimeCollectedQty;
                noteRow.ClientReceivedQty = noteRow.PrimeCollectedQty;
            }
        }

        public async void AddNewLinen()
        {
            var department = Departments.FirstOrDefault(x => x.Id == SelectedNoteHeader.DepartmentId);

            var addLinenListWindow = _resolverService.Resolve<AddLinenListViewModel>();
            await addLinenListWindow.InitializeAsync(department);
            var showDialog = _dialogService.ShowDialog(addLinenListWindow);

            if (!showDialog) return;

            var linenList = addLinenListWindow.GetLinenList();

            var noteRow = new NoteRowViewModel
            {
                LinenListId = linenList.OriginalObject.Id,
                LinenName = linenList.Name,
                NoteHeaderId = SelectedNoteHeader.Id,
                ServiceTypeId = 1,
                ColorStatus = 0,
                Weight = linenList.Weight,
                PriceUnit = linenList.UnitTypeId,
            };
            noteRow = AddNoteRowPriceWeight(noteRow);
            NoteRows.Add(noteRow);
            RaisePropertyChanged(() => SortedNoteRows);
        }

        public void RemoveNoteRow()
        {
            if (SelectedNoteRow == null)
                return;

            if (!_dialogService.ShowQuestionDialog($"Remove the note row '{SelectedNoteRow.LinenName}' ?"))
                return;

            if (SelectedNoteRow.OriginalObject.Id > 0)
            {
                _commonMethods.DeleteNoteRow(SelectedNoteRow);
            }

            NoteRows.Remove(SelectedNoteRow);

            RaisePropertyChanged(() => SortedNoteRows);
        }

        public void RemoveNote()
        {
            if (SelectedNoteHeader.NoteStatus >= (int) NoteStatusEnum.PreInvoice)
            {
                _dialogService.ShowInfoDialog("Deleting note is not allowed, Please contact administrator");
                return;
            }

            DeleteNote();
        }

        public void DeleteNote()
        {
            if (!_dialogService.ShowQuestionDialog($"Remove the Receipt note '{SelectedNoteHeader.Name}' ?"))
                return;

            var noteHeader = SelectedNoteHeader;
            var noteRows = SortedNoteRows.Where(x => x.OriginalObject.Id != 0).ToObservableCollection();

            _commonMethods.Delete(noteHeader, noteRows);

            foreach (var noteRow in SortedNoteRows)
            {
                NoteRows.Remove(noteRow);
            }

            NoteHeaders.Remove(SelectedNoteHeader);
            RaisePropertyChanged(() => SortedNoteHeaders);
        }

        public void SetSelectedNoteHeaders()
        {
            SelectedNoteHeaders = new List<NoteHeaderViewModel>();

            foreach (var selectedNote in SortedNoteHeaders.Where(x => x.IsSelected))
            {
                SelectedNoteHeaders.Add(selectedNote);
            }
        }

        private async void AddNewNote()
        {
            if (SelectedClient == null)
                return;

            var departmentWindow = _resolverService.Resolve<DepartmentSelectionViewModel>();
            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id)
                .ToObservableCollection();

            await departmentWindow.InitializeAsync(departments);
            var showDialog = _dialogService.ShowDialog(departmentWindow);
            if (!showDialog) return;

            var selectedDepartment = departmentWindow.GetSelectedDepartment();
            CreateNewNote(selectedDepartment);
        }

        public async void CreateNewNote(DepartmentViewModel depart)
        {
            var selectedDepartment = depart;
            if (selectedDepartment == null) return;

            var note = new NoteHeaderViewModel()
            {
                ClientId = SelectedClient.Id,
                DepartmentId = selectedDepartment.Id,
                DeliveryTypeId = 1,
                CollectionDate = DateTime.Now,
                DeliveryDate = DateTime.Now,
                ExpressCharge = selectedDepartment.Express,
                WeightPrice = SelectedClient.ClientInfo.WeighPrice,
                NoteStatus = (int)NoteStatusEnum.DeliveryNote,
                ColorStatus = 0,
            };

            note = await _commonMethods.GetNewNote(note);

            note.SetName();
            NoteHeaders.Add(note);
            AddNoteRow(note);
            RaisePropertyChanged(() => SortedNoteHeaders);
            SelectedNoteHeader = note;
        }

        public void AddNoteRow(NoteHeaderViewModel note)
        {
            var linens = new ObservableCollection<NoteRowViewModel>();
            foreach (var linenList in LinenLists.Where(x => x.DepartmentId == note.DepartmentId))
            {
                var linen = new NoteRowViewModel()
                {
                    LinenName = linenList.MasterLinen.Name,
                    LinenListId = linenList.Id,
                    ServiceTypeId = 1,
                    NoteHeaderId = note.Id,
                    ColorStatus = 0
                };
                linens.Add(AddNoteRowPriceWeight(linen));
            }
            NoteRows.AddRange(linens);
            RaisePropertyChanged(() => SortedNoteRows);
        }

        private void DuplicateNoteRow()
        {
            if (SelectedNoteRow == null) return;

            if (!_dialogService.ShowQuestionDialog($"Duplicate {SelectedNoteRow.LinenName} ?"))
                return;

            var noteRow = new NoteRowViewModel()
            {
                LinenListId = SelectedNoteRow.LinenListId,
                LinenName = SelectedNoteRow.LinenName,
                NoteHeaderId = SelectedNoteRow.NoteHeaderId,
                ServiceTypeId = SelectedNoteRow.ServiceTypeId,
                ColorStatus = 0,
            };

            NoteRows.Add(AddNoteRowPriceWeight(noteRow));
            RaisePropertyChanged(() => SortedNoteRows);
        }

        private void DuplicateNote()
        {
            if (SelectedNoteHeader == null) return;

            var dep = Departments?.FirstOrDefault(x => x.Id == SelectedNoteHeader?.DepartmentId);
            if (!_dialogService.ShowQuestionDialog($"Create \"{dep?.Name}\" Note?"))
                return;

            CreateNewNote(dep);
        }

        private async void ChangeDepartment()
        {
            if (SelectedNoteHeader == null) return;

            var departmentWindow = _resolverService.Resolve<DepartmentSelectionViewModel>();
            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id)
                .ToObservableCollection();
            await departmentWindow.InitializeAsync(departments);
            var showDialog = _dialogService.ShowDialog(departmentWindow);
            if (!showDialog) return;

            var newDepartment = departmentWindow.GetSelectedDepartment();
            if (newDepartment.Id == SelectedNoteHeader.DepartmentId) return;

            var noteHeader = SelectedNoteHeader;
            var newNoteRows = new ObservableCollection<NoteRowViewModel>();
            var oldNoteRows = NoteRows.Where(x =>
                x.NoteHeaderId == SelectedNoteHeader.Id &&
                (x.PrimeCollectedQty + x.PrimeDeliveredQty + x.ClientReceivedQty) > 0).ToList();

            var hasNullLinen = false;
            var linens = $"Below linens doesn't exist in {newDepartment.Name}\n";
            foreach (var oldRow in oldNoteRows)
            {
                var masterLinenId = LinenLists?.FirstOrDefault(x => x.Id == oldRow.LinenListId)?.MasterLinenId;
                var linen = LinenLists?.FirstOrDefault(x =>
                   x.DepartmentId == newDepartment.Id &&
                   x.MasterLinenId == masterLinenId);

                if (linen != null) continue;
                hasNullLinen = true;
                linens += $"* {oldRow.LinenName} \n";
            }

            if (hasNullLinen)
            {
                if (!_dialogService.ShowQuestionDialog(
                    $"{linens} \n Do you want to create new linens?"))
                {
                    return;
                }
            }


            foreach (var oldRow in oldNoteRows)
            {
                var masterLinenId = LinenLists?.FirstOrDefault(x => x.Id == oldRow.LinenListId)?.MasterLinenId;
                var linen = LinenLists?.FirstOrDefault(x =>
                    x.DepartmentId == newDepartment.Id &&
                    x.MasterLinenId == masterLinenId);

                if (linen == null)
                {
                    var oldLinen = LinenLists?.FirstOrDefault(x => x.Id == oldRow.LinenListId);
                    linen = new LinenList()
                    {
                        Weight = oldLinen.Weight,
                        UnitId = oldLinen.UnitId,
                        MasterLinenId = oldLinen.MasterLinenId,
                        DepartmentId = newDepartment.Id,
                        Active = true,
                        DryCleaning = oldLinen.DryCleaning,
                        Laundry = oldLinen.Laundry,
                        Pressing = oldLinen.Pressing,
                        ClientId = newDepartment.ClientId,
                    };
                    await _dataService.AddOrUpdateAsync(linen);
                    LinenLists?.Add(linen);
                }

                newNoteRows.Add(new NoteRowViewModel()
                {
                    LinenName = oldRow.LinenName,
                    LinenListId = linen.Id,
                    ServiceTypeId = oldRow.ServiceTypeId,
                    NoteHeaderId = noteHeader.Id,
                    ColorStatus = oldRow.ColorStatus,
                    ClientReceivedQty = oldRow.ClientReceivedQty,
                    Comment = oldRow.Comment,
                    Price = oldRow.Price,
                    PriceUnit = oldRow.PriceUnit,
                    PrimeCollectedQty = oldRow.PrimeCollectedQty,
                    PrimeDeliveredQty = oldRow.PrimeDeliveredQty,
                    Weight = oldRow.Weight,
                });
            }

            noteHeader.DepartmentId = newDepartment.Id;
            SaveNoteHeader(noteHeader);
            SaveNoteRows(newNoteRows);
            NoteRows.AddRange(newNoteRows);

            foreach (var noteRow in oldNoteRows)
            {
                NoteRows.Remove(noteRow);
                if (noteRow.OriginalObject.Id > 0)
                    await _dataService.DeleteAsync(noteRow.OriginalObject);
            }

            RaisePropertyChanged(() => SortedNoteHeaders);
            RaisePropertyChanged(() => SortedNoteRows);
        }

        #region PrintingRegion

        private bool GetShowPrice()
        {
            var showPrice = false;

            if (SelectedClient.ClientInfo != null)
                switch (SelectedClient.ClientInfo.NoteId)
                {
                    case (int)NoteTypeEnum.WithPrice:
                        showPrice = true;
                        break;
                }
            return showPrice;
        }

        private void Print(object param)
        {
            var printParam = int.Parse(param.ToString());
            if (SelectedNoteHeader == null) return;
            var note = SelectedNoteHeader;
            SetSelectedNoteHeaders();

            if (printParam == 0)
            {
                Generate(SelectedNoteHeader);
            }
            else
            {
                SelectedNoteHeaders.ForEach(Generate);
                if (SelectedNoteHeaders.Count == 0) return;
            }

            var report = GetReport(printParam, note);

            if (report == null) return;

            if (!_reportService.ShowReportPreviewBool(report))
            {
                PrintCopies(report);
            }
        }

        public void PrintCopies(IReport reportBase)
        {
            var report = (NotesReportBase)reportBase;

            var copyNumberWindow = _resolverService.Resolve<PrintCopyNumberViewModel>();
            
            var showDialog = _dialogService.ShowDialog(copyNumberWindow);
            if (!showDialog) return;

            var copyNumber = copyNumberWindow.SelectedCopyNumber;
            switch (copyNumber)
            {
                case 1:
                    report.WaterMark = null;
                    _reportService.Print(report);
                    break;
                case 2:
                    report.WaterMark = "LAUNDRY COPY";
                    _reportService.Print(report);

                    report.WaterMark = "CLIENT COPY";
                    _reportService.Print(report);
                    break;
                case 3:
                    report.WaterMark = "LAUNDRY COPY";
                    _reportService.Print(report);

                    report.WaterMark = "CLIENT COPY";
                    _reportService.Print(report);
                    _reportService.Print(report);
                    break;
            }
        }

        private IReport GetReport(int selectedReportType, NoteHeaderViewModel selectedNote)
        {
            var note = selectedNote;
            var reportType = selectedReportType;
            if (note == null) return null;
            IReport report;

            switch (reportType)
            {
                case 0:
                    report = GetNoteReport(GetShowPrice(), note);
                    return report;

                case (int)NoteReportTypesEnum.ByTotalPiece:
                    report = GetReportByTotal(ReportBase(GetShowPrice()));
                    return report;

                case (int)NoteReportTypesEnum.ByDepartment:
                    report = GetReportByNote(ReportBase(GetShowPrice()));
                    return report;

                case (int)NoteReportTypesEnum.ByLinen:
                    report = GetReportByLinen(ReportBase(GetShowPrice()));
                    return report;

                default: return null;
            }
        }

        private IReport GetNoteReport(bool priceVisibility, NoteHeaderViewModel selectedNote)
        {
            var note = selectedNote;
            var deliveryType = DeliveryTypes.FirstOrDefault(x => x.Id == note?.DeliveryTypeId);
            var report = new NoteReport
            {
                ClientAddress = SelectedClient.ClientInfo.Address,
                ClientName = SelectedClient.Name,
                Comment = note.Comment,
                DeliveryDate = note.DeliveryDate,
                DeliveryKg = note.DeliveryWeight,
                DeliveryType = deliveryType?.Name,
                DepartmentName = Departments.Single(x => x.Id == note.DepartmentId).Name,
                NoteNumber = note.Name,
                CollectionDate = note.CollectionDate,
                CollectionWeight = note.CollectionWeight,
                ClientLogo = Extension.GetBitmap(Extension.GetBitmapImage(SelectedClient.ClientInfo.Logo)),
                PrimeLogo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo)),
                ShowItemPrice = priceVisibility,
            };

            var reportItems = new List<NotesReportRowItem>();
            foreach (var notRow in NoteRows.Where(x => x.NoteHeaderId == note.Id))
            {
                if (Equals(notRow.PrimeCollectedQty, 0.0) && Equals(notRow.PrimeDeliveredQty, 0.0) && Equals(notRow.ClientReceivedQty, 0.0)) continue;

                var serviceType = ServiceTypes.FirstOrDefault(x => x.Id == notRow.ServiceTypeId)?.Name;
                var item = new NotesReportRowItem()
                {
                    Id = notRow.LinenListId,
                    Name = notRow.LinenName,
                    Price = notRow.Price,
                    DeliveryQuantity = notRow.PrimeDeliveredQty,
                    CollectionQuantity = notRow.PrimeCollectedQty,
                    Remark = notRow.Comment,
                    ServiceType = serviceType,
                };

                if (ShowClientReceivedQty)
                {
                    item.ClientReceivedQuantity = notRow.ClientReceivedQty;
                }

                if (report.ShowItemPrice)
                {
                    switch (note.DeliveryTypeId)
                    {
                        case (int)DeliveryTypeEnum.Express:
                            item.Price += item.Price * note.ExpressCharge;
                            break;
                        case (int)DeliveryTypeEnum.ReWash:
                            item.Price = 0;
                            break;
                    }
                }
                reportItems.Add(item);
            }

            report.ItemRows = reportItems.OrderBy(x => x.Name).ToList();
            return report;
        }

        private IReport ReportBase(bool priceVisible)
        {
            var report = (NotesReportBase)ReportHeader(priceVisible);
            var reportItems = new List<NotesReportNoteItem>();

            foreach (var noteHeader in SelectedNoteHeaders)
            {
                var itemNote = new NotesReportNoteItem()
                {
                    Items = new List<NotesReportRowItem>(),
                    CollectionNoteWeight = noteHeader.CollectionWeight,
                    DeliveryNoteWeight = noteHeader.DeliveryWeight,
                    Name = noteHeader.Name,
                    Id = noteHeader.Id,
                    DepartmentId = noteHeader.Id,
                    DepartmentName = Departments.Single(x => x.Id == noteHeader.DepartmentId).Name,
                    DelivereddDate = noteHeader.DeliveryDate,
                    CollectionDate = noteHeader.CollectionDate,
                    Comment = noteHeader.Comment,
                    DeliveryType = DeliveryTypes.FirstOrDefault(x => x.Id == noteHeader.DeliveryTypeId)?.Name,
                };

                foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id))
                {
                    if (Equals(noteRow.PrimeCollectedQty, 0.0) && Equals(noteRow.PrimeDeliveredQty, 0.0) &&
                        Equals(noteRow.ClientReceivedQty, 0.0)) continue;

                    var item = new NotesReportRowItem()
                    {
                        CollectionQuantity = noteRow.PrimeCollectedQty,
                        DeliveryQuantity = noteRow.PrimeDeliveredQty,
                        Id = noteRow.LinenListId,
                        LinenListId = noteRow.LinenListId,
                        Name = noteRow.LinenName,
                        ServiceType = ServiceTypes.FirstOrDefault(x => x.Id == noteRow.ServiceTypeId)?.Name,
                        ServiceTypeId = noteRow.ServiceTypeId,
                        Remark = noteRow.Comment,
                        Price = noteRow.Price,
                    };

                    if (ShowClientReceivedQty)
                    {
                        item.ClientReceivedQuantity = noteRow.ClientReceivedQty;
                    }

                    if (report.ShowItemPrice)
                    {
                        switch (noteHeader.DeliveryTypeId)
                        {
                            case (int)DeliveryTypeEnum.Express:
                                item.Price += item.Price * noteHeader.ExpressCharge;
                                break;
                            case (int)DeliveryTypeEnum.ReWash:
                                item.Price = 0;
                                break;
                        }
                    }
                    itemNote.Items.Add(item);
                }
                reportItems.Add(itemNote);
            }

            report.Items = reportItems.OrderBy(x => x.Name).ToList();
            return report;
        }

        private IReport ReportHeader(bool priceVisible)
        {
            var report = new NotesReportBase()
            {
                ClientAddress = SelectedClient.ClientInfo.Address,
                ClientName = SelectedClient.Name,
                ClientLogo = Extension.GetBitmap(Extension.GetBitmapImage(SelectedClient.ClientInfo.Logo)),
                PrimeLogo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo)),
                PrintDate = PrintTime,
                ShowItemPrice = priceVisible,
            };

            var departments = new List<Department>();
            foreach (var noteHeader in SelectedNoteHeaders)
            {
                if (departments.Find(x => x.Id == noteHeader.DepartmentId) == null)
                {
                    departments.Add(Departments.FirstOrDefault(x => x.Id == noteHeader.DepartmentId)?.OriginalObject);
                }
            }

            var departmentsParents = new List<Department>();
            foreach (var department in departments)
            {
                if (department.ParentId == null)
                {
                    departmentsParents.Add(department);
                    continue;
                }
                var departmentParent = departmentsParents.Find(x => x.Id == department.ParentId);

                if (departmentParent == null)
                {
                    departmentsParents.Add(Departments.Find(x => x.Id == department.ParentId)?.OriginalObject);
                }
            }

            if (departmentsParents.Count == 1)
            {
                report.DepartmentName = departmentsParents.FirstOrDefault()?.Name;
            }
            else if (departmentsParents.Count > 1)
            {
                foreach (var department in departments)
                {
                    report.DepartmentName += department.Name + ", ";
                }
            }

            return report;
        }

        private IReport GetReportByLinen(IReport baseReport)
        {
            var reportBase = (NotesReportBase)baseReport;

            var report = new NoteByLinenReport(reportBase);

            var byLinenReportItems = new List<NotesReportRowItem>();

            foreach (var reportNoteItem in reportBase.Items)
            {
                report.DeliveryKg += reportNoteItem.DeliveryNoteWeight;
                report.CollectionWeight += reportNoteItem.CollectionNoteWeight;
                report.NoteNumber += reportNoteItem.Name + ", ";

                foreach (var itemRow in reportNoteItem.Items)
                {
                    var price = itemRow.Price * itemRow.DeliveryQuantity;

                    if (ShowClientReceivedQty)
                    {
                        price = (double) (itemRow.Price * itemRow.ClientReceivedQuantity);
                    }

                    var item = byLinenReportItems.FirstOrDefault(x => x.LinenListId == itemRow.LinenListId && x.ServiceTypeId == itemRow.ServiceTypeId);

                    if (item == null)
                    {
                        var byLinenReportItem = new NotesReportRowItem
                        {
                            Id = itemRow.Id,
                            LinenListId = itemRow.LinenListId,
                            Name = itemRow.Name,
                            DepartmentName = reportNoteItem.DepartmentName,
                            DeliveryQuantity = itemRow.DeliveryQuantity,
                            CollectionQuantity = itemRow.CollectionQuantity,
                            ClientReceivedQuantity = itemRow.ClientReceivedQuantity,
                            Price = price,
                            ServiceType = itemRow.ServiceType,
                            ServiceTypeId = itemRow.ServiceTypeId,
                            Remark = itemRow.Remark,
                        };

                        if (ShowClientReceivedQty)
                        {
                            byLinenReportItem.ClientReceivedQuantity = itemRow.ClientReceivedQuantity;
                        }

                        byLinenReportItems.Add(byLinenReportItem);
                        continue;
                    }

                    foreach (var rowReport in byLinenReportItems.Where(x => x.Id == itemRow.Id && x.ServiceTypeId == itemRow.ServiceTypeId))
                    {
                        rowReport.DeliveryQuantity += itemRow.DeliveryQuantity;
                        rowReport.CollectionQuantity += itemRow.CollectionQuantity;
                        rowReport.Remark += (", " + itemRow.Remark);
                        rowReport.Price += price;

                        if (ShowClientReceivedQty)
                        {
                            rowReport.ClientReceivedQuantity += itemRow.ClientReceivedQuantity;
                        }
                        break;
                    }
                }
            }

            report.ItemRows = byLinenReportItems.OrderBy(x => x.DepartmentName).ToList();
            return report;
        }

        private IReport GetReportByTotal(IReport baseReport)
        {
            var reportBase = (NotesReportBase)baseReport;
            var report = new NoteByTotalReport(reportBase) { Items = new List<NotesReportNoteItem>() };

            foreach (var noteHeader in reportBase.Items)
            {
                report.DeliveryKg += noteHeader.DeliveryNoteWeight;
                report.CollectionWeight += noteHeader.CollectionNoteWeight;

                var item = new NotesReportNoteItem()
                {
                    Name = noteHeader.Name,
                    DepartmentName = noteHeader.DepartmentName,
                    DepartmentId = noteHeader.DepartmentId,
                    Id = noteHeader.Id,
                };

                if (ShowClientReceivedQty)
                {
                    item.ClientReceivedQuantity = 0;
                }

                foreach (var noteRow in noteHeader.Items)
                {
                    item.CollectionQuantity += noteRow.CollectionQuantity;
                    item.DeliveryQuantity += noteRow.DeliveryQuantity;

                    if (ShowClientReceivedQty)
                    {
                        item.Price += noteRow.Price * (double)noteRow.ClientReceivedQuantity;
                        item.ClientReceivedQuantity += noteRow.ClientReceivedQuantity;
                    }
                    else
                    {
                        item.Price += noteRow.Price * noteRow.DeliveryQuantity;
                    }
                }
                report.Items.Add(item);
            }
            report.Items.OrderBy(x => x.Name);
            return report;
        }

        private IReport GetReportByNote(IReport baseReport)
        {
            var reportBase = (NotesReportBase)baseReport;
            var report = new NoteByNoteReport(reportBase);

            foreach (var noteHeader in reportBase.Items)
            {
                report.DeliveryKg += noteHeader.DeliveryNoteWeight;
                report.CollectionWeight += noteHeader.CollectionNoteWeight;

                report.Items.Add(noteHeader);

                foreach (var itemRow in noteHeader.Items.OrderBy(x => x.Name))
                {
                    itemRow.NoteId = noteHeader.Id;
                    report.ReportNoteRows.Add(itemRow);
                }
            }
            report.ReportNoteRows.OrderBy(x => x.Name);
            return report;
        }

        #endregion
    }
}
