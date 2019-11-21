using Autofac;

namespace Client.Desktop.Laundry.Module
{
    public static class MainModuleExtension
    {
        public static ContainerBuilder RegisterMainModule(this ContainerBuilder builder)
        {
            builder.RegisterModule<MainModule>();

            return builder;
        }
    }
}