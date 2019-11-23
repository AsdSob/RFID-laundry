using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.EntityModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Dictionaries;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Dictionaries.Base
{
    public abstract class DictionaryViewModel<TEntity, TViewModel> : DictionaryBaseViewModel, IDictionaryViewModel
        where TEntity : NameEntity, new()
        where TViewModel : DictionaryItemViewModel<TEntity>, new()
    {
        private readonly IDialogService _dialogService;
        protected readonly IDispatcher Dispatcher;
        protected readonly IDataService DataService;

        private ObservableCollection<TViewModel> _items;
        private TViewModel _selectedItem;
        private bool _hasChanges;
        private bool _isValid;

        /// <summary>
        /// Коллекцыя записей справочника
        /// </summary>
        public ObservableCollection<TViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }

        public TViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set => Set(ref _hasChanges, value);
        }

        public Action CancelEditAction { get; set; }

        public override async Task InitializeAsync()
        {
            var items = await DataService.GetAsync<TEntity>();

            var viewModels = items.Select(GetItem).ToObservableCollection();

            viewModels.ForEach(x => x.PropertyChanged += ItemOnPropertyChanged);

            Dispatcher.RunInMainThread(() => Items = viewModels);
        }

        protected DictionaryViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        protected DictionaryViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService) : this()
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            DataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        protected abstract TViewModel GetItem(TEntity entity);

        protected override void Add()
        {
            Items.Add(GetItem(new TEntity()));
        }

        protected override async void Save()
        {
            var changedItems = GetChangedItems();
            if (changedItems.Length == 0) return;

            changedItems.ForEach(x => x.AcceptChanges());
            var newItems = changedItems.Where(x => x.IsNew).ToList();

            await DataService.AddOrUpdateAsync(changedItems.Select(x => x.OriginalObject));

            newItems.ForEach(x => x.Reset());

            HasChanges = false;
        }

        protected override bool CanSave()
        {
            return HasChanges && IsValid;
        }

        protected override bool CanCancel()
        {
            return HasChanges;
        }

        protected override bool CanRemove()
        {
            return SelectedItem != null;
        }

        protected override async void Remove()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want remove '{SelectedItem.Name}'?"))
            {
                var entity = SelectedItem.OriginalObject;

                Items.Remove(SelectedItem);

                if (!entity.IsNew)
                    await DataService.DeleteAsync(entity);
            }
        }

        protected override void Cancel()
        {
            var changedItems = GetChangedItems();

            if (changedItems.Length > 0 && _dialogService.ShowQuestionDialog("Do you want cancel all changes?"))
            {
                CancelEditAction?.Invoke();

                changedItems.ForEach(x => x.Reset());

                foreach (var item in changedItems.Where(x => x.IsNew))
                {
                    Items.Remove(item);
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Items))
            {
                Items.CollectionChanged += ItemsOnCollectionChanged;
            }
            else if (e.PropertyName == nameof(SelectedItem))
            {
                RemoveCommand?.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(HasChanges))
            {
                SaveCommand?.RaiseCanExecuteChanged();
                CancelCommand?.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(IsValid))
            {
                SaveCommand?.RaiseCanExecuteChanged();
            }
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<ViewModelBase>())
                {
                    item.PropertyChanged += ItemOnPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<ViewModelBase>())
                {
                    item.PropertyChanged -= ItemOnPropertyChanged;
                }
            }

            UpdateHasChanges();
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsSignificalPropertyName(e.PropertyName))
            {
                UpdateHasChanges();
            }
        }

        protected virtual bool IsSignificalPropertyName(string propertyName)
        {
            return propertyName == nameof(Name);
        }

        private void UpdateHasChanges()
        {
            HasChanges = Items?.Any(x => x.HasChanges) == true;
        }

        private TViewModel[] GetChangedItems()
        {
            return Items.Where(x => x.HasChanges || x.IsChanged).ToArray();
        }
    }
}