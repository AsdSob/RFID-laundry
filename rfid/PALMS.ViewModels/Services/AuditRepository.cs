using PALMS.Data.Audit;
using PALMS.Data.Services;

namespace PALMS.ViewModels.Services
{
    public class AuditRepository : Repository<AuditModel>
    {
        public AuditRepository(DataContext context) : base(context)
        {
        }

    }
}