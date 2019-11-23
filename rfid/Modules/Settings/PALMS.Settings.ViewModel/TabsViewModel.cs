using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Settings.ViewModel.AppSettings;
using PALMS.Settings.ViewModel.Audit;
using PALMS.Settings.ViewModel.Dictionaries;
using PALMS.Settings.ViewModel.LaundryDetails;
using PALMS.Settings.ViewModel.NoteLinenReplacement;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel
{
    public class TabsViewModel : ViewModelBase, ISettingsViewModel, IInitializationAsync
    {
        private ObservableCollection<TabViewModel> _items;

        public ObservableCollection<TabViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public TabsViewModel(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            Items = new ObservableCollection<TabViewModel>
            {
                new TabViewModel(resolver.Resolve<DictionariesViewModel>()),
                new TabViewModel(resolver.Resolve<AppSettingsViewModel>()),
                new TabViewModel(resolver.Resolve<VendorDetailsViewModel>()),
                new TabViewModel(resolver.Resolve<AuditHistoryViewModel>()),
                new TabViewModel(resolver.Resolve<NoteLinenReplacementViewModel>())
            };
        }

        public async Task InitializeAsync()
        {
            if (Items == null) return;

            var tasks = new HashSet<Task>();
            foreach (var item in Items)
                tasks.Add(item.InitializeAsync());

            await Task.WhenAll(tasks);
        }

        public bool HasChanges()
        {
            return Items?.Any(x => x.Content.HasChanges()) == true;
        }
    }
}
