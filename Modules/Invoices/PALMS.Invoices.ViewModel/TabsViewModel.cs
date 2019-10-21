using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Invoices.ViewModel.Tabs;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Invoices.ViewModel
{
    public class TabsViewModel : ViewModelBase, ISettingsViewModel, IInitializationAsync
    {
        private ObservableCollection<TabViewModel> _items;
        private object _selectedItem;

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
            var invoicingViewModel = resolver.Resolve<InvoicingViewModel>();
            var invoiceEditViewModel = resolver.Resolve<InvoiceEditViewModel>();

            Items = new ObservableCollection<TabViewModel>
            {
                new TabViewModel(invoicingViewModel),
                new TabViewModel(invoiceEditViewModel),
            };

            SelectedItem = Items.FirstOrDefault();
            PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(SelectedItem))
            {

            }
        }

        public async Task InitializeAsync()
        {
            if (Items == null) return;

            var tasks = new HashSet<Task>();
            foreach (var item in Items)
                tasks.Add(item.InitializeAsync());

            await Task.WhenAll(tasks);
        }
    }
}
