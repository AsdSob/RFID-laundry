using System.Threading.Tasks;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.AppSettings
{
    public interface IAppSettingsProvider : IAppSettings
    {
        void Read();

        Task ReadAsync();

        void Save();

        Task SaveAsync();
    }
}