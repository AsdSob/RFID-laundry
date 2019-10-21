using GalaSoft.MvvmLight;
using PALMS.Data.Objects.Conveyor;

namespace PALMS.TPS.ViewModel.EntityViewModel
{
    public class CLinenViewModel : ViewModelBase
    {
        public CLinen _originalObject;

        public CLinen OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public int _id;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public CLinenViewModel()
        {
            OriginalObject = new CLinen();
        }

        public CLinenViewModel(CLinen linen) : this()
        {
            Update(linen);
            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(CLinen linen)
        {
            OriginalObject = linen;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name);
    }
}
