using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.LinenModel;
using PALMS.ViewModels.Common;

namespace PALMS.ViewModels.EntityViewModel
{
    public class MasterLinenViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _linenId;
        private int _masterLinenID;
        private string _name;
        private int? _groupLinen;
        private int? _typeLinen;
        private int? _familyLinen;

        public string Error { get; set; }
        public MasterLinen OriginalObject { get; set; }
        public int LinenId
        {
            get => _linenId;
            set => Set(ref _linenId, value);
        }
        public int MasterLinenId
        {
            get => _masterLinenID;
            set => Set(ref _masterLinenID, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public int? GroupLinen
        {
            get => _groupLinen;
            set => Set(ref _groupLinen, value);
        }
        public int? TypeLinen
        {
            get => _typeLinen;
            set => Set(ref _typeLinen, value);
        }
        public int? FamilyLinen
        {
            get => _familyLinen;
            set => Set(ref _familyLinen, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public MasterLinenViewModel()
        {
            OriginalObject = new MasterLinen();
        }

        public MasterLinenViewModel(MasterLinen masterLinen) : this()
        {
            Update(masterLinen);
        }

        public MasterLinenViewModel Clone()
        {
            return new MasterLinenViewModel(OriginalObject);
        }

        public void Update(MasterLinenViewModel masterLinen)
        {
            Update(masterLinen.OriginalObject);
        }

        public string this[string columnName] => Validate(columnName);
        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error) ||
                    !Name.ValidateBySpaces(out error) ||
                    !Name.ValidateByMaxLength(out error))
                {
                    return error;
                }
            }

            return null;
        }

        private void Update(MasterLinen masterLinen)
        {
            OriginalObject = masterLinen;

            Name = OriginalObject.Name;
            LinenId = OriginalObject.Id;
            FamilyLinen = OriginalObject.FamilyLinenId;
            TypeLinen = OriginalObject.LinenTypeId;
            GroupLinen = OriginalObject.GroupLinenId;
        }
    }
}
