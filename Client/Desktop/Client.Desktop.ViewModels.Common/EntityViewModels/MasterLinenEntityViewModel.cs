using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class MasterLinenEntityViewModel :ViewModelBase
    {
        private MasterLinenEntity _originalObject;
        private int _id;
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public MasterLinenEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public MasterLinenEntityViewModel()
        {
            OriginalObject = new MasterLinenEntity();
        }

        public MasterLinenEntityViewModel(MasterLinenEntity entity) : this()

        {
            Update(entity);
        }

        public void Update(MasterLinenEntity originalObject)
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
