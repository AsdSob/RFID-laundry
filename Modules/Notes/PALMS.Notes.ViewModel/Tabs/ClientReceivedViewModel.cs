using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    public class ClientReceivedViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        private readonly NoteCommonMethods _commonMethods;
        private readonly IDialogService _dialogService;
        private Client _selectedClient;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private NoteHeaderViewModel _selectedNoteHeader;
        private readonly IDispatcher _dispatcher;
        private List<UnitViewModel> _serviceTypes;
        private List<UnitViewModel> _deliveryTypes;
        private List<Client> _clients;
        private List<DepartmentViewModel> _departments;

        public int NoteStatus => 4;
        public string Name => "Client Note";


        public List<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
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
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
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
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders =>
            NoteHeaders?.Where(x => x.ClientId == SelectedClient?.Id).OrderByDescending(x => x.OriginalObject.Id)
                .ToObservableCollection();

        public ObservableCollection<NoteRowViewModel> SortedNoteRows =>
            NoteRows?.Where(x => x.NoteHeaderId == SelectedNoteHeader?.Id).OrderBy(x => x.LinenName).ToObservableCollection();


        public RelayCommand SaveCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand CopyQuantityCommand { get; }
        public RelayCommand ToDeliveryNoteCommand { get; }


        public ClientReceivedViewModel(NoteCommonMethods commonMethods, IDialogService dialogService, IDispatcher dispatcher)
        {
            _commonMethods = commonMethods ?? throw new ArgumentNullException(nameof(commonMethods));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save, () => SelectedNoteHeader != null);
            ClearCommand = new RelayCommand(Clear, () => SelectedNoteHeader != null);
            PrintCommand = new RelayCommand(Print, () => SelectedNoteHeader != null);
            CopyQuantityCommand = new RelayCommand(CopyQuantity, () => SelectedNoteHeader != null);
            ToDeliveryNoteCommand = new RelayCommand(ToDeliveryNote, () => SelectedNoteHeader != null);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            _dispatcher.RunInMainThread(() =>
            {
                Clients = _commonMethods.Clients;
                ServiceTypes = _commonMethods.WashTypes;
                DeliveryTypes = _commonMethods.DeliveryTypes;
                Departments = _commonMethods.Departments;
                NoteRows = _commonMethods.NoteRows;
                NoteHeaders = _commonMethods.NoteHeaders?.Where(x => x.NoteStatus == (int)NoteStatusEnum.ClientNote)
                    .ToObservableCollection();

                SelectedClient = Clients.FirstOrDefault();

            });

            await Task.CompletedTask;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedNoteHeaders);
            }

            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                RaisePropertyChanged(() => SortedNoteRows);
                ClearCommand.RaiseCanExecuteChanged();
                PrintCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                CopyQuantityCommand.RaiseCanExecuteChanged();
                ToDeliveryNoteCommand.RaiseCanExecuteChanged();
            }
        }

        public void Clear()
        {
            if (!_dialogService.ShowQuestionDialog(" Do you want to remove all changes ?"))
                return;

            foreach (var noteRows in SortedNoteRows.Where(x=> x.NoteHeaderId == SelectedNoteHeader.Id))
            {
                noteRows.ClientReceivedQty = 0;
            }
        }

        public void Save()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to save note {SelectedNoteHeader.Name} ?"))
                return;

            AddPriceWeightToRows(SortedNoteRows);

            var noteRows = NoteRows.Where(x => x.NoteHeaderId == SelectedNoteHeader.Id).ToObservableCollection();

            SelectedNoteHeader.ClientReceivedDate = DateTime.Now;
            SelectedNoteHeader.NoteStatus = (int) NoteStatusEnum.PreInvoice;
            _commonMethods.Save(SelectedNoteHeader, noteRows);

            NoteHeaders?.Remove(SelectedNoteHeader);
            RaisePropertyChanged(() => SortedNoteHeaders);
            SelectedNoteHeader = null;
        }

        private void AddPriceWeightToRows(ObservableCollection<NoteRowViewModel> noteRows)
        {
            if (noteRows.Count == 0)
                return;

            foreach (var noteRow in noteRows)
            {
                var row = _commonMethods.AddNoteRowPriceWeight(noteRow);

                noteRow.Price = row.Price;
                noteRow.Weight = row.Weight;
            }
        }

        private void ToDeliveryNote()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to convert {SelectedNoteHeader.Name} back to Delivery Note?"))
                return;

            var noteHeader = SelectedNoteHeader;

            noteHeader.NoteStatus = (int) NoteStatusEnum.DeliveryNote;

            _commonMethods.SaveNoteHeader(noteHeader);

            NoteHeaders.Remove(noteHeader);
            RaisePropertyChanged(() => SortedNoteHeaders);
            SelectedNoteHeader = null;
        }

        private void Print()
        {

        }

        public void CopyQuantity()
        {
            SortedNoteRows?.ForEach(x=> x.ClientReceivedQty = x.PrimeDeliveredQty);
        }
    }

}
