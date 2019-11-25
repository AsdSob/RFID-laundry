using Autofac;

namespace PALMS.ViewModels.Common
{
    public interface IIocModule
    {
        void Register(ContainerBuilder container);
    }
}
