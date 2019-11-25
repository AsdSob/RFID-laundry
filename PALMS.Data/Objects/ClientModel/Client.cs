using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class Client : NameEntity
    {
        public string ShortName { get; set; }
        public string Color { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //public virtual ICollection<LinenList> LinenLists { get; set; }
        //public virtual ICollection<NoteHeader> NoteHeaders { get; set; }
        //public virtual ICollection<Department> Departments { get; set; }
        //public virtual ICollection<TaxAndFees> TaxAndFees { get; set; }
        //public virtual ICollection<Invoice> Invoices { get; set; }
        //public virtual ICollection<TrackingService> TrackingServices { get; set; }

        //public virtual ClientInfo ClientInfo { get; set; }
        
    }
}
