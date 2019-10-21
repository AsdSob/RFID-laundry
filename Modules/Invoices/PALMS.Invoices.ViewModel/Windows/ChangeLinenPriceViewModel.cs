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
    public class ChangeLinenPriceViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;

        private ObservableCollection<LinenListViewModel> _linenLists;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private ObservableCollection<NoteRowViewModel> _noteRows;

        private Client _client;
        private ClientInfoViewModel _clientInfo;
        private ObservableCollection<Department> _departments;
        private Department _selectedDepartment;

        public bool IsChanged;
        private List<UnitViewModel> _linenListUnit;

        public List<UnitViewModel> LinenListUnit
        {
            get => _linenListUnit;
            set => Set(ref _linenListUnit, value);
        }
        public ClientInfoViewModel ClientInfo
        {
            get => _clientInfo;
            set => Set(ref _clientInfo, value);
        }
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public Client Client
        {
            get => _client;
            set => Set(ref _client, value);
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
        public ObservableCollection<LinenListViewModel> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }

        public int? SelectedInvoiceId { get; set; }

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders =>
            NoteHeaders.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

        public ObservableCollection<LinenListViewModel> SortedLinenLists => SortLinenList();

        public Action<bool> CloseAction { get; set; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand ApplyForAllNotesCommand { get; }

        public ChangeLinenPriceViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentException(nameof(dispatcher));

            CloseCommand = new RelayCommand(Close);
            ApplyForAllNotesCommand = new RelayCommand(ChangePriceInNotes);
        }

        public async Task InitializeAsync(Client client, int? invoiceId)
        {
            if(client == null) return;

            SelectedInvoiceId = invoiceId;
            Client = client;
            IsChanged = false;

            _dialogService.ShowBusy();

            try
            {
                if (client.ClientInfo == null)
                {
                    var info = await _dataService.GetAsync<ClientInfo>(x => x.Client == client);
                    var clientInfo = info.FirstOrDefault(x => x.Client == client);
                    _dispatcher.RunInMainThread(() => ClientInfo = new ClientInfoViewModel(clientInfo));
                }
                else
                    ClientInfo = new ClientInfoViewModel(client.ClientInfo);

                var noteHeader = await _dataService.GetAsync<NoteHeader>(x => x.InvoiceId == SelectedInvoiceId && x.ClientId == Client.Id);
                var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());

                var noteRows = new ObservableCollection<NoteRowViewModel>();
                foreach (var note in NoteHeaders)
                {
                    var noteRow = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    noteRows.AddRange(noteRow.Select(x => new NoteRowViewModel(x)));
                }
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());

                var linenList = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                var linenLists = linenList.Where(x => x.ClientId == Client.Id).Select(x => new LinenListViewModel(x));
                _dispatcher.RunInMainThread(() => LinenLists = linenLists.ToObservableCollection());

                var department = await _dataService.GetAsync<Department>();
                var departments = department.Where(x => x.ClientId == Client.Id);
                _dispatcher.RunInMainThread(() => Departments = departments.ToObservableCollection());

                LinenListUnit = EnumExtentions.GetValues<LinenUnitEnum>();
                PropertyChanged += OnPropertyChanged;
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedLinenLists);
                RaisePropertyChanged(() => SortedNoteHeaders);
            }
        }

        private ObservableCollection<LinenListViewModel> SortLinenList()
        {
            if (SelectedDepartment == null) return null;
            var linenList =new ObservableCollection<LinenListViewModel>();

            foreach (var note in NoteHeaders.Where(x => x.DepartmentId == SelectedDepartment.Id))
            {
                foreach (var noteRow in NoteRows.Where(x => x.NoteHeaderId == note.Id))
                {
                    var hasSameLinen = linenList.Any(x=> x.Id == noteRow.LinenListId);

                    if (hasSameLinen) continue;
                    {
                        var linen = LinenLists.FirstOrDefault(x => x.Id == noteRow.LinenListId);
                        linenList.Add(linen);
                    }
                }
            }
            return linenList;
        }

        private async void Save()
        {
            if (ClientInfo.HasChanges())
            {
                ClientInfo.AcceptChanges(Client);
                await _dataService.AddOrUpdateAsync(ClientInfo.OriginalObject);
                IsChanged = true;
            }

            if (LinenLists.Any(x => x.HasChanges()))
            {
                var linenLists = LinenLists.Where(x => x.HasChanges()).ToList();
                linenLists.ForEach(x => x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(linenLists.Select(x => x.OriginalObject));
                IsChanged = true;
            }

            if (SortedNoteHeaders.Any(x => x.HasChanges()))
            {
                var noteHeaders = SortedNoteHeaders.Where(x => x.HasChanges()).ToList();
                noteHeaders.ForEach(x => x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(noteHeaders.Select(x => x.OriginalObject));
                IsChanged = true;
            }

            if (NoteRows.Any(x => x.HasChanges()))
            {
                var noteRows = NoteRows.Where(x => x.HasChanges()).ToList();
                noteRows.ForEach(x => x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(noteRows.Select(x => x.OriginalObject));
                IsChanged = true;
            }

        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                return;
            CloseAction?.Invoke(IsChanged);
        }

        private void ChangePriceInNotes()
        {
            _dialogService.ShowWarnigDialog(
                "Changing price for notes can take a time.  DO NOT CLOSE PROGRAM until getting confirmation!");

            if (!_dialogService.ShowQuestionDialog("Do you want to apply changes to all non-Invoiced Notes? "))
                return;

            var noteHeaders = SortedNoteHeaders;
            var noteRows = new ObservableCollection<NoteRowViewModel>();

            foreach (var noteHeader in noteHeaders)
            {
                noteHeader.WeightPrice = ClientInfo.WeightPrice;
                noteHeader.ExpressCharge = SelectedDepartment.Express;
                noteRows.AddRange(NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id));
            }

            ChangeNoteRows(noteRows);
            Save();

            _dialogService.ShowInfoDialog("All Changes was implemented");
        }
        private void ChangeNoteRows(ObservableCollection<NoteRowViewModel> noteRows)
        {
            if (noteRows.Count == 0)
                return;

            foreach (var linenList in LinenLists)
            {
                foreach (var noteRow in noteRows.Where(x => x.LinenListId == linenList.Id))
                {
                    noteRow.PriceUnit = linenList.UnitTypeId;

                    switch (noteRow.ServiceTypeId)
                    {
                        case (int)ServiceTypeEnum.Laundry:
                            noteRow.Price = linenList.Laundry;
                            break;

                        case (int)ServiceTypeEnum.DryCleaning:
                            noteRow.Price = linenList.DryCleaning;
                            break;

                        case (int)ServiceTypeEnum.Pressing:
                            noteRow.Price = linenList.Pressing;
                            break;
                    }
                }
            }
        }
    }
}
