using Autofac;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.MasterLinens.ViewModel
{
    public class MasterLinensModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<MasterLinensSection>().SingleInstance();

            container.RegisterType<TypeLinenTabViewModel>().SingleInstance();

            container.Register(x => new MasterLinensViewModel(x.Resolve<TypeLinenTabViewModel>(), x.Resolve<ICanExecuteMediator>()))
                     .As<MasterLinensViewModel>().SingleInstance();
        }
    }
}
