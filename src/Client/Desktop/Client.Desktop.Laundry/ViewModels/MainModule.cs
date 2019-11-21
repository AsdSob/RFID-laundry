using Autofac;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.Views.Services;

namespace Client.Desktop.Laundry.ViewModels
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
        }
    }
}