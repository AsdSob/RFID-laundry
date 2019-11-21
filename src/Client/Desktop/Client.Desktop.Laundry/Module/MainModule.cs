using Autofac;
using Client.Desktop.Laundry.Services;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.Views.Services;

namespace Client.Desktop.Laundry.Module
{
    public class MainModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<Resolver>().As<IResolver>().SingleInstance();
        }
    }
}