using Autofac;
using Client.Desktop.Laundry.Module;
using Client.Desktop.Laundry.Services;
using Client.Desktop.ViewModels;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Content.Administration;
using Client.Desktop.ViewModels.Services;
using Client.Desktop.ViewModels.Windows;
using Client.Desktop.Views.Services;
using Common.Logger.Module;
using Storage.Module;

namespace Client.Desktop.Laundry.ViewModels
{
    public class ViewModelLocator
    {
        public static IContainer Container { get; set; }

        public MainViewModel MainViewModel => Container.Resolve<MainViewModel>();

        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();

            RegisterViewModels(builder);
            RegisterModules(builder);
            RegisterServices(builder);

            Container = builder.Build();
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<LoginWindowViewModel>();
            builder.RegisterType<MenuViewModel>().SingleInstance();

            builder.RegisterType<DataViewModel>().SingleInstance();
            builder.RegisterType<MasterClientViewModel>().SingleInstance();
            builder.RegisterType<MasterStaffViewModel>().SingleInstance();
            builder.RegisterType<MasterLinenViewModel>().SingleInstance();

            builder.RegisterType<RfidReaderWindowModel>().SingleInstance();
            builder.RegisterType<StaffChangeWindowModel>();

            builder.RegisterType<AuthManageViewModel>();
            builder.RegisterType<TagRegistrationViewModel>().SingleInstance();
            builder.RegisterType<BinSoilCollectionViewModel>().SingleInstance();
        }

        private void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterSettingsModule();
            builder.RegisterMainModule();
            builder.RegisterLaundryDataModule();
            builder.RegisterLoggerModule();
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<LaundryService>().As<ILaundryService>().SingleInstance();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<MainDispatcher>().As<IMainDispatcher>().SingleInstance();
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().SingleInstance();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().SingleInstance();
            builder.RegisterType<JsonSerializer>().As<ISerializer>();
            builder.RegisterType<SettingsManagerProvider>().As<ISettingsManagerProvider>();
        }
    }
}
