using GalaSoft.MvvmLight;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class PackedLinenViewModel : ViewModelBase
    {
        private Entity _originalObject;
        private string _name;
        private int _id;
        private int? _parentId;
        private int _orderNumber;


        public int OrderNumber
        {
            get => _orderNumber;
            set => Set(() => OrderNumber, ref _orderNumber, value);
        }
        public int? ParentId
        {
            get => _parentId;
            set => Set(() => ParentId, ref _parentId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }
        public Entity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }
    }
}
