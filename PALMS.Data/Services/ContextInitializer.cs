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
            //AddStaff(context);
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

        private void AddStaff(DataContext context)
        {
            var staff = context.ClientStaves;

            staff.Add(new ClientStaff
            {
                DepartmentId = 1,
                Name = "Mr. One",
                StaffId = "0011AA",
                PackingValue = 10
            });
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