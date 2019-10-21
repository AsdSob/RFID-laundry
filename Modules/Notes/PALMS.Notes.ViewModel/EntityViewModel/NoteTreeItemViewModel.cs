using PALMS.Data.Objects.EntityModel;
using PALMS.ViewModels.Common.Tree;

namespace PALMS.Notes.ViewModel.EntityViewModel
{
    public class NoteTreeItemViewModel : TreeItemViewModel
    {
        private int _clientId;
        private string _collectionDate;
        private string _collectionWeight;
        private string _deliveryDate;
        private string _deliveryWeight;
        private int _noteTreeStatus;
        private int _departmentId;
        private int _deliveryTypeId;
        private double _expressCharge;

        public bool IsSelected { get; set; }
        public double ExpressCharge
        {
            get => _expressCharge;
            set => Set(ref _expressCharge, value);
        }
        public int DeliveryTypeId
        {
            get => _deliveryTypeId;
            set => Set(ref _deliveryTypeId, value);
        }
        public int DepartmentId
        {
            get => _departmentId;
            set => Set(ref _departmentId, value);
        }
        public int NoteTreeStatus
        {
            get => _noteTreeStatus;
            set => Set(ref _noteTreeStatus, value);
        }
        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public string CollectionDate
        {
            get => _collectionDate;
            set => Set(ref _collectionDate, value);
        }

        public string DeliveryDate
        {
            get => _deliveryDate;
            set => Set(ref _deliveryDate, value);
        }

        public string CollectionWeight
        {
            get => _collectionWeight;
            set => Set(ref _collectionWeight, value);
        }

        public string DeliveryWeight
        {
            get => _deliveryWeight;
            set => Set(ref _deliveryWeight, value);
        }


        public NoteTreeItemViewModel(INameEntity entity) : base(entity)
        {
        }
    }
}
