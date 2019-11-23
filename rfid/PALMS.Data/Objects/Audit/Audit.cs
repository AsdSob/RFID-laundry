using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Z.EntityFramework.Plus;

namespace PALMS.Data.Objects.Audit
{
    public class CustomAuditEntry : AuditEntry
    {
        public int EntityId { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public ICollection<CustomAuditEntryProperty> CustomProperties { get; set; }
    }

    public class CustomAuditEntryProperty : AuditEntryProperty
    {
        public int EntityId { get; set; }
        public string EntityTypeName { get; set; }
    }
}
