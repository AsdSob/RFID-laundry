using PALMS.Data.Services;
using PALMS.ViewModels.Common.Services;

namespace PALMS.ViewModels.Services
{
    public class ContextFactory : IContextFactory
    {
        private readonly IAppSettings _appSettings;

        public ContextFactory(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public DataContext Create()
        {
            var dataContext = new DataContext(_appSettings.ConnectionString);

            dataContext.Database.Connection.ConnectionString = _appSettings.ConnectionString;

            return dataContext;
        }
    }
}