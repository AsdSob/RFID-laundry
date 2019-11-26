using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class MasterLinenEntityViewModel : ViewModelBase
    {
        private MasterLinen _originalObject;
        private int _id;
        private string _name;
        private int _packingValue;

        public int PackingValue
        {
            get => _packingValue;
            set => Set(() => PackingValue, ref _packingValue, value);
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
        public MasterLinen OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public MasterLinenEntityViewModel()
        {
            OriginalObject = new MasterLinen();
        }

        public MasterLinenEntityViewModel(MasterLinen originalObject) : this()
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
            PackingValue = OriginalObject.PackingValue;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.PackingValue = PackingValue;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(PackingValue, OriginalObject.PackingValue);
    }
}
