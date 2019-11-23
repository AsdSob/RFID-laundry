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
using PALMS.Settings.ViewModel.NoteLinenReplacement.EntityModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.NoteLinenReplacement
{
    public class NoteLinenReplacementViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public string Name => "Note Linen";
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<LinenListViewModel> _linenLists;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private NoteHeaderViewModel _selectedNoteHeader;
        private NoteRowViewModel _selectedNoteRow;
        private List<UnitViewModel> _noteStatus;
        private LinenListViewModel _selectedLinenList;


        public LinenListViewModel SelectedLinenList
        {
            get => _selectedLinenList;
            set => Set(ref _selectedLinenList, value);
        }
        public List<UnitViewModel> NoteStatus
        {
            get => _noteStatus;
            set => Set(ref _noteStatus, value);
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

        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRow();
        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => SortNoteHeaders();
        public ObservableCollection<LinenListViewModel> SortedLinenList => SortLinenList();

        public RelayCommand SaveCommand { get; }
        public RelayCommand DeleteNoteCommand { get; }
        public RelayCommand DeleteRowCommand { get; }

        public NoteLinenReplacementViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save, HasChanges);
            DeleteNoteCommand = new RelayCommand(DeleteNoteHeader, () => SelectedNoteHeader != null);
            DeleteRowCommand = new RelayCommand(DeleteNoteRow, ()=> SelectedNoteRow != null);

            PropertyChanged += OnPropertyChanged;
        }

        public bool HasChanges()
        {
            if (NoteHeaders == null || NoteRows == null)
                return false;

            return NoteHeaders.Any(x => x.HasChanges()) || NoteRows.Any(x => x.HasChanges());
        }

        public async Task InitializeAsync()
        {
            var linenList = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
            var linenLists = linenList.Select(x=> new LinenListViewModel(x));
            _dispatcher.RunInMainThread(() => LinenLists = linenLists.ToObservableCollection());

            var noteHeader = await _dataService.GetAsync<NoteHeader>(x=> x.Client);
            var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x)).Where(x => x.InvoiceId == null);
            _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());

            var noteRow = await _dataService.GetAsync<NoteRow>(x=> x.LinenList);
            var noteRows = noteRow.Select(x=> new NoteRowViewModel(x));
            _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());

            NoteRows.ForEach(x=> x.LinenName = LinenLists.FirstOrDefault(y=> y.Id == x.LinenListId)?.OriginalObject.MasterLinen.Name);

            NoteStatus = EnumExtentions.GetValues<NoteStatusEnum>().ToList();
            RaisePropertyChanged(()=> SortedNoteHeaders);
            RaisePropertyChanged(()=> SortedNoteRows);
            RaisePropertyChanged(()=> SortedLinenList);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PrimeInfo))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                DeleteNoteCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                DeleteRowCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<LinenListViewModel> SortLinenList()
        {
            var linenLists = new ObservableCollection<LinenListViewModel>();

            linenLists = LinenLists.Where(x => x.OriginalObject.MasterLinen == null).ToObservableCollection();

            return linenLists;
        }

        public ObservableCollection<NoteRowViewModel> SortNoteRow()
        {
            var noteRows = new ObservableCollection<NoteRowViewModel>();

            foreach (var noteRow in NoteRows)
            {
                if (noteRow.LinenName == null || noteRow.OriginalObject.LinenList == null)
                {
                    noteRows.Add(noteRow);
                }
            }

            return noteRows;
        }

        public ObservableCollection<NoteHeaderViewModel> SortNoteHeaders()
        {
            var noteHeaders = new ObservableCollection<NoteHeaderViewModel>();

            foreach (var noteHeader in NoteHeaders)
            {
                var noteRows = NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id);

                if (noteRows.Count() > 0) continue;
                noteHeaders.Add(noteHeader);
            }

            return noteHeaders;
        }

        public async void Save()
        {
            if (!_dialogService.ShowQuestionDialog(" Do you want to Save all changes ?"))
                return;

            var noteRows = NoteRows.Where(x => x.HasChanges()).ToObservableCollection();
            var noteHeaders = NoteHeaders.Where(x => x.HasChanges()).ToObservableCollection();

            if (noteRows.Any())
            {
                noteRows.ForEach(x => x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(noteRows.Select(x => x.OriginalObject));
            }

            if (noteHeaders.Any())
            {
                noteHeaders.ForEach(x=> x.AcceptChanges());
                await _dataService.AddOrUpdateAsync(noteHeaders.Select(x => x.OriginalObject));
            }
        }

        public async void DeleteNoteRow()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete Linen ?"))
                return;

            await _dataService.DeleteAsync(SelectedNoteRow.OriginalObject);
        }

        public async void DeleteNoteHeader()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete {SelectedNoteHeader.Name} ?"))
                return;

            await _dataService.DeleteAsync(SelectedNoteHeader.OriginalObject);
        }
    }
}
