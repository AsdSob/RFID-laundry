using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;
using Storage.Laundry.Models;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface ILaundryService : IBaseService
    {
    }

    public class LaundryService : ILaundryService
    {
        private readonly IDbContextFactory _contextFactory;

        public LaundryService(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;

            using (var context = _contextFactory.Create())
            {
                var linen = context.Set<MasterLinenEntity>().FirstOrDefault();
                if (linen == null)
                {
                    // TODO: use seed dbcontext seed

                    var masterLinens = new List<MasterLinenEntity>();

                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Shirt",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Blouse",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Trouser",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Skirt",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Jacket",
                        PackingValue = 1
                    }); masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Scarf",
                        PackingValue = 1
                    }); masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Apron",
                        PackingValue = 1
                    }); masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Waistcoat",
                        PackingValue = 1
                    }); masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "T-Shirt",
                        PackingValue = 1
                    }); masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Tie",
                        PackingValue = 1
                    }); masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Nanny Dress",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Butler Gloves",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Shila",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Abaya",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Pullover",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Sweater",
                        PackingValue = 1
                    });
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Cardigan",
                        PackingValue = 1
                    });                    
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Over Coat",
                        PackingValue = 1
                    });                    
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Nursing Bottom",
                        PackingValue = 1
                    });                    
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Nursing Top",
                        PackingValue = 1
                    });                    
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Lab Coat",
                        PackingValue = 1
                    });                    
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Doctors Coat",
                        PackingValue = 1
                    });                   
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Patient Gown",
                        PackingValue = 1
                    });                   
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Patient Blanket",
                        PackingValue = 1
                    });                   
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Crew Sleeping - Top",
                        PackingValue = 1
                    });                   
                    masterLinens.Add(new MasterLinenEntity()
                    {
                        Name = "Crew Sleeping - Bottom",
                        PackingValue = 1
                    });

                    context.Set<MasterLinenEntity>().AddRange(masterLinens);

                    context.Set<ClientEntity>().Add(new ClientEntity()
                    {
                        Name = "Etihad Airways",
                        Active = true,
                        Address = "Dubai Airport",
                        CityId = 1,
                        ShortName = "Etihad",
                    });

                    context.Set<DepartmentEntity>().Add(new DepartmentEntity()
                    {
                        Name = "Houskeeping",
                        ClientId = 1,
                        DepartmentTypeId = 1,
                        
                    });

                    context.SaveChanges();
                }
            }
        }

        public async Task<ICollection<T>> GetAllAsync<T>() where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<T>().ToListAsync();

                return entities;
            }
        }

        public async Task DeleteAsync<T>(T entity) where T : class, IEntity<int>
        {
            if(entity.Id == 0) return;

            using (var context = await _contextFactory.CreateAsync())
            {
                context.Remove(entity);
                context.SaveChanges();
            }
        }

        public async Task DeleteAsync<T>(IEnumerable<T> entities) where T : class, IEntity<int>
        {
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public async Task AddOrUpdateAsync<T>(T entity) where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                if (entity.Id == 0)
                {
                    context.Attach(entity);
                }
                else
                {
                    context.Update(entity);
                }

                context.SaveChanges();
            }
        }


    }
}

