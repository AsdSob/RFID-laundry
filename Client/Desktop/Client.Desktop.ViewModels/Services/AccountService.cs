using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Services;
using Storage.Core.Abstract;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountService(IDbContextFactory contextFactory, IAuthenticationService authenticationService) : base(contextFactory)
        {
            _authenticationService = authenticationService;
        }

        public override Task AddOrUpdateAsync<T>(T entity)
        {
            if (!(entity is AccountEntity accountEntity)) return Task.CompletedTask;

            accountEntity.Password = _authenticationService.GetSecretPassword(accountEntity.Password);

            return base.AddOrUpdateAsync(entity);
        }
    }
}
