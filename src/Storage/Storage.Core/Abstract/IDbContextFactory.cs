using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Storage.Core.Abstract
{
    public interface IDbContextFactory
    {
        Task<DbContext> CreateAsync();
        DbContext Create();
    }
}
