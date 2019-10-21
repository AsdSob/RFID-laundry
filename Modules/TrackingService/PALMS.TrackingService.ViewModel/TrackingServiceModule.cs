using Autofac;
using PALMS.ViewModels.Common;

namespace PALMS.TrackingService.ViewModel
{
    public class TrackingServiceModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<TrackingServiceSection>().SingleInstance();

            container.RegisterType<TrackingServiceViewModel>().SingleInstance();
        }
    }
}