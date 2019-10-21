using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Interfaces;

namespace PALMS.Invoices.ViewModel.EntityViewModel
{
    public class TaxAndFeeViewModel : ViewModelBase, IDataErrorInfo, IOrderItem
    {
        private string _name;
        private int _unitId;
        private double _number;
        private int _orderNumber;
        private int _clientId;
        private int? _invoiceId;

        public int? InvoiceId
        {
            get => _invoiceId;
            set => Set(ref _invoiceId, value);
        }
        public TaxAndFees OriginalObject { get; set; }
        public string Error { get; set; }

        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public int UnitId
        {
            get => _unitId;
            set => Set(ref _unitId, value);
        }
        public double Number
        {
            get => _number;
            set => Set(ref _number, value);
        }
        public int OrderNumber
        {
            get => _orderNumber;
            set => Set(ref _orderNumber, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public TaxAndFeeViewModel()
        {
            OriginalObject = new TaxAndFees();
        }

        public TaxAndFeeViewModel(TaxAndFees taxAndFees) : this()
        {
            OriginalObject = taxAndFees;

            ClientId = OriginalObject.ClientId;
            Name = OriginalObject.Name;
            UnitId = OriginalObject.UnitId;
            Number = OriginalObject.Number;
            OrderNumber = OriginalObject.Priority;
            InvoiceId = OriginalObject.InvoiceId;
        }

        public override string ToString()
        {
            return $"{OrderNumber}";
        }

        public bool HasChanges()
        {
            var originalObject = OriginalObject;

            return originalObject.IsNew ||
                   originalObject.ClientId != ClientId ||
                   originalObject.Name != Name ||
                   originalObject.UnitId != UnitId ||
                   originalObject.Number != Number ||
                   originalObject.Priority != OrderNumber ||
                   originalObject.InvoiceId != InvoiceId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.ClientId = ClientId;
            OriginalObject.Name = Name;
            OriginalObject.Priority = OrderNumber;
            OriginalObject.Number = Number;
            OriginalObject.UnitId = UnitId;
            OriginalObject.InvoiceId = InvoiceId;
        }

        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(UnitId))
            {
                UnitId.ValidateRequired(out error);
            }
            else if (columnName == nameof(Name))
            {
                Name.ValidateRequired(out error);
                Name.ValidateBySpaces(out error);
            }
            else if (columnName == nameof(Number))
            {
                Number.ValidateRequired(out error);
            }

            return null;
        }
    }
}
