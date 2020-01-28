using System.IO;
using Autofac;
using Client.Desktop.Laundry.Configuration;
using Client.Desktop.ViewModels.Common.Configuration;
using Storage.Core.Abstract;

namespace Client.Desktop.Laundry.Module
{
    public class SettingsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
            {
                var settingsManager = new SettingsManager<AppConfiguration>();

                var settings = settingsManager.LoadSettings();
                if (settings == null)
                    throw new FileNotFoundException("settings file");

                return settings;
            }).As<AppConfiguration>();

            builder.Register(x =>
                {
                    var appSettings = x.Resolve<AppConfiguration>();

                    return new DbConfiguration
                    {
                        ConnectionString = appSettings.DbConnectionString
                    };
                })
                .As<IDbConfiguration>();
        }
    }
}
