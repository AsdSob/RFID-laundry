using System.Data.Entity;

namespace PALMS.Data.Services
{
    public class ContextInitializer : CreateDatabaseIfNotExists<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            //AddFamilyLinen(context);
            //AddGroupLinen(context);
            //AddMasterLinen(context);
            //AddPrimeInfo(context);
            context.SaveChanges();
        }

    }
}