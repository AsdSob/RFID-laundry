using GalaSoft.MvvmLight;
using PALMS.Data.Objects.InvoiceModel;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.ViewModel.EntityViewModel
{
    public class ExtraChargeViewModel : ViewModelBase
    {
        private string _name;
        private double _amount;
        private int? _invoiceId;
        private int _id;
        private int _clientId;

        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public int? InvoiceId
        {
            get => _invoiceId;
            set => Set(ref _invoiceId, value);
        }
        public double Amount
        {
            get => _amount;
            set => Set(ref _amount, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public ExtraCharge OriginalObject { get; set; }
        public string Error { get; }
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;


        public ExtraChargeViewModel()
        {
            OriginalObject = new ExtraCharge();
        }

        public ExtraChargeViewModel(ExtraCharge extraCharge) : this()
        {
            Update(extraCharge);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(ExtraCharge extraCharge)
        {
            OriginalObject = extraCharge;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            Amount = OriginalObject.Amount;
            InvoiceId = OriginalObject.InvoiceId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.Amount = Amount;
            OriginalObject.InvoiceId = InvoiceId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Name != OriginalObject.Name ||
                                    InvoiceId != OriginalObject.InvoiceId ||
                                    Amount != OriginalObject.Amount;


        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            string error;
            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error) ||
                    !Name.ValidateBySpaces(out error))
                {
                    return error;
                }
            }
            if (columnName == nameof(Amount))
            {
                if (!Name.ValidateRequired(out error))
                {
                    return error;
                }
            }
            return null;
        }
    }
    
}

