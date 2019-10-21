using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class AddLinenViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;

        private ObservableCollection<LinenList> _linenList;
        private LinenList _selectedLinen;
        private NoteHeaderViewModel _selectedNoteHeader;
        private ObservableCollection<LinenList> _noteLinens;
        private NoteRowViewModel _newNotRow;

        public NoteRowViewModel NewNotRow
        {
            get => _newNotRow;
            set => Set(ref _newNotRow, value);
        }
        public ObservableCollection<LinenList> NoteLinens
        {
            get => _noteLinens;
            set => Set(ref _noteLinens, value);
        }
        public ObservableCollection<LinenList> LinenList
        {
            get => _linenList;
            set => Set(ref _linenList, value);
        }
        public LinenList SelectedLinen
        {
            get => _selectedLinen;
            set => Set(ref _selectedLinen, value);
        }

        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public Action<bool> CloseAction { get; set; }

        public ObservableCollection<LinenList> SortedLinensList => LinenList;
        public ObservableCollection<LinenList> SortedNoteLinens => NoteLinens;

        public RelayCommand AddCommand { get; }
        public RelayCommand CloseCommand { get; }

        public async Task InitializeAsync(NoteHeaderViewModel noteHeader, List<LinenList> linenList)
        {
            LinenList = linenList.ToObservableCollection();
            SelectedNoteHeader = noteHeader;
            NoteLinens = new ObservableCollection<LinenList>();
            NewNotRow = new NoteRowViewModel();

            RaisePropertyChanged(()=> SortedLinensList);
        }

        public AddLinenViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddCommand = new RelayCommand(Add, () => SelectedLinen != null);
            CloseCommand = new RelayCommand(Close);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedLinen))
            {
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public void Add()
        {
            if(SelectedLinen == null)
                return;

            NewNotRow = new NoteRowViewModel()
            {
                LinenListId = SelectedLinen.Id,
                LinenName = SelectedLinen.MasterLinen?.Name,
                NoteHeaderId = SelectedNoteHeader.Id,
                ServiceTypeId = 1,
            };

            CloseAction?.Invoke(true);
        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to close window ?"))
                return;

            CloseAction?.Invoke(false);
        }

        public NoteRowViewModel GetNotRows()
        {
            return NewNotRow;
        }
    }
}
