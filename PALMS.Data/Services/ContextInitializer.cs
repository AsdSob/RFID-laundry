using System.Data.Entity;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.Audit;
using PALMS.Data.Objects.Conveyor;

namespace PALMS.Data.Services
{
    public class ContextInitializer : CreateDatabaseIfNotExists<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            AddGroupLinen(context);
            AddUser(context);
            AddCClient(context);
            AddCLinen(context);
            AddCClientLinen(context);
            context.SaveChanges();
        }

        private void AddCClient(DataContext context)
        {
            var client = context.CClients;

            client.Add(new CClient { Name = "Client 1"});
            client.Add(new CClient { Name = "Client 2"});
            client.Add(new CClient { Name = "Client 3"});
        }

        private void AddCLinen(DataContext context)
        {
            var linen = context.CLinens;

            linen.Add(new CLinen { Name = "Shirt" });
            linen.Add(new CLinen { Name = "Trouser" });
            linen.Add(new CLinen { Name = "Jacket" });
        }

        private void AddCClientLinen(DataContext context)
        {
            var linen = context.CClientLinens;

            linen.Add(new CClientLinen { ClientId = 1, LinenId  = 1, StaffId = 330});
            linen.Add(new CClientLinen { ClientId = 1, LinenId  = 2, StaffId = 330});
            linen.Add(new CClientLinen { ClientId = 1, LinenId  = 3, StaffId = 330});

            linen.Add(new CClientLinen { ClientId = 2, LinenId  = 1, StaffId = 330});
            linen.Add(new CClientLinen { ClientId = 2, LinenId  = 2, StaffId = 330});
        }

        private void AddGroupLinen(DataContext context)
        {
            var groupLinen = context.GroupLinens;

            groupLinen.Add(new GroupLinen { Name = "Group 1" });
        }

        private void AddUser(DataContext context)
        {
            var user = context.Users;

            user.Add(new User
            {
                Name = "Admin",
                Password = "Admin",
                UserRoleId = 5,
            });
        }
    }
}