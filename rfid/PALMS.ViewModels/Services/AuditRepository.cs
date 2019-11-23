using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PALMS.Data.Audit;
using PALMS.Data.Objects.Audit;
using PALMS.Data.Services;
using Z.EntityFramework.Plus;

namespace PALMS.ViewModels.Services
{
    public class AuditRepository : Repository<AuditModel>
    {
        public AuditRepository(DataContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<AuditModel>> GetAllAsync()
        {
            var audits = await GetDbSet<CustomAuditEntry>().ToListAsync();
            var auditProperties = await GetDbSet<AuditEntryProperty>().ToListAsync();
            var auditPropertiesDict = auditProperties.GroupBy(x => x.AuditEntryID).ToDictionary(x => x.Key, x => x);

            var result = new List<AuditModel>();

            foreach (var audit in audits)
            {
                var model = new AuditModel();

                model.CreatedDate = audit.CreatedDate;
                model.UserName = audit.CreatedBy;
                model.EntityTypeName = audit.EntityTypeName;
                model.StateName = audit.StateName;
                model.State = (int)audit.State;
                model.EntityTypeId = audit.EntityId;
                model.Description = audit.Description;

                if (auditPropertiesDict.TryGetValue(audit.AuditEntryID, out var properties))
                {
                    model.AuditModelProperties = properties
                        .Select(x => new AuditModelProperty
                        {
                            PropertyName = x.PropertyName,
                            OldValue = x.OldValue,
                            NewValue = x.NewValue
                        })
                        .ToList();
                }
                

                result.Add(model);
            }

            return result;
        }
    }
}