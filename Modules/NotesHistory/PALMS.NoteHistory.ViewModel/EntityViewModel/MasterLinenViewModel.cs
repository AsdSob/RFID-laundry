using System;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.LinenModel;

namespace PALMS.NoteHistory.ViewModel.EntityViewModel
{
    public class MasterLinenViewModel : ViewModelBase
    {
        private MasterLinen _originalObject;
        private int _id;
        private int _typeLinenId;
        private string _name;
        private int _familyLinenId;
        private int _masterLinenId;

        public MasterLinen OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public int TypeleLinenId
        {
            get => _typeLinenId;
            set => Set(ref _typeLinenId, value);
        }
        public int FamilylinenId
        {
            get => _familyLinenId;
            set => Set(ref _familyLinenId, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public int MasterLinenId
        {
            get => _masterLinenId;
            set => Set(ref _masterLinenId, value);
        }


        public MasterLinenViewModel()
        {
            
        }

        public MasterLinenViewModel(MasterLinen entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Name = OriginalObject.Name;
            Id = OriginalObject.Id;
            FamilylinenId = OriginalObject.FamilyLinenId;
            TypeleLinenId = OriginalObject.LinenTypeId;

        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null || OriginalObject.IsNew;

    }
}
