using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace PALMS.LinenList.ViewModel
{
    public class LinenListModelViewModel : ViewModelBase, IDataErrorInfo
    {
        private Data.Objects.LinenModel.LinenList _originalObject;
        private int _id;
        private int _clientId;
        private int _masterLinenId;
        private double _laundry;
        private double _dryCleaning;
        private double _pressing;
        private int _weight;
        private bool _active;
        private int _departmentId;
        private int _unitTypeId;

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public Data.Objects.LinenModel.LinenList OriginalObject { get; set; }

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
        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int MasterLinenId
        {
            get => _masterLinenId;
            set => Set(ref _masterLinenId, value);
        }
        public double Laundry
        {
            get => _laundry;
            set => Set(ref _laundry, value);
        }
        public double DryCleaning
        {
            get => _dryCleaning;
            set => Set(ref _dryCleaning, value);
        }
        public double Pressing
        {
            get => _pressing;
            set => Set(ref _pressing, value);
        }
        public int Weight
        {
            get => _weight;
            set => Set(ref _weight, value);
        }
        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }
        public int UnitTypeId
        {
            get => _unitTypeId;
            set => Set(ref _unitTypeId, value);
        }

        public string Error { get; set; }

        public Func<LinenListModelViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        public string this[string columnName] => Validate(columnName);

        public LinenListModelViewModel()
        {
            OriginalObject = new Data.Objects.LinenModel.LinenList();
        }

        public LinenListModelViewModel(Data.Objects.LinenModel.LinenList entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            MasterLinenId = OriginalObject.MasterLinenId;
            Laundry = OriginalObject.Laundry;
            DryCleaning = OriginalObject.DryCleaning;
            Pressing = OriginalObject.Pressing;
            Weight = OriginalObject.Weight;
            Active = OriginalObject.Active;
            DepartmentId = OriginalObject.DepartmentId;
            UnitTypeId = OriginalObject.UnitId;
        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Id != OriginalObject.Id ||
                                    ClientId != OriginalObject.ClientId ||
                                    MasterLinenId != OriginalObject.MasterLinenId ||
                                    !Equals(Laundry, OriginalObject.Laundry) ||
                                    !Equals(DryCleaning, OriginalObject.DryCleaning) ||
                                    !Equals(Pressing, OriginalObject.Pressing) ||
                                    !Equals(Weight, OriginalObject.Weight) ||
                                    Active != OriginalObject.Active ||
                                    UnitTypeId != OriginalObject.UnitId;

        public bool Validate(bool isPricePerUnit)
        {
            if (isPricePerUnit)
            {
                return (string.IsNullOrEmpty(Validate(nameof(Weight))) &&
                        string.IsNullOrEmpty(Validate(nameof(Laundry))) &&
                        string.IsNullOrEmpty(Validate(nameof(Pressing))) &&
                        string.IsNullOrEmpty(Validate(nameof(DryCleaning))));
            }
            return (string.IsNullOrEmpty(Validate(nameof(Weight))));

        }

        public string Validate(string columnName)
        {
            string error;

            //if (columnName == nameof(WeightDelivered))
            //{
            //    if (!WeightDelivered.ValidateRequired(out error) )
            //        return error;
            //}
            //else if (columnName == nameof(Laundry))
            //{
            //    if (LaundryRequiedValidationFunc == null || LaundryRequiedValidationFunc())
            //    {
            //        if (!Laundry.ValidateRequired(out error))
            //            return error;
            //    }
            //}
            //else if (columnName == nameof(Pressing))
            //{
            //    if (!Pressing.ValidateRequired(out error))
            //        return error;
            //}
            //else if (columnName == nameof(DryCleaning))
            //{
            //    if (!DryCleaning.ValidateRequired(out error))
            //        return error;
            //}

            return null;
        }

        public void AcceptChanges()
        {
            OriginalObject.ClientId = ClientId;
            OriginalObject.MasterLinenId = MasterLinenId;
            OriginalObject.Laundry = Laundry;
            OriginalObject.DryCleaning = DryCleaning;
            OriginalObject.Pressing = Pressing;
            OriginalObject.Weight = Weight;
            OriginalObject.Active = Active;
            OriginalObject.DepartmentId = DepartmentId;
            OriginalObject.UnitId = UnitTypeId;
        }
    }
}
