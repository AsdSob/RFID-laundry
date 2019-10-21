using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;


namespace PALMS.Invoices.ViewModel.Windows
{
    public class ChangeNotesLinenViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }
        public bool IsChanged { get; set; }

        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;

        private ObservableCollection<NoteRowViewModel> _noteRows;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;

        private List<Department> _departments;
        private Department _selectedDepartment;
        private Department _selectedNewDepartment;

        private List<LinenList> _linenLists;
        private LinenList _selectedLinenList;

        private int _selectedServiceTypeId;
        private List<UnitViewModel> _serviceTypes;

        private List<Client> _clients;
        private Client _selectedClient;
        private NoteRowViewModel _selectedNoteRow;
        private ObservableCollection<NoteHeaderViewModel> _selectedNoteHeaders;
        private bool _applyForAllNotes;


        public bool ApplyForAllNotes
        {
            get => _applyForAllNotes;
            set => Set(ref _applyForAllNotes, value);
        }
        public ObservableCollection<NoteHeaderViewModel> SelectedNoteHeaders
        {
            get => _selectedNoteHeaders;
            set => Set(ref _selectedNoteHeaders, value);
        }
        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public int SelectedServiceTypeId
        {
            get => _selectedServiceTypeId;
            set => Set(ref _selectedServiceTypeId, value);
        }
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public Department SelectedNewDepartment
        {
            get => _selectedNewDepartment;
            set => Set(ref _selectedNewDepartment, value);
        }
        public LinenList SelectedLinenList
        {
            get => _selectedLinenList;
            set => Set(ref _selectedLinenList, value);
        }
        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
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
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public int? SelectedInvoiceId { get; set; }

        public List<Department> SortedDepartments => SortDepartments();
        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRows();
        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => SortNoteHeaders();
        public List<LinenList> SortedLinenLists =>
            LinenLists?.Where(x => x.DepartmentId == SelectedNewDepartment?.Id).OrderBy(x => x.MasterLinen.Name).ToList();

        public RelayCommand CloseCommand { get; }
        public RelayCommand ChangeLinenCommand { get; }

        public ChangeNotesLinenViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            CloseCommand = new RelayCommand(Close);
            ChangeLinenCommand = new RelayCommand(ChangeLinen);
        }

        public async Task InitializeAsync(Client client, int? invoiceId)
        {
            if (client == null) return;

            _dialogService.ShowBusy();

            try
            {
                SelectedInvoiceId = invoiceId;
                SelectedClient = client;

                var departments = await _dataService.GetAsync<Department>(x=> x.ClientId == SelectedClient.Id);
                _dispatcher.RunInMainThread(() => Departments = departments.ToList());

                var linenList = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                _dispatcher.RunInMainThread(() => LinenLists = linenList.ToList());

                var notes = await _dataService.GetAsync<NoteHeader>(x => x.InvoiceId == SelectedInvoiceId && x.ClientId == SelectedClient.Id);
                var noteHeader = notes.Select(x => new NoteHeaderViewModel(x));
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeader.ToObservableCollection());

                var noteRows = new List<NoteRowViewModel>();
                foreach (var note in NoteHeaders)
                {
                    var row = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    var noteRow = row.Select(x => new NoteRowViewModel(x));
                    noteRows.AddRange(noteRow);
                }

                foreach (var row in noteRows)
                {
                    row.LinenName = LinenLists.FirstOrDefault(x=> x.Id == row.LinenListId)?.MasterLinen.Name;
                }

                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());

                ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();

                //SelectedClient = null;
                ApplyForAllNotes = false;
                IsChanged = false;
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

            ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                SelectedNewDepartment = SelectedDepartment;

                RaisePropertyChanged(() => SortedNoteHeaders);
                RaisePropertyChanged(() => SortedNoteRows);
            }
            if (e.PropertyName == nameof(SelectedNewDepartment))
            {
                RaisePropertyChanged(() => SortedLinenLists);
            }
            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                SetServiceType();
            }
        }

        public void SetServiceType()
        {
            if (SelectedNoteRow == null) return;

            SelectedServiceTypeId = SelectedNoteRow.ServiceTypeId;
        }

        private List<Department> SortDepartments()
        {
            if (SelectedClient == null) return null;

            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id).ToList();
            return departments;
        }

        private ObservableCollection<NoteHeaderViewModel> SortNoteHeaders()
        {
            if (SelectedDepartment == null || NoteHeaders == null) return null;

            var notes = NoteHeaders?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
            return notes;
        }

        private ObservableCollection<NoteRowViewModel> SortNoteRows()
        {
            var sortedNoteRows = new ObservableCollection<NoteRowViewModel>();
            if (NoteRows == null || SelectedDepartment == null) return null;

            foreach (var note in NoteHeaders.Where(x => x.DepartmentId == SelectedDepartment.Id))
            {
                foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == note.Id))
                {
                    var hasSameLinen = sortedNoteRows.Any(x =>
                        x.LinenListId == noteRow.LinenListId && x.ServiceTypeId == noteRow.ServiceTypeId);

                    if (!hasSameLinen)
                    {
                        sortedNoteRows.Add(noteRow);
                    }
                }
            }
            return sortedNoteRows.OrderBy(x => x.LinenName).ToObservableCollection();
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                CloseAction?.Invoke(IsChanged);
        }

        private async void Save(NoteRowViewModel noteRow)
        {
            if (noteRow == null || !noteRow.HasChanges())
                return;

            IsChanged = true;
            noteRow.AcceptChanges();
            await _dataService.AddOrUpdateAsync(noteRow.OriginalObject);
        }

        private void SetSelectedNotes()
        {
            ApplyForAllNotes = true;

            if (ApplyForAllNotes)
            {
                SelectedNoteHeaders = SortedNoteHeaders;
            }
            else
            {
                SelectedNoteHeaders = SortedNoteHeaders.Where(x => x.IsSelected).ToObservableCollection();
            }
        }

        private void ChangeLinen()
        {
            if(SelectedNoteRow == null || SelectedLinenList == null)return;
            SetSelectedNotes();
            if(SelectedNoteHeaders.Count<= 0)return;

            if (SelectedDepartment.Id == SelectedNewDepartment.Id)
                ChangeToSameDep();
            else
            {
                ChangeToOtherDepartment();
            }
        }

        private async void ChangeToSameDep()
        {
            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to change - {SelectedNoteRow.LinenName} - of {SelectedDepartment.Name} - to - {SelectedLinenList.MasterLinen.Name} - in all Notes ? ")
            ) return;

            foreach (var note in SelectedNoteHeaders)
            {
                // получаем список белья
                var noteRows = NoteRows.Where(x => x.NoteHeaderId == note.Id).ToList();
                if (!noteRows.Any(x =>
                    x.LinenListId == SelectedNoteRow.LinenListId && x.ServiceTypeId == SelectedNoteRow.ServiceTypeId)) continue;

                var oldNoteRow = noteRows.FirstOrDefault(x =>
                    x.LinenListId == SelectedNoteRow.LinenListId && x.ServiceTypeId == SelectedNoteRow.ServiceTypeId);

                // если новое бельё имеется
                if (noteRows.Any(x => x.LinenListId == SelectedLinenList.Id && x.ServiceTypeId == SelectedServiceTypeId))
                {
                    var existNoteRow = noteRows.FirstOrDefault(x =>
                        x.LinenListId == SelectedLinenList.Id && x.ServiceTypeId == SelectedServiceTypeId);

                    existNoteRow.PrimeDeliveredQty += oldNoteRow.PrimeDeliveredQty;
                    existNoteRow.PrimeCollectionQty += oldNoteRow.PrimeCollectionQty;
                    existNoteRow.ClientReceivedQty += oldNoteRow.ClientReceivedQty;
                    existNoteRow.Comment += ". " + oldNoteRow.Comment;

                    Save(existNoteRow);
                }

                else
                {
                    var newNoteRow = new NoteRowViewModel()
                    {
                        ServiceTypeId = SelectedServiceTypeId,
                        NoteHeaderId = note.Id,
                        LinenName = SelectedLinenList.MasterLinen.Name,
                        LinenListId = SelectedLinenList.Id,
                        Weight = SelectedLinenList.Weight,
                        Comment = oldNoteRow.Comment,
                        PrimeCollectionQty = oldNoteRow.PrimeCollectionQty,
                        PrimeDeliveredQty = oldNoteRow.PrimeDeliveredQty,
                        ClientReceivedQty = oldNoteRow.ClientReceivedQty,
                    };

                    switch (newNoteRow.ServiceTypeId)
                    {
                        case (int)ServiceTypeEnum.Laundry:
                            newNoteRow.Price = SelectedLinenList.Laundry;
                            break;

                        case (int)ServiceTypeEnum.DryCleaning:
                            newNoteRow.Price = SelectedLinenList.DryCleaning;
                            break;

                        case (int)ServiceTypeEnum.Pressing:
                            newNoteRow.Price = SelectedLinenList.Pressing;
                            break;
                    }
                    Save(newNoteRow);
                }

                await _dataService.DeleteAsync(oldNoteRow.OriginalObject);
                if (_dialogService.ShowQuestionDialog($"Do you want to close window?"))
                    CloseAction(true);
            }
        }

        private async void ChangeToOtherDepartment()
        {
            _dialogService.ShowWarnigDialog("After Applying this changes its not possible to recover it");

            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to move All - {SelectedNoteRow.LinenName} s - of {SelectedDepartment.Name} - to - {SelectedLinenList.MasterLinen.Name} of {SelectedNewDepartment.Name} - in all Notes ? ")
            ) return;
            string changedNotes = ToString();

            foreach (var oldNote in SelectedNoteHeaders.OrderBy(x=> x.DeliveryDate))
            {
                var noteRows = NoteRows.Where(x => x.NoteHeaderId == oldNote.Id).ToList();
                if (!noteRows.Any(x =>
                    x.LinenListId == SelectedNoteRow.LinenListId && x.ServiceTypeId == SelectedNoteRow.ServiceTypeId)) continue;
                changedNotes += $"\n {oldNote.Name}";

                var oldNoteRow = noteRows.FirstOrDefault(x =>
                    x.LinenListId == SelectedNoteRow.LinenListId && x.ServiceTypeId == SelectedNoteRow.ServiceTypeId);

                // проверяем наличие наклодной с совпадашей датой. если нет но создаём новую

                NoteHeaderViewModel newNote;
                if (NoteHeaders.Any(x =>
                    x.DepartmentId == SelectedNewDepartment.Id &&
                    x.DeliveryDate.DayOfYear == oldNote.DeliveryDate.DayOfYear))
                {
                    newNote = NoteHeaders.FirstOrDefault(x =>
                        x.DepartmentId == SelectedNewDepartment.Id &&
                        x.DeliveryDate.DayOfYear == oldNote.DeliveryDate.DayOfYear);
                }
                else
                {
                    newNote = new NoteHeaderViewModel()
                    {
                        ClientId = oldNote.ClientId,
                        ClientReceivedDate = oldNote.ClientReceivedDate,
                        CollectionDate = oldNote.CollectionDate,
                        Comment = $"Note auto created to move {SelectedNoteRow.LinenName} from Note # {oldNote.Name}",
                        DeliveryDate = oldNote.DeliveryDate,
                        DeliveryTypeId = oldNote.DeliveryTypeId,
                        DepartmentId = SelectedNewDepartment.Id,
                        ExpressCharge = oldNote.ExpressCharge,
                        NoteStatus = oldNote.NoteStatus,
                        InvoiceId = oldNote.InvoiceId,
                        WeightPrice = oldNote.WeightPrice,
                    };

                    newNote.AcceptChanges();
                    await _dataService.AddOrUpdateAsync(newNote.OriginalObject);
                    NoteHeaders.Add(newNote);
                }

                // создаём новую запись белья

                var linen = LinenLists.FirstOrDefault(x => x.Id == SelectedLinenList.Id);
                var newNoteRow = new NoteRowViewModel()
                {
                    LinenListId = linen.Id,
                    LinenName = linen.MasterLinen.Name,
                    ClientReceivedQty = oldNoteRow.ClientReceivedQty,
                    PrimeDeliveredQty = oldNoteRow.PrimeDeliveredQty,
                    PrimeCollectionQty = oldNoteRow.PrimeCollectionQty,
                    Comment = oldNoteRow.Comment + $"# Moved From {oldNote.Name} #",
                    NoteHeaderId = newNote.OriginalObject.Id,
                    ServiceTypeId = SelectedServiceTypeId,
                    PriceUnit = linen.UnitId,
                    Weight = linen.Weight,
                };

                switch (newNoteRow.ServiceTypeId)
                {
                    case (int)ServiceTypeEnum.Laundry:
                        newNoteRow.Price = SelectedLinenList.Laundry;
                        break;

                    case (int)ServiceTypeEnum.DryCleaning:
                        newNoteRow.Price = SelectedLinenList.DryCleaning;
                        break;

                    case (int)ServiceTypeEnum.Pressing:
                        newNoteRow.Price = SelectedLinenList.Pressing;
                        break;
                }

                var oldNoteHeader = oldNote;
                oldNoteHeader.Comment += "." +SelectedNoteRow.LinenName + " Was moved to  note # " + newNote.OriginalObject.Id;
                oldNoteHeader.AcceptChanges();
                await _dataService.AddOrUpdateAsync(oldNoteHeader.OriginalObject);

                Save(newNoteRow);
                await _dataService.DeleteAsync(oldNoteRow.OriginalObject);
            }

            _dialogService.ShowInfoDialog($"{SelectedNoteRow.LinenName} was moved from \n {changedNotes}");

            if (_dialogService.ShowQuestionDialog($"Do you want to De-activate {SelectedNoteRow.LinenName}?"))
                DeActivatetOldNoteRowLinen();
            IsChanged = true;

            if (_dialogService.ShowQuestionDialog($"Do you want to close window?"))
                Close();
        }

        public async void DeActivatetOldNoteRowLinen()
        {
            if(SelectedNoteRow == null)return;

            var linen = LinenLists.FirstOrDefault(x => x.Id == SelectedNoteRow.LinenListId);
            if (linen == null) return;

            linen.Active = false;
            await _dataService.AddOrUpdateAsync(linen);
        }
    }
}
