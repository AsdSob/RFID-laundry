using System;
using System.Collections.Generic;
using PALMS.Data.Objects;

namespace PALMS.Data.Audit
{
    public class AuditModel : IEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int Id { get; set; }
        public bool IsNew => false;
        public string UserName { get; set; }
        public string EntityTypeName { get; set; }
        public int EntityTypeId { get; set; }
        public string StateName { get; set; }
        public int State { get; set; }
        public string Description { get; set; }

        public ICollection<AuditModelProperty> AuditModelProperties { get; set; }
    }

    public class AuditModelProperty
    {
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
