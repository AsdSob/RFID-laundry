using Autofac;

namespace Client.Desktop.Laundry.Module
{
    public static class SettingsModuleExtension
    {
        public static ContainerBuilder RegisterSettingsModule(this ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();

            return builder;
        }
    }
}