using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.LinenModel;

namespace PALMS.LinenList.ViewModel
{
    public class MasterLinensViewModel : ViewModelBase
    {
        private MasterLinen _originalObject;
        private int _id;
        private int _typeLinenId;
        private string _name;
        private int _familyLinenId;
        private int _masterLinenId;
        private int? _weight;

        public int? Weight
        {
            get => _weight;
            set => Set(ref _weight, value);
        }
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


        public MasterLinensViewModel()
        {
            
        }

        public MasterLinensViewModel(MasterLinen entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Name = OriginalObject.Name;
            Id = OriginalObject.Id;
            FamilylinenId = OriginalObject.FamilyLinenId;
            TypeleLinenId = OriginalObject.LinenTypeId;
            Weight = OriginalObject.Weight;

        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null || OriginalObject.IsNew;

    }
}
