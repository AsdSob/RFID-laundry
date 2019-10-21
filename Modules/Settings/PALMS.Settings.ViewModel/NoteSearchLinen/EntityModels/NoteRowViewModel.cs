using System;
using System.ComponentModel;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.NoteModel;

namespace PALMS.Settings.ViewModel.NoteLinenReplacement.EntityModels
{
    public class NoteRowViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _id;
        private int _noteHeaderId;
        private int _linenListId;
        private double _primeCollectedQty;
        private double _primeDeliveredQty;
        private double _clientReceivedQty;
        private string _comment;
        private int _weight;
        private int _serviceTypeId;
        private double _price;
        private int _priceUnit;
        private bool _hasMasterLinen;
        private string _linenName;


        public string LinenName
        {
            get => _linenName;
            set => Set(ref _linenName, value);
        }
        public bool HasMasterLinen
        {
            get => _hasMasterLinen;
            set => Set(ref _hasMasterLinen, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public int NoteHeaderId
        {
            get => _noteHeaderId;
            set => Set(ref _noteHeaderId, value);
        }
        public int LinenListId
        {
            get => _linenListId;
            set => Set(ref _linenListId, value);
        }
        public double PrimeCollectedQty
        {
            get => _primeCollectedQty;
            set => Set(ref _primeCollectedQty, value);
        }
        public double PrimeDeliveredQty
        {
            get => _primeDeliveredQty;
            set => Set(ref _primeDeliveredQty, value);
        }
        public double ClientReceivedQty
        {
            get => _clientReceivedQty;
            set => Set(ref _clientReceivedQty, value);
        }
        public string Comment
        {
            get => _comment;
            set => Set(ref _comment, value);
        }
        public int Weight
        {
            get => _weight;
            set => Set(ref _weight, value);
        }
        public int ServiceTypeId
        {
            get => _serviceTypeId;
            set => Set(ref _serviceTypeId, value);
        }
        public double Price
        {
            get => _price;
            set => Set(ref _price, value);
        }
        public int PriceUnit
        {
            get => _priceUnit;
            set => Set(ref _priceUnit, value);
        }

        public NoteRow OriginalObject { get; set; }
        public string Error { get; }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;


        public void SetName()
        {
            if (OriginalObject.LinenList != null)
            {
                LinenName = OriginalObject.LinenList.MasterLinen.Name;
            }
        }

        public void SetHasMasterLinen()
        {
            if (OriginalObject.LinenList.MasterLinen.DeletedDate == null)
            {
                HasMasterLinen = false;
            }
            else
            {
                HasMasterLinen = true;
            }
        }

        public NoteRowViewModel()
        {
            OriginalObject = new NoteRow();

        }

        public NoteRowViewModel(NoteRow originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(NoteRow noteRow)
        {
            OriginalObject = noteRow;

            Id = OriginalObject.Id;
            NoteHeaderId = OriginalObject.NoteHeaderId;
            LinenListId = OriginalObject.LinenListId;
            PrimeCollectedQty = OriginalObject.PrimeCollectionQty;
            PrimeDeliveredQty = OriginalObject.PrimeDeliveryQty;
            ClientReceivedQty = OriginalObject.ClientReceivedQty;
            Comment = OriginalObject.Comment;
            Weight = OriginalObject.Weight;
            ServiceTypeId = OriginalObject.ServiceTypeId;
            Price = OriginalObject.Price;
            PriceUnit = OriginalObject.PriceUnit;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.NoteHeaderId = NoteHeaderId != default(int) ? NoteHeaderId : OriginalObject.NoteHeader?.Id ?? 0;
            OriginalObject.LinenListId =LinenListId ;
            OriginalObject.PrimeCollectionQty = PrimeCollectedQty;
            OriginalObject.PrimeDeliveryQty = PrimeDeliveredQty;
            OriginalObject.ClientReceivedQty = ClientReceivedQty;
            OriginalObject.Comment = Comment;
            OriginalObject.Weight = Weight;
            OriginalObject.ServiceTypeId = ServiceTypeId;
            OriginalObject.Price = Price;
            OriginalObject.PriceUnit = PriceUnit;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    NoteHeaderId != OriginalObject.NoteHeaderId ||
                                    LinenListId != OriginalObject.LinenListId ||
                                    PrimeCollectedQty != OriginalObject.PrimeCollectionQty ||
                                    PrimeDeliveredQty != OriginalObject.PrimeDeliveryQty ||
                                    ClientReceivedQty != OriginalObject.ClientReceivedQty ||
                                    Comment != OriginalObject.Comment ||
                                    Weight != OriginalObject.Weight ||
                                    ServiceTypeId != OriginalObject.ServiceTypeId ||
                                    !Equals(Price, OriginalObject.Price) ||
                                    PriceUnit != OriginalObject.PriceUnit;
                                    

        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            return null;
        }
    }
}
