using Autofac;
using PALMS.Settings.ViewModel.AppSettings;
using PALMS.Settings.ViewModel.Audit;
using PALMS.Settings.ViewModel.Dictionaries;
using PALMS.Settings.ViewModel.LaundryDetails;
using PALMS.Settings.ViewModel.NoteSearchLinen;
using PALMS.Settings.ViewModel.NoteSearchLinen.Windows;
using PALMS.Settings.ViewModel.Users;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel
{
    public class SettingsModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<SettingsSection>().SingleInstance();

            container.Register(x => new SettingsViewModel(x.Resolve<TabsViewModel>(), x.Resolve<ICanExecuteMediator>()))
                .As<SettingsViewModel>()
                .SingleInstance();

            container.RegisterType<TabsViewModel>().SingleInstance();
            container.RegisterType<DictionariesViewModel>().SingleInstance();
            container.RegisterType<GroupLinenDictionaryViewModel>().SingleInstance();
            container.RegisterType<FamilyLinenDictionaryViewModel>().SingleInstance();
            container.RegisterType<TypeLinenDictionaryViewModel>().SingleInstance();

            container.RegisterType<AppSettingsViewModel>().SingleInstance();
            container.RegisterType<AppSettingsProvider>().As<IAppSettingsProvider>().As<IAppSettings>().SingleInstance();

            container.RegisterType<VendorDetailsViewModel>().SingleInstance();

            container.RegisterType<AuditHistoryViewModel>().SingleInstance();

            container.RegisterType<NoteSearchViewModel>().SingleInstance();
            container.RegisterType<NoteEditViewModel>().SingleInstance();
            container.RegisterType<UserEditViewModel>().SingleInstance();
        }
    }
}
