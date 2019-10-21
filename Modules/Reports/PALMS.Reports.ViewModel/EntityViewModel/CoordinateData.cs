using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;

namespace PALMS.Reports.ViewModel.EntityViewModel
{

    public class NoteHeaderCoordinate : CoordinateBase
    {
        private NoteHeader _originalObject;

        public NoteHeader OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }
        public NoteHeaderCoordinate(NoteHeader entity)
        {
            OriginalObject = entity;
        }
    }


    public class CoordinateBase : ViewModelBase
    {
        private int _id;
        private string _name;
        private double _value;

        public double Value
        {
            get => _value;
            set => Set(ref _value, value);
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
    }
}
