using System;
using System.Collections.Generic;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.InvoiceModel;
using PALMS.Data.Objects.NoteModel;

namespace PALMS.Data.Objects.Payment
{
    public class Invoice : NameEntity
    {
        public int ClientId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public double VAT { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<ExtraCharge> ExtraCharges { set; get; }
        public virtual ICollection<NoteHeader> NoteHeaders { set; get; }
        public virtual ICollection<DepartmentContract> DepartmentContracts { set; get; }
        public virtual ICollection<TaxAndFees> TaxAndFeeses { set; get; }

    }

}
