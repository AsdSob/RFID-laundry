using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.Reports.ViewModel.EntityViewModel
{
    public class UnitReportViewModel : UnitViewModel
    {
        private bool _isSelected;
        //private int _id;
        //private string _name;

        //public string Name
        //{
        //    get => _name;
        //    set => Set(ref _name, value);
        //}
        //public int Id
        //{
        //    get => _id;
        //    set => Set(ref _id, value);
        //}

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public UnitReportViewModel(int id, string name) : base(id, name)
        {
        }
    }
}
