using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.ViewModels
{
    public class MenuViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly IResolver _resolver;
        private ObservableCollection<ISection> _items;
        private ISection _selectedItem;
        
        public ObservableCollection<ISection> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }
        public ISection SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public MenuViewModel(IResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

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
            var isAdmin = roles.Any(x => x == RoleEnum.Admin);

            foreach (var section in GetSections())
            {
                if (isAdmin)
                {
                    section.IsVisible = true;
                }
                else
                {
                    var authRoleAttribute = Attribute.GetCustomAttribute(section.GetType(), typeof(AuthRolesAttribute)) as AuthRolesAttribute;
                    if (authRoleAttribute == null) continue;

                    section.IsVisible = authRoleAttribute.Roles.Any(r => roles.Any(role => role == r));
                }

                if (section.IsVisible)
                {
                    Helper.RunInMainThread(() => Items.Add(section));
                }
            }

            //Helper.RunInMainThread(() => SelectedItem = Items.FirstOrDefault());
        }
    }
}
