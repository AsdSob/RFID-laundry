using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.LinenModel;

namespace PALMS.LinenList.ViewModel.EntityViewModel
{
    public class FullLeasingLinenViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _id;
        private string _name;
        private int _linenListId;
        private double _originalPrice;

        public Data.Objects.LinenModel.LinenList LinenList { get; set; }
        public string this[string columnName] => Validate(columnName);


        public string Error { get; set; }

        public double OriginalPrice
        {
            get => _originalPrice;
            set => Set(ref _originalPrice, value);
        }

        public int LinenListId
        {
            get => _linenListId;
            set => Set(ref _linenListId, value);
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

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public LeasingLinen OriginalObject { get; set; }

        public FullLeasingLinenViewModel()
        {
            OriginalObject = new LeasingLinen();
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(LeasingLinen noteRow)
        {
            OriginalObject = noteRow;

            Id = OriginalObject.Id;
            LinenListId = OriginalObject.LinenListId;
            Name = OriginalObject.Name;
            OriginalPrice = OriginalObject.OriginalPrice;
        }

        public FullLeasingLinenViewModel(LeasingLinen entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Id = OriginalObject.Id;
            LinenListId = OriginalObject.LinenListId;
            OriginalPrice = OriginalObject.OriginalPrice;
            Name = OriginalObject.Name;
        }

        public void AcceptChanges()
        {
            OriginalObject.Name = Name;
            OriginalObject.OriginalPrice = OriginalPrice;
            OriginalObject.LinenListId = LinenListId;
        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Id != OriginalObject.Id ||
                                    Name != OriginalObject.Name ||
                                    !Equals(OriginalPrice, OriginalObject.OriginalPrice) ||
                                    LinenListId != OriginalObject.LinenListId;

        public string Validate(string columnName)
        {
            string error;

            return null;
        }
    }
}
