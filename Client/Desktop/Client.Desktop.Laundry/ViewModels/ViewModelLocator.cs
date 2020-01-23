using Autofac;
using Client.Desktop.Laundry.Module;
using Client.Desktop.ViewModels;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Content.Master;
using Client.Desktop.ViewModels.Windows;
using Client.Desktop.Views.Services;
using Common.Logger.Module;
using Microsoft.EntityFrameworkCore.Internal;
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
            builder.RegisterType<MenuViewModel>().SingleInstance();

            builder.RegisterType<DataViewModel>().SingleInstance();
            builder.RegisterType<MasterClientViewModel>().SingleInstance();
            builder.RegisterType<MasterStaffViewModel>().SingleInstance();
            builder.RegisterType<MasterLinenViewModel>().SingleInstance();

            builder.RegisterType<RfidReaderWindowModel>().SingleInstance();

        }

        private void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterMainModule();
            builder.RegisterLaundryDataModule();
            builder.RegisterLoggerModule();
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<LaundryService>().As<ILaundryService>().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
        }
    }
}
