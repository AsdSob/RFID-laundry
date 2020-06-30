using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class DeliveryNoteRowEntityViewModel: ViewModelBase
    {
        private int _id;
        private int _clientLinenId;
        private int _collectionQuantity;
        private int _deliveryQuantity;
        private int _clientCollectionQuantity;
        private string _comment;
        private int _deliveryNoteId;

        public int DeliveryNoteId
        {
            get => _deliveryNoteId;
            set => Set(ref _deliveryNoteId, value);
        }
        public string Comment
        {
            get => _comment;
            set => Set(ref _comment, value);
        }
        public int ClientCollectionQuantity
        {
            get => _clientCollectionQuantity;
            set => Set(ref _clientCollectionQuantity, value);
        }
        public int DeliveryQuantity
        {
            get => _deliveryQuantity;
            set => Set(ref _deliveryQuantity, value);
        }
        public int CollectionQuantity
        {
            get => _collectionQuantity;
            set => Set(ref _collectionQuantity, value);
        }
        public int ClientLinenId
        {
            get => _clientLinenId;
            set => Set(ref _clientLinenId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
    }
}
