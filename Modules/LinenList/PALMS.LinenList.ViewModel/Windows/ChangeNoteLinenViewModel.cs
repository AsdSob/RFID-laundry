using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.LinenList.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using LinenModel = PALMS.Data.Objects.LinenModel.LinenList;

namespace PALMS.LinenList.ViewModel.Windows
{
    public class ChangeNoteLinenViewModel : ViewModelBase, IWindowDialogViewModel
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
        private List<LinenModel> _linenLists;
        private LinenModel _selectedLinenList;
        private int _selectedServiceTypeId;
        private List<UnitViewModel> _serviceTypes;
        private Client _selectedClient;
        private NoteRowViewModel _selectedNoteRow;

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
        public LinenModel SelectedLinenList
        {
            get => _selectedLinenList;
            set => Set(ref _selectedLinenList, value);
        }
        public List<LinenModel> LinenLists
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

        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRows();
        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => NoteHeaders
            ?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
        public List<Department> SortedDepartments => Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToList();
        public List<LinenModel> SortedLinenLists =>
            LinenLists?.Where(x => x.DepartmentId == SelectedNewDepartment?.Id).OrderBy(x => x.MasterLinen.Name)
                .ToList();

        public RelayCommand CloseCommand { get; }
        public RelayCommand ChangeLinenCommand { get; }

        public ChangeNoteLinenViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            CloseCommand = new RelayCommand(Close);
            ChangeLinenCommand = new RelayCommand(ChangeLinen);
        }

        public async Task InitializeAsync(Client client)
        {
            if (client == null) return;

            _dialogService.ShowBusy();

            try
            {
                SelectedClient = client;

                var departments = await _dataService.GetAsync<Department>(x => x.ClientId == SelectedClient.Id);
                _dispatcher.RunInMainThread(() => Departments = departments.ToList());

                var linenList = await _dataService.GetAsync<LinenModel>(x => x.MasterLinen);
                _dispatcher.RunInMainThread(() => LinenLists = linenList.ToList());

                var notes = await _dataService.GetAsync<NoteHeader>(x => x.InvoiceId == null && x.ClientId == SelectedClient.Id);
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
                    row.NumberOfUsage = noteRows.Count(x => x.LinenListId == row.LinenListId);
                    var linen = LinenLists.FirstOrDefault(x => x.Id == row.LinenListId);
                    row.LinenName = linen?.MasterLinen.Name;

                    if (linen == null) continue;
                    row.MasterLinenDelete = linen.MasterLinen.DeletedDate.HasValue;
                }

                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());

                ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();

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

        private void ChangeLinen()
        {
            if (SelectedNoteRow == null || SelectedLinenList == null) return;

            if (NoteHeaders.Count <= 0) return;

            if (SelectedDepartment.Id == SelectedNewDepartment.Id)
                ChangeToSameDep();
            else
            {
                ChangeToOtherDepartment();
            }
        }

        private void ChangeToSameDep()
        {
            var selectedRow = SelectedNoteRow;
            var selectedLinen = SelectedLinenList;
            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to change - {selectedRow.LinenName} - of {SelectedDepartment.Name} - to - {selectedLinen.MasterLinen.Name} - in all Notes ? ")
            ) return;

            if (!NoteRows.Any(x =>
                x.LinenListId == selectedRow.LinenListId &&
                x.ServiceTypeId == selectedRow.ServiceTypeId)) return;

            var oldNoteRows = NoteRows.Where(x =>
                x.LinenListId == selectedRow.LinenListId &&
                x.ServiceTypeId == selectedRow.ServiceTypeId).ToList();

            foreach (var oldNoteRow in oldNoteRows)
            {
                var newNoteRow = new NoteRowViewModel()
                {
                    ServiceTypeId = SelectedServiceTypeId,
                    NoteHeaderId = oldNoteRow.NoteHeaderId,
                    LinenName = selectedLinen.MasterLinen.Name,
                    LinenListId = selectedLinen.Id,
                    Weight = selectedLinen.Weight,
                    Comment = oldNoteRow.Comment,
                    PrimeCollectedQty = oldNoteRow.PrimeCollectedQty,
                    PrimeDeliveredQty = oldNoteRow.PrimeDeliveredQty,
                    ClientReceivedQty = oldNoteRow.ClientReceivedQty,
                    Price = GetLinenPrice(SelectedServiceTypeId),
                };

                Save(newNoteRow);
                NoteRows.Remove(oldNoteRow);
            }

            oldNoteRows.ForEach(x=> _dataService.DeleteAsync(x.OriginalObject));
            RaisePropertyChanged(() => SortedNoteRows);
            DeactivateOldNoteRowLinen(selectedRow);
            IsChanged = true;
        }

        private async void ChangeToOtherDepartment()
        {
            _dialogService.ShowWarnigDialog("After Applying this changes its not possible to recover it");

            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to move All - {SelectedNoteRow.LinenName} s - of {SelectedDepartment.Name} - to - {SelectedLinenList.MasterLinen.Name} of {SelectedNewDepartment.Name} - in all Notes ? ")
            ) return;

            var oldNoteRows = NoteRows.Where(x =>
                x.LinenListId == SelectedNoteRow.LinenListId && x.ServiceTypeId == SelectedNoteRow.ServiceTypeId);
            var noteRowsToRemove = new List<NoteRowViewModel>();

            foreach (var oldNoteRow in oldNoteRows)
            {
                var oldNote = NoteHeaders.FirstOrDefault(x => x.Id == oldNoteRow.NoteHeaderId);

                var newNote = NoteHeaders.FirstOrDefault(x =>
                    x.DepartmentId == SelectedNewDepartment.Id &&
                    x.DeliveryDate.DayOfYear == oldNote.DeliveryDate.DayOfYear &&
                    x.CollectionDate.DayOfYear == oldNote.CollectionDate.DayOfYear);

                if (newNote ==null)
                {
                    newNote = new NoteHeaderViewModel()
                    {
                        ClientId = oldNote.ClientId,
                        ClientReceivedDate = oldNote.ClientReceivedDate,
                        CollectionDate = oldNote.CollectionDate,
                        Comment = $"({oldNote.Name}) ",
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

                    newNote.Name = $"PAL {newNote.OriginalObject.Id:D6}/{DateTime.Now:yy}";

                    newNote.AcceptChanges();
                    await _dataService.AddOrUpdateAsync(newNote.OriginalObject);

                    NoteHeaders.Add(newNote);
                }

                var linen = LinenLists.FirstOrDefault(x => x.Id == SelectedLinenList.Id);
                var newNoteRow = new NoteRowViewModel()
                {
                    LinenListId = linen.Id,
                    LinenName = linen.MasterLinen.Name,
                    ClientReceivedQty = oldNoteRow.ClientReceivedQty,
                    PrimeDeliveredQty = oldNoteRow.PrimeDeliveredQty,
                    PrimeCollectedQty = oldNoteRow.PrimeCollectedQty,
                    Comment = $"#{oldNote.Name}#" + oldNoteRow.Comment,
                    NoteHeaderId = newNote.OriginalObject.Id,
                    ServiceTypeId = SelectedServiceTypeId,
                    PriceUnit = linen.UnitId,
                    Weight = linen.Weight,
                    Price = GetLinenPrice(SelectedServiceTypeId),
                };

                oldNote.Comment += ". " + SelectedNoteRow.LinenName + newNote.OriginalObject.Id;
                oldNote.AcceptChanges();
                await _dataService.AddOrUpdateAsync(oldNote.OriginalObject);

                Save(newNoteRow);
                noteRowsToRemove.Add(oldNoteRow);
            }
            DeactivateOldNoteRowLinen(SelectedNoteRow);

            foreach (var noteRow in noteRowsToRemove)
            {
                NoteRows.Remove(noteRow);
                await _dataService.DeleteAsync(noteRow.OriginalObject);
            }
            RaisePropertyChanged(() => SortedNoteRows);
            IsChanged = true;
        }

        private double GetLinenPrice(int serviceTypeId)
        {
            var price = 0.0;
            switch (serviceTypeId)
            {
                case (int)ServiceTypeEnum.Laundry:
                    price = SelectedLinenList.Laundry;
                    break;

                case (int)ServiceTypeEnum.DryCleaning:
                    price = SelectedLinenList.DryCleaning;
                    break;

                case (int)ServiceTypeEnum.Pressing:
                    price = SelectedLinenList.Pressing;
                    break;
            }
            return price;
        }

        public async void DeactivateOldNoteRowLinen(NoteRowViewModel noteRow)
        {
            if (noteRow == null) return;

            if (!_dialogService.ShowQuestionDialog(
                $" {noteRow.LinenName} was moved \n Do you want to De-activate {noteRow.LinenName}?")) return;


            var linen = LinenLists.FirstOrDefault(x => x.Id == noteRow.LinenListId);
            if (linen == null) return;

            linen.Active = false;
            await _dataService.AddOrUpdateAsync(linen);
        }
    }
}
