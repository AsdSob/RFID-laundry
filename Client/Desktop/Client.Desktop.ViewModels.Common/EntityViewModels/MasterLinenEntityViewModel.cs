using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
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
        private bool _isValid;
        private string _error;

        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }
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

            PropertyChanged += OnPropertyChanged;
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

        public string this[string columnName] => Validate(columnName);


        private string Validate(string columnName)
        {
            var error = String.Empty;

            if (columnName == nameof(Name))
            {
                Name.ValidateRequired(ref error);
                Name.ValidateByNameMaxLength(ref error);
            }
            else

            if (columnName == nameof(PackingValue))
            {
                PackingValue.ValidateRequired(ref error);
                PackingValue.ValidateMinAmount(ref error);
            }

            FullValidate(columnName);

            return error;
        }

        private void FullValidate(string columnName)
        {
            var error = String.Empty;

            Name.ValidateRequired(ref error);
            Name.ValidateByNameMaxLength(ref error);

            PackingValue.ValidateRequired(ref error);
            PackingValue.ValidateMinAmount(ref error);

            Error = error;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Error))
            {
                IsValid = String.IsNullOrWhiteSpace(Error);
            }

            if (e.PropertyName == nameof(Name))
            {
                if (!String.IsNullOrEmpty(Name))
                {
                    var regex = new Regex(@"\s+");
                    Name = regex.Replace(Name, " ");
                }
            }

            //if (e.PropertyName == nameof(PackingValue))
            //{
            //    PackingValue = Convert.ToInt32(Regex.Replace(PackingValue.ToString(), "[^0-9]", ""));
            //}
        }
    }
}
