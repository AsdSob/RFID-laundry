using GalaSoft.MvvmLight;

namespace PALMS.LinenList.ViewModel.EntityViewModel
{
    public class UsedLinenViewModel : ViewModelBase
    {
        private bool _isSelected;
        private double _numberOfUsage;
        private string _name;
        private Data.Objects.LinenModel.LinenList _originalObject;
        private int _id;
        private int _departmentId;
        private bool _active;
        private bool _isMasterLinenDeleted;

        public bool IsMasterLinenDeleted
        {
            get => _isMasterLinenDeleted;
            set => Set(ref _isMasterLinenDeleted, value);
        }
        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }
        public int DepartmentId
        {
            get => _departmentId;
            set => Set(ref _departmentId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public Data.Objects.LinenModel.LinenList OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public double NumberOfUsage
        {
            get => _numberOfUsage;
            set => Set(ref _numberOfUsage, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public UsedLinenViewModel(Data.Objects.LinenModel.LinenList linenList)
        {
            OriginalObject = linenList;

            Name = OriginalObject.MasterLinen.Name;
            Id = OriginalObject.Id;
            DepartmentId = OriginalObject.DepartmentId;
            Active = OriginalObject.Active;
            IsMasterLinenDeleted = OriginalObject.MasterLinen.DeletedDate.HasValue;
        }
    }
}
