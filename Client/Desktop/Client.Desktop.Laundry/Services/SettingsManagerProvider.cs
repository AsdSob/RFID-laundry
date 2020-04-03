using System.Threading.Tasks;
using Client.Desktop.Laundry.Configuration;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Laundry.Services
{
    public class SettingsManagerProvider : ISettingsManagerProvider
    {
        public Task<T> GetAsync<T>(string fileName) where T: class
        {
            return new SettingsManager<T>(fileName).LoadSettingsAsync();
        }

        public Task SaveAsync<T>(T data, string fileName) where T: class
        {
            return new SettingsManager<T>(fileName).SaveSettingsAsync(data);
        }
    }
}
