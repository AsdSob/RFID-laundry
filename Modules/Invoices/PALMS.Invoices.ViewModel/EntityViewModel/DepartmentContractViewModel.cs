using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Interfaces;

namespace PALMS.Invoices.ViewModel.EntityViewModel
{
    public class DepartmentContractViewModel : ViewModelBase, IDataErrorInfo, IOrderItem
    {
        private int _id;
        private int _departmentId;
        private int? _quantity;
        private double _percentage;
        private int? _familyLinenId;
        private int _orderNumber;
        private string _name;
        private int? _invoiceId;

        public int? InvoiceId
        {
            get => _invoiceId;
            set => Set(ref _invoiceId, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public int DepartmentId
        {
            get => _departmentId;
            set => Set(ref _departmentId, value);
        }

        /// <summary>
        ///     Used only for new contracts. For right reference in saving.
        /// </summary>
        public DepartmentViewModel DepartmentViewModel { get; set; }

        public double Percentage
        {
            get => _percentage;
            set => Set(ref _percentage, value);
        }

        public int? Quantity
        {
            get => _quantity;
            set => Set(ref _quantity, value);
        }

        public int? FamilyLinenId
        {
            get => _familyLinenId;
            set => Set(ref _familyLinenId, value);
        }

        public int OrderNumber
        {
            get => _orderNumber;
            set => Set(ref _orderNumber, value);
        }

        public DepartmentContract OriginalObject { get; set; }

        public string Error { get; }

        public Func<DepartmentContractViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public DepartmentContractViewModel()
        {
            OriginalObject = new DepartmentContract();
        }

        public DepartmentContractViewModel(DepartmentContract departmentContract) : this()
        {
            Update(departmentContract);
        }

        private void Update(DepartmentContract departmentContract)
        {
            OriginalObject = departmentContract;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            DepartmentId = OriginalObject.DepartmentId;
            Percentage = OriginalObject.Percentage;
            Quantity = OriginalObject.Quantity;
            FamilyLinenId = OriginalObject.FamilyLinenId;
            OrderNumber = OriginalObject.OrderNumber;
            InvoiceId = OriginalObject.InvoiceId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.DepartmentId = DepartmentViewModel?.OriginalObject?.Id ?? DepartmentId;
            OriginalObject.Percentage = Percentage;
            OriginalObject.Quantity = Quantity;
            OriginalObject.FamilyLinenId = FamilyLinenId;
            OriginalObject.OrderNumber = OrderNumber;
            OriginalObject.Name = Name;
            OriginalObject.InvoiceId = InvoiceId;
        }

        public override string ToString()
        {
            return $"{OrderNumber}";
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    DepartmentId != OriginalObject.DepartmentId ||
                                    !Equals(Percentage, OriginalObject.Percentage) ||
                                    Quantity != OriginalObject.Quantity ||
                                    FamilyLinenId != OriginalObject.FamilyLinenId ||
                                    Name != OriginalObject.Name ||
                                    OrderNumber != OriginalObject.OrderNumber ||
                                    InvoiceId != OriginalObject.InvoiceId;

        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            string error;
            if (columnName == nameof(Percentage))
            {
                if (!Percentage.ValidateRequired(out error))
                {
                    return error;
                }
            }
            return null;
        }
    }
}
