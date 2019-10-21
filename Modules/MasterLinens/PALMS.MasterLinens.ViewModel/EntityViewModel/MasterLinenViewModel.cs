using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.LinenModel;
using PALMS.ViewModels.Common;

namespace PALMS.MasterLinens.ViewModel
{
    public class MasterLinenViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _id;
        private string _name;
        private int _familyLinenId;
        private int _groupLinenId;
        private int _typeLinenId;
        private readonly object _notifyingObjectIsChangedSyncRoot = new object();
        private bool _notifyingObjectIsChanged;
        private MasterLinen _originalObject;
        private int? _weight;

        public Func<MasterLinenViewModel, string, bool> NameUniqueValidationFunc { get; set; }
        public MasterLinen OriginalObject
        {
            get { return _originalObject; }
            set { _originalObject = value; }
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public int FamilyLinenId
        {
            get => _familyLinenId;
            set => Set(ref _familyLinenId, value);
        }
        public int GroupLinenId
        {
            get => _groupLinenId;
            set => Set(ref _groupLinenId, value);
        }
        public int TypeLinenId
        {
            get => _typeLinenId;
            set => Set(ref _typeLinenId, value);
        }

        public int? Weight
        {
            get => _weight;
            set => Set(ref _weight, value);
        }

        public string Error { get; set; }

        public MasterLinenViewModel()
        {
            OriginalObject = new MasterLinen();
            PropertyChanged += OnPropertyChanged;
        }

        public MasterLinenViewModel(MasterLinen entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            FamilyLinenId = OriginalObject.FamilyLinenId;
            GroupLinenId = OriginalObject.GroupLinenId;
            TypeLinenId = OriginalObject.LinenTypeId;
            Weight = OriginalObject.Weight;

            PropertyChanged += OnPropertyChanged;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Name != OriginalObject.Name ||
                                    TypeLinenId != OriginalObject.LinenTypeId ||
                                    GroupLinenId != OriginalObject.GroupLinenId ||
                                    FamilyLinenId != OriginalObject.FamilyLinenId ||
                                    Weight != OriginalObject.Weight;

        public bool IsChanged
        {
            get
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    return _notifyingObjectIsChanged;
                }
            }

            protected set
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    Set(ref _notifyingObjectIsChanged, value);
                }
            }
        }
        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(MasterLinen originalObject)
        {
            OriginalObject = originalObject;

            if (OriginalObject == null) return;

            //Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            FamilyLinenId = OriginalObject.FamilyLinenId;
            TypeLinenId = OriginalObject.LinenTypeId;
            GroupLinenId = OriginalObject.GroupLinenId;
            Weight = OriginalObject.Weight;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = RemoveDoubleSpace(Name);
            OriginalObject.FamilyLinenId = FamilyLinenId;
            OriginalObject.GroupLinenId = GroupLinenId;
            OriginalObject.LinenTypeId = TypeLinenId;
            OriginalObject.Weight = Weight;
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
                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc.Invoke(this, nameof(Name)))
                {
                    return "Name is already exist";
                }
            }
            else if (columnName == nameof(FamilyLinenId))
            {
                if (!FamilyLinenId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(GroupLinenId))
            {
                if (!GroupLinenId.ValidateRequired(out error))
                    return error;
            }

            return null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Name)) return;
            Name = Name?.ToUpper();
            Name = Name?.Trim();
            Name = Name?.RemoveDoubleSpace();
            Validate(Name);
        }

        public string RemoveDoubleSpace(string text)
        {
            text = text.Trim();
            return Regex.Replace(text, @"\s+", " ");
        }

    }
}