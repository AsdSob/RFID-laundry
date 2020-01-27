using System.ComponentModel;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Common.EntityViewModels.Abstract
{
    public class EntityViewModel : ViewModelBase
    {
        private EntityBase _originalObject;
        private int _id;
        public string Error { get; set; }

        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }

        public EntityBase OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public EntityViewModel()
        {
            //OriginalObject = new EntityBase();
        }

        public EntityViewModel(EntityBase originalObject) : this()
        {
            Update(originalObject);

        }

        public void Refresh()
        {
            Update(OriginalObject);
        }

        private void Update(EntityBase originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew;
    }
}
