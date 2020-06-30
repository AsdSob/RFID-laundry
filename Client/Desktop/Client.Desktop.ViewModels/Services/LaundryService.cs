using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;
using Storage.Laundry.Models;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Services
{

    public class LaundryService : BaseService, ILaundryService
    {
        private readonly IDbContextFactory _contextFactory;

        public LaundryService(IDbContextFactory contextFactory) : base(contextFactory)
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

        public async Task<ObservableCollection<MasterLinenEntityViewModel>> MasterLinens()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<MasterLinenEntity>().ToListAsync();
                var entitiesViewModel = entities.Select(x => new MasterLinenEntityViewModel(x));
                return entitiesViewModel.ToObservableCollection();
            }
        }

        public virtual async Task<ObservableCollection<ClientEntityViewModel>> Clients()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<ClientEntity>().ToListAsync();
                var entitiesViewModel = entities.Select(x => new ClientEntityViewModel(x));
                return entitiesViewModel.ToObservableCollection();
            }
        }
        
        public async Task<ObservableCollection<DepartmentEntityViewModel>> Departments()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<DepartmentEntity>()
                    .Include(x => x.StaffDetailsEntity).ToListAsync();

                var entitiesViewModel = entities.Select(x => new DepartmentEntityViewModel(x));
                return entitiesViewModel.ToObservableCollection();
            }
        }

        public async Task<ObservableCollection<ClientLinenEntityViewModel>> ClientLinens()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<ClientLinenEntity>()
                    .Include(x => x.MasterLinenEntity)
                    .ToListAsync();

                var entitiesViewModel = entities.Select(x => new ClientLinenEntityViewModel(x));
                return entitiesViewModel.ToObservableCollection();
            }
        }        
        
        public async Task<ObservableCollection<RfidReaderEntityViewModel>> RfidReaders()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<RfidReaderEntity>().ToListAsync();

                var entitiesViewModel = entities.Select(x => new RfidReaderEntityViewModel(x));
                return entitiesViewModel.ToObservableCollection();
            }
        }        
        
        public async Task<ObservableCollection<RfidAntennaEntityViewModel>> RfidAntennas()
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<RfidAntennaEntity>().ToListAsync();

                var entitiesViewModel = entities.Select(x => new RfidAntennaEntityViewModel(x));
                return entitiesViewModel.ToObservableCollection();
            }
        }












        public async Task<ICollection<T>> GetEntities<T>(Expression<Func<T, object>> include) where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<T>().Include(include).ToListAsync();

                return entities;
            }

        }



    }
}

