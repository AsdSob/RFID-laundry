using System.Threading.Tasks;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface ISettingsManagerProvider
    {
        public Task<T> GetAsync<T>(string fileName) where T: class;
        Task SaveAsync<T>(T data, string fileName) where T: class;
    }
}
