using System.Data.Entity;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.Data.Services
{
    public class ContextInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            AddClient(context);
            AddDepartment(context);
            AddConveyorItem(context);
            AddMasterLinen(context);

            context.SaveChanges();
        }

        private void AddClient(DataContext context)
        {
            var client = context.Clients;

            client.Add(new Client
            {
                Name = "Client Test",
                Active =  true,
                CityId = 1,
                ShortName = "Test",
            });
        }

        private void AddDepartment(DataContext context)
        {
            var dep = context.Departments;

            dep.Add(new Department
            {
                Name = "Uniforms",
                ClientId = 1,
                DepartmentTypeId = 1,
            });
        }

        private void AddConveyorItem(DataContext context)
        {
            var staff = context.ConveyorItems;

            for (var i = 1; i <= 600; i++)
            {
                staff.Add(new ConveyorItem
                {
                    BeltNumber = 1,
                    SlotNumber = i,
                });
            }

            for (var i = 1; i <= 776; i++)
            {
                staff.Add(new ConveyorItem()
                {
                    BeltNumber = 2,
                    SlotNumber = i,
                });
            }

        }

        private void AddMasterLinen(DataContext context)
        {
            var masterLinen = context.MasterLinens;

            masterLinen.Add(new MasterLinen()
            {
                PackingValue = 1,
                Name = "Jacket"
            });

            masterLinen.Add(new MasterLinen()
            {
                PackingValue = 1,
                Name = "Trouser"
            });

            masterLinen.Add(new MasterLinen()
            {
                PackingValue = 1,
                Name = "Shirt"
            });

            masterLinen.Add(new MasterLinen()
            {
                PackingValue = 1,
                Name = "Blouse"
            });
        }

    }
}