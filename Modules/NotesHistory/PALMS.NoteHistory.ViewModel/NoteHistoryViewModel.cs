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
using PALMS.NoteHistory.ViewModel.EntityViewModel;
using PALMS.NoteHistory.ViewModel.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.NoteHistory.ViewModel
{
    public class NoteHistoryViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private List<Client> _clients;
        private List<Department> _departments;
        private NoteHeaderViewModel _selectedNoteHeader;
        private NoteRowViewModel _selectedNoteRow;
        private List<UnitViewModel> _noteStatuses;
        private UnitViewModel _selectedNoteStatus;
        private List<UnitViewModel> _serviceTypes;
        private DateTime _dateStart;
        private DateTime _dateEnd;
        private List<UnitViewModel> _deliveryTypes;
        private List<LinenList> _linens;
        private bool _betweenDates;

        public bool BetweenDates
        {
            get => _betweenDates;
            set => Set(ref _betweenDates, value);
        }
        public List<LinenList> Linens
        {
            get => _linens;
            set => Set(ref _linens, value);
        }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public DateTime DateEnd
        {
            get => _dateEnd;
            set => Set(ref _dateEnd, value);
        }
        public DateTime DateStart
        {
            get => _dateStart;
            set => Set(ref _dateStart, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public UnitViewModel SelectedNoteStatus
        {
            get => _selectedNoteStatus;
            set => Set(ref _selectedNoteStatus, value);
        }
        public List<UnitViewModel> NoteStatuses
        {
            get => _noteStatuses;
            set => Set(ref _noteStatuses, value);
        }
        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public ObservableCollection<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => SortNoteHeaders();
        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRows();

        public RelayCommand SaveCommand { get; }
        public RelayCommand GetNotesCommand { get; }
        public RelayCommand ChangeToPreInvoiceCommand { get; }
        public RelayCommand DeleteRowCommand { get; }
        public RelayCommand DeleteNoteCommand { get; }  
        public RelayCommand ChangeClientCommand { get; }  
        public RelayCommand AddLinenCommand { get; }  
        public RelayCommand SavePreInvoiceCommand { get; }  

        public NoteHistoryViewModel(IResolver resolver, IDataService dataService, IDispatcher dispatcher, IDialogService dialog)
        {
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialog ?? throw new ArgumentNullException(nameof(dialog));

            SaveCommand = new RelayCommand(SaveChangedNotes);
            GetNotesCommand = new RelayCommand(GetNotes);
            ChangeToPreInvoiceCommand = new RelayCommand(ChangeToPreInvoice);
            DeleteRowCommand = new RelayCommand(DeleteRow, ()=> SelectedNoteRow != null);
            DeleteNoteCommand = new RelayCommand(DeleteNote, () => SelectedNoteHeader != null);
            ChangeClientCommand = new RelayCommand(ChangeClient, ()=> SelectedNoteHeader != null);
            AddLinenCommand = new RelayCommand(AddNewLinen, ()=> SelectedNoteHeader != null);
            SavePreInvoiceCommand = new RelayCommand(SavePreInvoice, ()=> SelectedNoteHeader != null);

            DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
            ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
            NoteStatuses = EnumExtentions.GetValues<NoteStatusEnum>();

            NoteStatuses.Remove(NoteStatuses.FirstOrDefault(x => x.Id == (int) NoteStatusEnum.NewNote));
            NoteStatuses.Remove(NoteStatuses.FirstOrDefault(x => x.Id == (int) NoteStatusEnum.CollectionNote));

            GetNotes();

            DateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);
            DateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            PropertyChanged += OnPropertyChanged;

        }
        public async Task InitializeAsync()
        {

        }

        public async void GetNotes()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
                var clients = client;
                _dispatcher.RunInMainThread(() => Clients = clients.OrderBy(x => x.ShortName).ToList());

                var linenList = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                var linenLists = linenList;
                _dispatcher.RunInMainThread(() => Linens = linenLists.ToList());

                var department = await _dataService.GetAsync<Department>();
                _dispatcher.RunInMainThread(() => Departments = department.ToList());

                var noteHeader = await _dataService.GetAsync<NoteHeader>();
                var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());
                NoteHeaders.ForEach(SubscribeItem);
                NoteHeaders.CollectionChanged += NoteHeadersCollectionChanged;

                var noteRow = await _dataService.GetAsync<NoteRow>();
                var noteRows = noteRow.Select(x => new NoteRowViewModel(x));
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());
                NoteRows.ForEach(SubscribeItem);

                NoteRows.CollectionChanged += NoteRowsCollectionChanged;

                RaisePropertyChanged(()=> SortedNoteHeaders);
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
            if (e.PropertyName == nameof(DateStart))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(DateEnd))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
            if (e.PropertyName == nameof(SelectedNoteStatus))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }

            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                DeleteRowCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(BetweenDates))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }

            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                RaisePropertyChanged(() => SortedNoteRows);
                DeleteNoteCommand.RaiseCanExecuteChanged();
                AddLinenCommand.RaiseCanExecuteChanged();
                ChangeClientCommand.RaiseCanExecuteChanged();
                SavePreInvoiceCommand.RaiseCanExecuteChanged();
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

        private void UnSubscribeItem(NoteHeaderViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(NoteHeaderViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is NoteRowViewModel item)
            {
                if (item.HasChanges() || SelectedNoteHeader.HasChanges())
                {
                    SelectedNoteHeader.ColorStatus = 1;
                }
                else
                {
                    SelectedNoteHeader.ColorStatus = 0;
                }
                if (e.PropertyName == nameof(item.ServiceTypeId))
                {
                    var row = AddNoteRowPriceWeight(item);
                    item.Price = row.Price;
                    SetNoteRowColorStatus(item);
                }
            }

            if (sender is NoteHeaderViewModel note)
            {
                if (note.HasChanges() || SortedNoteRows.Any(x => x.HasChanges()))
                {
                    note.ColorStatus = 1;
                }
                else
                {
                    note.ColorStatus = 0;
                }

            }
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

        private void NoteHeadersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<NoteHeaderViewModel>())
                {
                    SubscribeItem(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<NoteHeaderViewModel>())
                {
                    UnSubscribeItem(item);
                }
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


        public NoteRowViewModel AddNoteRowPriceWeight(NoteRowViewModel row)
        {
            var noteRow = row;

            var linen = Linens?.FirstOrDefault(x => x.Id == noteRow.LinenListId);
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
            var notes = NoteHeaders;
            var noteHeaders = new ObservableCollection<NoteHeaderViewModel>();

            if (BetweenDates)
            {
                notes = NoteHeaders.Where(x => x.DeliveryDate >= DateStart && x.DeliveryDate <= DateEnd)
                    .ToObservableCollection();
            }

            if (SelectedNoteStatus == null)
            {
                noteHeaders = notes;
            }
            else
            {
                noteHeaders.AddRange(notes.Where(x=> x.NoteStatus == SelectedNoteStatus.Id));
            }

            SelectedNoteHeader = null;
           return noteHeaders?.OrderByDescending(x=> x.Id).ToObservableCollection();
        }

        public ObservableCollection<NoteRowViewModel> SortNoteRows()
        {
            return NoteRows?.Where(x => x.NoteHeaderId == SelectedNoteHeader?.Id).OrderBy(x => x.LinenName).ToObservableCollection();
        }

        private void SaveChangedNotes()
        {
            var notes = SortedNoteHeaders.Where(x => x.HasChanges()).ToList();
            var noteRows = NoteRows.Where(x=> x.HasChanges() ||x.IsNew).ToList();

            if (!notes.Any() && !noteRows.Any())
            {
                _dialogService.ShowInfoDialog("Notes/Linens doesn't has changes");
                return;
            }

            if (!_dialogService.ShowQuestionDialog("Do you want to save all changed NOTEs & Linens?")) return;

            Save(notes, noteRows);
        }
        private void ChangeToPreInvoice()
        {
            var notes = SortedNoteHeaders.Where(x => x.IsSelected).ToList();
            notes.ForEach(x=> x.NoteStatus = (int) NoteStatusEnum.PreInvoice);
            SaveNotes(notes);
        }

        private void SavePreInvoice()
        {
            if(SelectedNoteHeader == null) return;

            var notes = new List<NoteHeaderViewModel>();
            SelectedNoteHeader.NoteStatus = (int) NoteStatusEnum.PreInvoice;
            notes.Add(SelectedNoteHeader);

            var rows = NoteRows.Where(x => x.NoteHeaderId == SelectedNoteHeader.Id).ToList();

            if (!_dialogService.ShowQuestionDialog($"Do you want to save {SelectedNoteHeader.Name} ?")) return;
            Save(notes, rows);
        }

        private void SaveNotes(List<NoteHeaderViewModel> noteHeaders)
        {
            var notes = noteHeaders;
            var noteRows = new List<NoteRowViewModel>();

            foreach (var note in notes)
            {
                noteRows.AddRange(NoteRows.Where(x => x.NoteHeaderId == note.Id && x.HasChanges()));
            }

            if (!notes.Any() && !noteRows.Any())
            {
                _dialogService.ShowInfoDialog("Notes/Linens doesn't has changes");
                return;
            }

            if (!_dialogService.ShowQuestionDialog("Do you want to save all changed NOTEs & Linens?")) return;

            Save(notes, noteRows);
        }

        private async void Save(List<NoteHeaderViewModel> notes, List<NoteRowViewModel> rows)
        {
            if (notes!=null && notes.Any(x=> x.HasChanges()))
            {
                notes.ForEach(x=> x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(notes.Select(x => x.OriginalObject));
            }

            if (rows != null && rows.Any(x => x.HasChanges()))
            {
                rows.ForEach(x => x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(rows.Select(x => x.OriginalObject));
            }

            _dialogService.ShowInfoDialog("All changes saved");
        }

        public void DeleteRow()
        {
            var row = SelectedNoteRow;
            if (row == null)
                return;

            if (!_dialogService.ShowQuestionDialog($"Remove the note row '{row.LinenName}' ?"))
                return;

            if (row.OriginalObject.Id > 0)
            {
                _dataService.DeleteAsync(row.OriginalObject);
            }

            NoteRows.Remove(row);
            RaisePropertyChanged(() => SortedNoteRows);
        }

        public void DeleteNote()
        {
            var note = SelectedNoteHeader;
            if(note == null) return;

            if (!_dialogService.ShowQuestionDialog($"Remove the Receipt note '{note.Name}' ?"))
                return;

            var noteRows = NoteRows.Where(x => x.NoteHeaderId == note.Id);

            _dataService.DeleteAsync(note.OriginalObject);
            foreach (var row in noteRows)
            {
                _dataService.DeleteAsync(row.OriginalObject);
                NoteRows.Remove(row);
            }

            SelectedNoteHeader = null;
            NoteHeaders.Remove(note);
            RaisePropertyChanged(() => SortedNoteHeaders);
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

        public async void ChangeClient()
        {
            var changeWindow = _resolverService.Resolve<ClientDepSelectViewModel>();
            await changeWindow.InitializeAsync();
            var showDialog = _dialogService.ShowDialog(changeWindow);

            if (!showDialog) return;

            var newDepartment = changeWindow.GetSelectedDepartment();
            if (newDepartment.Id == SelectedNoteHeader.DepartmentId) return;
            var noteHeader = SelectedNoteHeader;
            var oldNoteRows = NoteRows.Where(x =>x.NoteHeaderId == SelectedNoteHeader.Id).ToList();

            var hasNullLinen = false;
            var linens = $"Below linens doesn't exist in {newDepartment.Name}\n";
            foreach (var oldRow in oldNoteRows)
            {
                var masterLinenId = Linens?.FirstOrDefault(x => x.Id == oldRow.LinenListId)?.MasterLinenId;
                var linen = Linens?.FirstOrDefault(x =>
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


            var newNoteRows = new List<NoteRowViewModel>();
            foreach (var oldRow in oldNoteRows)
            {
                var masterLinenId = Linens?.FirstOrDefault(x => x.Id == oldRow.LinenListId)?.MasterLinenId;
                var linen = Linens?.FirstOrDefault(x =>
                    x.DepartmentId == newDepartment.Id &&
                    x.MasterLinenId == masterLinenId);

                if (linen == null)
                {
                    var oldLinen = Linens?.FirstOrDefault(x => x.Id == oldRow.LinenListId);
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
                    Linens?.Add(linen);
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
            noteHeader.ClientId = newDepartment.ClientId;
            noteHeader.AcceptChanges();
            await _dataService.AddOrUpdateAsync(noteHeader.OriginalObject);

            newNoteRows.ForEach(x=> x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(newNoteRows.Select(x=> x.OriginalObject));

            NoteRows.AddRange(newNoteRows);

            foreach (var noteRow in oldNoteRows)
            {
                NoteRows.Remove(noteRow);
                if (noteRow.OriginalObject.Id > 0)
                    await _dataService.DeleteAsync(noteRow.OriginalObject);
            }

            RaisePropertyChanged(() => SortedNoteHeaders);
        }
    }
}
