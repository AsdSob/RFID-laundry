using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Settings.ViewModel.Dictionaries.Base;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Dictionaries
{
    public class DictionariesViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        private readonly IResolver _resolver;
        private ObservableCollection<DictionaryBaseViewModel> _items;
        private DictionaryBaseViewModel _selectedItem;

        public string Name => "Dictionaries";

        public bool HasChanges()
        {
            return Items.Any(x => x.CancelCommand.CanExecute(null));
        }

        public ObservableCollection<DictionaryBaseViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }
        public DictionaryBaseViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public DictionariesViewModel(IResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

            //_resolver = resolver != null ? resolver : new ArgumentNullException(nameof(resolver));
            //if (resolver != null)
            //    _resolver = resolver;
            //else _resolver = new ArgumentNullException(nameof(resolver));
            //In English, it means "If whatever is to the left is not null, use that, otherwise use what's to the right."
            //Note that you can use any number of these in sequence.The following statement will assign the first non-null Answer# to Answer (if all Answers are null then the Answer is null):
            //string Answer = Answer1 ?? Answer2 ?? Answer3 ?? Answer4;

            AddCommand = new RelayCommand(Add, CanAdded);
            SaveCommand = new RelayCommand(Save, CanSave);
            RemoveCommand = new RelayCommand(Remove, CanRemove);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            Items = new ObservableCollection<DictionaryBaseViewModel>
            {
                _resolver.Resolve<FamilyLinenDictionaryViewModel>(),
                _resolver.Resolve<GroupLinenDictionaryViewModel>(),
                _resolver.Resolve<TypeLinenDictionaryViewModel>(),
                _resolver.Resolve<TrackingTypeDictionaryViewModel>(),
            };

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem))
            {
                AddCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();

                foreach (var item in Items)
                {
                    item.RemoveCommand.CanExecuteChanged -= RemoveCommandOnCanExecuteChanged;
                    item.SaveCommand.CanExecuteChanged -= SaveCommandOnCanExecuteChanged;
                    item.CancelCommand.CanExecuteChanged -= CancelCommandOnCanExecuteChanged;
                }

                if (SelectedItem != null)
                {
                    SelectedItem.RemoveCommand.CanExecuteChanged += RemoveCommandOnCanExecuteChanged;
                    SelectedItem.SaveCommand.CanExecuteChanged += SaveCommandOnCanExecuteChanged;
                    SelectedItem.CancelCommand.CanExecuteChanged += CancelCommandOnCanExecuteChanged;
                }
            }
        }

        private void RemoveCommandOnCanExecuteChanged(object sender, EventArgs eventArgs) => RemoveCommand?.RaiseCanExecuteChanged();
        private void SaveCommandOnCanExecuteChanged(object sender, EventArgs eventArgs) => SaveCommand?.RaiseCanExecuteChanged();
        private void CancelCommandOnCanExecuteChanged(object sender, EventArgs eventArgs) => CancelCommand?.RaiseCanExecuteChanged();

        private void Save()
        {
            SelectedItem.SaveCommand?.Execute(null);
        }

        private void Add()
        {
            SelectedItem.AddCommand?.Execute(null);
        }

        private void Remove()
        {
            SelectedItem.RemoveCommand?.Execute(null);
        }

        private void Cancel()
        {
            SelectedItem.CancelCommand?.Execute(null);
        }

        public async Task InitializeAsync()
        {
            await Task.WhenAll(Items.Select(x => x.InitializeAsync())).ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    var exception = x.Exception?.Flatten();
                    throw new Exception($"Initialize error: {exception?.Message}", exception);
                }
            });
        }

        protected bool CanAdded()
        {
            return SelectedItem?.IsEnumDictionary == false;
        }

        protected bool CanRemove()
        {
            return SelectedItem != null &&
                   SelectedItem.IsEnumDictionary == false &&
                   SelectedItem.RemoveCommand?.CanExecute(null) == true;
        }

        protected bool CanSave()
        {
            return SelectedItem != null &&
                   SelectedItem.IsEnumDictionary == false &&
                   SelectedItem.SaveCommand?.CanExecute(null) == true;
        }

        protected bool CanCancel()
        {
            return SelectedItem != null &&
                   SelectedItem.IsEnumDictionary == false &&
                   SelectedItem.CancelCommand?.CanExecute(null) == true;
        }
    }
}