using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface ILaundryService
    {
        Task<ICollection<Laundry>> GetAllAsync();
    }
    
    public class Laundry
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LaundryService : ILaundryService
    {
        private readonly IDbContextFactory _contextFactory;

        public LaundryService(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ICollection<Laundry>> GetAllAsync()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var laundries = await context.Set<LaundryEntity>().ToListAsync();

                return laundries.Select(x => new Laundry
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }
        }
    }
}

