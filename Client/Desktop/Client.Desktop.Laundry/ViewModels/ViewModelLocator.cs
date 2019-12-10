using Autofac;
using Client.Desktop.Laundry.Module;
using Client.Desktop.ViewModels;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Content.Operation;
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

            Container = builder.Build();
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<MenuViewModel>().SingleInstance();

            builder.RegisterType<DataViewModel>().SingleInstance();
            builder.RegisterType<AddViewModel>().SingleInstance();
            builder.RegisterType<UniformConveyorViewModel>().SingleInstance();
        }

        private void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterMainModule();
            builder.RegisterLaundryDataModule();
            builder.RegisterLoggerModule();
        }
    }
}
