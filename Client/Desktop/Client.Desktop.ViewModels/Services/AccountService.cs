using Client.Desktop.ViewModels.Common.Services;
using Storage.Core.Abstract;

namespace Client.Desktop.ViewModels.Services
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(IDbContextFactory contextFactory) : base(contextFactory)
        {
        }
    }
}
