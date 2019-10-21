using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Data.Objects.Payment;

namespace PALMS.Invoices.ViewModel.EntityViewModel
{
    public class InvoiceSumViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private DateTime _dateStart;
        private DateTime _dateEnd;
        private int _clientId;
        private double _vat;


        private double _grandTotal;
        private double _vatAmount;
        private double _amount;

        private List<NoteHeader> _noteHeaders;
        private List<NoteRow> _noteRows;
        private List<DepartmentContract> _departmentContracts;
        private List<Department> _departments;

        public List<DepartmentContract> DepartmentContracts
        {
            get => _departmentContracts;
            set => Set(ref _departmentContracts, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<NoteHeader> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }

        public double GrandTotal
        {
            get => _grandTotal;
            set => Set(ref _grandTotal, value);
        }
        public double VatAmount
        {
            get => _vatAmount;
            set => Set(ref _vatAmount, value);
        }
        public double Amount
        {
            get => _amount;
            set => Set(ref _amount, value);
        }


        public double Vat
        {
            get => _vat;
            set => Set(ref _vat, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public DateTime DateEnd
        {
            get => _dateEnd;
            set => Set(ref _dateEnd, value);
        }
        public DateTime DateStart
        {
            get => _dateStart;
            set => Set(ref _dateStart, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }


        public Invoice OriginalObject { get; set; }
        public string Error { get; }
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public InvoiceSumViewModel()
        {
            OriginalObject = new Invoice();

            Departments = new List<Department>();
            NoteHeaders = new List<NoteHeader>();
            NoteRows = new List<NoteRow>();
            DepartmentContracts = new List<DepartmentContract>();
        }

        public InvoiceSumViewModel(Invoice invoice) : this()
        {
            Update(invoice);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(Invoice invoice)
        {
            OriginalObject = invoice;

            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            Name = OriginalObject.Name;
            DateEnd = OriginalObject.DateEnd;
            DateStart = OriginalObject.DateStart;
            Vat = OriginalObject.VAT;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ClientId = ClientId;
            OriginalObject.DateEnd = DateEnd;
            OriginalObject.DateStart = DateStart;
            OriginalObject.VAT = Vat;

        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    ClientId != OriginalObject.ClientId ||
                                    DateEnd != OriginalObject.DateEnd ||
                                    DateStart != OriginalObject.DateStart ||
                                    Name != OriginalObject.Name ||
                                    !Equals(Vat, OriginalObject.VAT);
    }

}
