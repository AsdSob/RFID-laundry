using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.NoteModel;
using PALMS.Settings.ViewModel.NoteLinenReplacement.EntityModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Settings.ViewModel.NoteSearchLinen.Windows
{
    public class NoteEditViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private NoteHeaderViewModel _selectedNoteHeader;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private NoteRowViewModel _selectedNoteRow;
        private List<UnitViewModel> _deliveryTypes;
        private List<UnitViewModel> _serviceTypes;
        private bool _isChanged;

        public bool IsChanged
        {
            get => _isChanged;
            set => Set(ref _isChanged, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand CancelCommand { get; }

        public NoteEditViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            CancelCommand = new RelayCommand(Cancel);
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

            if (e.PropertyName == nameof(item.ServiceTypeId))
            {
                switch (item.ServiceTypeId)
                {
                    case (int) ServiceTypeEnum.Laundry:
                        item.Price = item.OriginalObject.LinenList.Laundry;
                        break;

                    case (int) ServiceTypeEnum.DryCleaning:
                        item.Price = item.OriginalObject.LinenList.DryCleaning;
                        break;

                    case (int) ServiceTypeEnum.Pressing:
                        item.Price = item.OriginalObject.LinenList.Pressing;
                        break;
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

        public async Task InitializeAsync(NoteHeaderViewModel noteHeader)
        {
            SelectedNoteHeader = noteHeader;

            var noteRow = await _dataService.GetAsync<NoteRow>(x=> x.LinenList.MasterLinen);
            var noteRows = noteRow.Select(x=> new NoteRowViewModel(x));
            _dispatcher.RunInMainThread(() => NoteRows = noteRows.Where(x=> x.NoteHeaderId == SelectedNoteHeader.Id).ToObservableCollection());

            NoteRows.ForEach(SubscribeItem);
            NoteRows.CollectionChanged += NoteRowsCollectionChanged;

            IsChanged = false;

            DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
            ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();

        }

        public async void Save()
        {
            if (!HasChanges()) return;

            if( !_dialogService.ShowQuestionDialog($"Do you want to save all changes ? "))
                return;

            SelectedNoteHeader.AcceptChanges();
            await _dataService.AddOrUpdateAsync(SelectedNoteHeader.OriginalObject);

            NoteRows.ForEach(x=> x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(NoteRows.Select(x=> x.OriginalObject));

            IsChanged = true;
            CloseAction?.Invoke(true);
        }

        public void Close()
        {
            if(_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                CloseAction?.Invoke(true);
        }

        public void Cancel()
        {
            if (!HasChanges() ) return;

            if( !_dialogService.ShowQuestionDialog($"Do you want to cancel all changes? ")) return;

            SelectedNoteHeader.Reset();
            NoteRows.ForEach(x => x.Reset());
        }

        public bool HasChanges()
        {
            return (SelectedNoteHeader.HasChanges() || NoteRows.Any(x => x.HasChanges()));
        }

        public NoteHeaderViewModel GetNoteHeader()
        {
            var noteHeader = SelectedNoteHeader;

            return noteHeader;
        }

        public ObservableCollection<NoteRowViewModel> GetNoteRows()
        {
            var noteRows = NoteRows;

            return noteRows;
        }
    }
}
