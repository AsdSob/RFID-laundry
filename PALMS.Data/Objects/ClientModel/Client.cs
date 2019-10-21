using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Data.Objects.Payment;
using PALMS.Data.Objects.Received_data;

namespace PALMS.Data.Objects.ClientModel
{
    public class Client : NameEntity
    {
        public string ShortName { get; set; }
        public string InvoiceName { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OrderNumber { get; set; }

        public virtual ICollection<LinenList> LinenLists { get; set; }
        public virtual ICollection<NoteHeader> NoteHeaders { get; set; }
        public virtual ICollection<LaundryKg> LaundryKg { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<TaxAndFees> TaxAndFees { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }

        public virtual ClientInfo ClientInfo { get; set; }
        
    }
}
