using PALMS.Data.Services;

namespace PALMS.ViewModels.Services
{
    public interface IContextFactory
    {
        DataContext Create();
    }
}