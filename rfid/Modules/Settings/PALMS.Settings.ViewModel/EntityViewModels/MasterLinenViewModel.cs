using GalaSoft.MvvmLight;
using PALMS.Data.Objects;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class MasterLinenViewModel: ViewModelBase
    {
        private MasterLinen _originalObject;
        private int _id;
        private string _name;

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
        public MasterLinen OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public MasterLinenViewModel()
        {
            OriginalObject = new MasterLinen();
        }

        public MasterLinenViewModel(MasterLinen originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(MasterLinen originalObject)
        {
            OriginalObject = originalObject;

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
