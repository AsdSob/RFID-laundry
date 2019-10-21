using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    public class TabsViewModel : ViewModelBase,  IInitializationAsync
    {
        private ObservableCollection<TabViewModel> _items;
        readonly NoteCommonMethods _noteCommonMethods;
        private DeliveryNoteViewModel deliveryNoteViewModel;
        private object _selectedItem;
        private bool _isInitializing;

        public Object SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        public ObservableCollection<TabViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }
        
        public TabsViewModel(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _noteCommonMethods = resolver.Resolve<NoteCommonMethods>();

            //var collectionNoteViewModel = resolver.Resolve<CollectionNoteViewModel>();
            deliveryNoteViewModel = resolver.Resolve<DeliveryNoteViewModel>();
            var clientReceivedViewModel = resolver.Resolve<ClientReceivedViewModel>();
            var editNoteViewModel = resolver.Resolve<EditNoteViewModel>();

            Items = new ObservableCollection<TabViewModel>
            {
                //new TabViewModel(collectionNoteViewModel),
                //new TabViewModel(deliveryNoteViewModel),
                new TabViewModel(clientReceivedViewModel),
                new TabViewModel(editNoteViewModel),
            };

            PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_isInitializing) return;

            if (propertyChangedEventArgs.PropertyName == nameof(SelectedItem))
            {
                await InitializeSelectedItemAsync();
            }
        }

        private async Task InitializeSelectedItemAsync()
        {
            if (SelectedItem == null)return;

            await _noteCommonMethods.GetNoteHeaders((SelectedItem as TabViewModel)?.Content.NoteStatus);

            (SelectedItem as TabViewModel)?.InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            if (Items == null) return;

            _isInitializing = true;

            await _noteCommonMethods.InitializeAsync();

            //var tasks = new HashSet<Task>();
            //foreach (var item in Items)
            //    tasks.Add(item.InitializeAsync());
            //await Task.WhenAll(tasks);

            _isInitializing = false;

            await InitializeSelectedItemAsync();
        }

        public void Clear()
        {
            _noteCommonMethods.Clear();
        }
    }
}
