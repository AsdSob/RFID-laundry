using GalaSoft.MvvmLight;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.BuilderSettings
{
    public class AnnexTemp : ViewModelBase
    {
        private Cell _dayLast;
        private Cell _dayFirst;
        private Group[] _groups;
        private int _lastColumn;

        public int LastColumn
        {
            get => _lastColumn;
            set => Set(ref _lastColumn, value);
        }
        public Cell DayFirst
        {
            get => _dayFirst;
            set => Set(ref _dayFirst, value);
        }
        public Cell DayLast
        {
            get => _dayLast;
            set => Set(ref _dayLast, value);
        }

        public Group[] Groups
        {
            get => _groups;
            set => Set(ref _groups, value);
        }

    }

    public class Group : ViewModelBase
    {
        private Cell _groupFirst;
        private Cell _groupLast;

        public Cell GroupLast
        {
            get => _groupLast;
            set => Set(ref _groupLast, value);
        }
        public Cell GroupFirst
        {
            get => _groupFirst;
            set => Set(ref _groupFirst, value);
        }
    }

}
