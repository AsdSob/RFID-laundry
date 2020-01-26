using System;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class MasterLinenEntityViewModel :ViewModelBase, IDataErrorInfo
    {
        private MasterLinenEntity _originalObject;
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
            set => Set(() => Name, ref _name, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public MasterLinenEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public MasterLinenEntityViewModel()
        {
            OriginalObject = new MasterLinenEntity();
        }

        public MasterLinenEntityViewModel(MasterLinenEntity entity) : this()

        {
            Update(entity);
        }

        public void Update(MasterLinenEntity originalObject)
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

        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);

        public Func<MasterLinenEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(PackingValue))
            {
                if (!PackingValue.ValidateRequired(out error) ||
                    !PackingValue.ValidateMinAmount(out error))
                {
                    return error;
                }

            }

            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error) ||
                    !Name.ValidateBySpaces(out error))
                {
                    return error;
                }

                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(Name)))
                {
                    return "Staff Id is already exist";
                }
            }
            return null;
        }
    }
}
