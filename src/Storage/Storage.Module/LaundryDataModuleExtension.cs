using Autofac;

namespace Storage.Module
{
    public static class LaundryDataModuleExtension
    {
        public static ContainerBuilder RegisterLaundryDataModule(this ContainerBuilder builder)
        {
            builder.RegisterModule<LaundryDataModule>();

            return builder;
        }
    }
}
