using System;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects;
using PALMS.ViewModels.Common.Interfaces;

namespace PALMS.Invoices.ViewModel.EntityViewModel
{
    public class InvoiceItems : ViewModelBase, IOrderItem
    {
        private int _pId;
        private int _id;
        private string _name;
        private bool _isSelected;
        private int? _parentId;
        private int _orderNumber;
        private double _qtyCollected;
        private double _qtyDelivered;
        private double _qtyClientReceived;
        private double _weightCollected;
        private double _weightDelivered;
        private double _amount;
        private double _discountAmount;
        private double _vatAmount;

        public IEntity OriginalObject { get; set; }

        public int OrderNumber
        {
            get => _orderNumber;
            set => Set(ref _orderNumber, value);
        }
        public int? ParentId
        {
            get => _parentId;
            set => Set(ref _parentId, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
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
        public int PId
        {
            get => _pId;
            set => Set(ref _pId, value);
        }

        public double QtyCollected
        {
            get => _qtyCollected;
            set => Set(ref _qtyCollected, value);
        }

        public double QtyDelivered
        {
            get => _qtyDelivered;
            set => Set(ref _qtyDelivered, value);
        }

        public double QtyClientReceived
        {
            get => _qtyClientReceived;
            set => Set(ref _qtyClientReceived, value);
        }

        public double WeightCollected
        {
            get => _weightCollected;
            set => Set(ref _weightCollected, value);
        }

        public double WeightDelivered
        {
            get => _weightDelivered;
            set => Set(ref _weightDelivered, value);
        }

        public double Amount
        {
            get => _amount;
            set => Set(ref _amount, value);
        }

        public double DiscountAmount
        {
            get => _discountAmount;
            set => Set(ref _discountAmount, value);
        }

        public double VatAmount
        {
            get => _vatAmount;
            set => Set(ref _vatAmount, value);
        }

        public override string ToString()
        {
            return $"{OrderNumber}";
        }

        public InvoiceItems(IEntity entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));
            Id = entity.Id;
        }

        public InvoiceItems()
        {

        }
    }
}
