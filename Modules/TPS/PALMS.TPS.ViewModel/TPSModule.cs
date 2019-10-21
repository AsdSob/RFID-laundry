using Autofac;
using PALMS.ViewModels.Common;

namespace PALMS.TPS.ViewModel
{
    public class TpsModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<TpsSection>().SingleInstance();

            container.RegisterType<TpsViewModel>().SingleInstance();

        }
    }
}