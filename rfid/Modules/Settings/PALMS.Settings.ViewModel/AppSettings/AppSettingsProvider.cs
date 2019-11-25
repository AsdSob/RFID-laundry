using System;
using System.Configuration;
using System.Threading.Tasks;

namespace PALMS.Settings.ViewModel.AppSettings
{
    public class AppSettingsProvider : IAppSettingsProvider
    {
        private const string ConnectionName = "PrimeConnection";

        public string ConnectionString { get; set; }

        public void Read()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
        }

        public Task ReadAsync()
        {
            return Task.Factory.StartNew(Read).ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    var exception = x.Exception?.Flatten();
                    var message = exception?.Message;
                    throw new Exception($"Settings provider read error: {message}", exception);
                }
            });
        }

        public void Save()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConnectionStringSettings cs = config.ConnectionStrings.ConnectionStrings[ConnectionName];

            cs.ConnectionString = ConnectionString;

            config.Save();
        }

        public Task SaveAsync()
        {
            return Task.Factory.StartNew(Save).ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    var exception = x.Exception?.Flatten();
                    var message = exception?.Message;
                    throw new Exception($"Settings provider save error: {message}", exception);
                }
            });
        }
    }
}