using GalaSoft.MvvmLight;
using PALMS.Data.Objects.LinenModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using PALMS.Data.Objects.NoteModel;

namespace PALMS.MasterLinens.ViewModel
{
    public class TypeLinenTabViewModel : ViewModelBase, IInitializationAsync
    {

        private readonly LinenType _linenType;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IResolver _resolverService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<MasterLinenViewModel> _items;
        private MasterLinenViewModel _selectedItem;
        private ObservableCollection<FamilyLinen> _familyLinenList;
        private ObservableCollection<LinenType> _typeLinenList;
        private ObservableCollection<GroupLinen> _groupLinenList;
        private bool _isValid;
        private int _id;
        private bool? _hasChanges;

        public string Name => _linenType?.Name;
        private List<NoteRow> _noteRows;

        public ObservableCollection<FamilyLinen> FamilyLinenList
        {
            get => _familyLinenList;
            set => Set(ref _familyLinenList, value);
        }

        public ObservableCollection<GroupLinen> GroupLinenList
        {
            get => _groupLinenList;
            set => Set(ref _groupLinenList, value);
        }

        public ObservableCollection<LinenType> TypeLinenList
        {
            get => _typeLinenList;
            set => Set(ref _typeLinenList, value);
        }

        public ObservableCollection<MasterLinenViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public MasterLinenViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }

        public Action CancelEditAction { get; set; }

        public RelayCommand AddCommand { get; }

        public RelayCommand SaveCommand { get; }

        public RelayCommand RemoveCommand { get; }

        public RelayCommand CancelCommand { get; }

        public TypeLinenTabViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService, IResolver resolver)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));


            AddCommand = new RelayCommand(Add);
            SaveCommand = new RelayCommand(Save, () => IsValid && HasChanges());
            RemoveCommand = new RelayCommand(Remove, () => SelectedItem != null);
            CancelCommand = new RelayCommand(Cancel, HasChanges);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            var masterLinens = await _dataService.GetAsync<MasterLinen>();
            var items = masterLinens.Select(x => new MasterLinenViewModel(x));
            _dispatcher.RunInMainThread(() => Items = items.ToObservableCollection());
            Items.ForEach(x => x.PropertyChanged += ItemOnPropertyChanged);
            Items.CollectionChanged += ItemsOnCollectionChanged;

            var familyLinen = await _dataService.GetAsync<FamilyLinen>();
            _dispatcher.RunInMainThread(() => FamilyLinenList = familyLinen.ToObservableCollection());

            var typeLinen = await _dataService.GetAsync<LinenType>();
            _dispatcher.RunInMainThread(() => TypeLinenList = typeLinen.ToObservableCollection());

            var groupLinen = await _dataService.GetAsync<GroupLinen>();
            _dispatcher.RunInMainThread(() => GroupLinenList = groupLinen.ToObservableCollection());

            var noteRow = await _dataService.GetAsync<NoteRow>(x => x.LinenList);
            _dispatcher.RunInMainThread(() => NoteRows = noteRow.ToList());
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<MasterLinenViewModel>())
                {
                    item.NameUniqueValidationFunc = NameUniqueValidationFunc;
                    item.PropertyChanged += ItemOnPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<MasterLinenViewModel>())
                {
                    item.NameUniqueValidationFunc = null;
                    item.PropertyChanged -= ItemOnPropertyChanged;
                }
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _hasChanges = null;

            SaveCommand?.RaiseCanExecuteChanged();
            CancelCommand?.RaiseCanExecuteChanged();
        }

        private bool HasChanges()
        {
            return _hasChanges ?? (_hasChanges = Items?.Any(x => x.HasChanges()) == true).Value;
        }

        private bool NameUniqueValidationFunc(MasterLinenViewModel item, string propertyName)
        {

            if (item == null) throw new ArgumentNullException(nameof(item));

            var properyInfo = typeof(MasterLinenViewModel).GetProperty(propertyName);

            if (properyInfo == null) throw new ArgumentNullException($"{propertyName} not found");

            return !Items.Any(x => Equals(properyInfo.GetValue(x), properyInfo.GetValue(item)) && !Equals(x, item));
        }

        protected async void Save()
        {
            var changedItems = GetChangedItems();
            if (changedItems.Length == 0) return;

            if (!_dialogService.ShowQuestionDialog("Do you want to save new Linens ?")) return;

            changedItems.ForEach(x => x.AcceptChanges());

            await _dataService.AddOrUpdateAsync(changedItems.Select(x => x.OriginalObject));

            if (!_dialogService.ShowInfoDialog("Items Saved")) { }
        }

        protected async void Remove()
        {

            if (NoteRows != null || NoteRows.Count > 0)
            {
                if (NoteRows.Any(x => x.LinenList.MasterLinenId == SelectedItem.Id))
                {
                    _dialogService.ShowInfoDialog($"{SelectedItem.Name}  is used in Notes and can not be removed");
                    return;
                }
            }

            if (_dialogService.ShowQuestionDialog($"Do you want remove '{SelectedItem.Name}'?"))
            {
                CancelEditAction?.Invoke();

                var entity = SelectedItem.OriginalObject;

                Items.Remove(SelectedItem);

                if (!entity.IsNew)
                    await _dataService.DeleteAsync(entity);
            }
        }

        protected void Cancel()
        {
            var changedItems = GetChangedItems();
            
                if (changedItems.Length > 0 && _dialogService.ShowQuestionDialog("Do you want cancel all changes?"))
            {
                CancelEditAction?.Invoke();

                changedItems.ForEach(x =>
                {
                    if (x.OriginalObject.IsNew)
                        Items.Remove(x);
                    else
                        x.Reset();
                });
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem))
                RemoveCommand.RaiseCanExecuteChanged();
            else if (e.PropertyName == nameof(IsValid))
                SaveCommand.RaiseCanExecuteChanged();
        }

        private void Add()
        {
            Items.Add(new MasterLinenViewModel());
        }

        private MasterLinenViewModel[] GetChangedItems()
        {
            return Items.Where(x => x.HasChanges() || x.IsChanged).ToArray();
        }
    }
}