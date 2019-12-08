using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Settings.ViewModel.ViewModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.ViewModels
{
    public class MenuViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly IResolver _resolver;
        private ObservableCollection<ISection> _items;
        private Type _selectedItem;
        
        public ObservableCollection<ISection> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }
        public Type SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public ICommand NewCommand { get; }
        public ICommand ConveyorCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ClientCommand { get; }
        public ICommand LinenCommand { get; }

        public MenuViewModel(IResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

            ClientCommand = new RelayCommand(() => Select(typeof(ClientViewModel)));
            LinenCommand = new RelayCommand(() => Select(typeof(LinenViewModel)));
            NewCommand = new RelayCommand(() => Select(typeof(DataViewModel)));
            ConveyorCommand = new RelayCommand(() => Select(typeof(VendorDetailsViewModel)));

            Items = new ObservableCollection<ISection>();
        }

        public async Task InitializeAsync()
        {
            //await Task.Run(() =>
            //{
            //    var sections = GetSections();

            //    foreach (var section in sections)
            //    {
            //        section.IsVisible = false;
            //        Helper.RunInMainThread(() => Items.Add(section));
            //    }

            //    Helper.RunInMainThread(() => SelectedItem = Items.FirstOrDefault());
            //});
        }

        private void Select(Type type)
        {
            // TODO: use enum?
            SelectedItem = type;
        }

        private IEnumerable<ISection> GetSections()
        {
            var sectionType = typeof(ISection);
            var sections = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.Contains("PALMS"))
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.IsAbstract == false && sectionType.IsAssignableFrom(x))
                .Select(x => _resolver.Resolve(x))
                .OfType<ISection>()
                .Where(x => x.IsVisible)
                .OrderBy(x => x.Index);

            return sections;
        }

        public void Set(ICollection<RoleEnum> roles)
        {
            foreach (var section in GetSections())
            {
                section.IsVisible = true;                

                Helper.RunInMainThread(() => Items.Add(section));
            }

        }
    }
}
