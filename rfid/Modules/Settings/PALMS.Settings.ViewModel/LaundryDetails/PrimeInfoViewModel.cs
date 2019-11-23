using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.ViewModel.LaundryDetails
{
    public class PrimeInfoViewModel : ViewModelBase
    {
        private double _vAT;
        private int _id;
        private string _name;
        private string _tRNnumber;
        private string _address;
        private BitmapImage _logo;
        private byte[] _logoBytes;

        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }
        public string TRNnumber
        {
            get => _tRNnumber;
            set => Set(ref _tRNnumber, value);
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

        public double VAT
        {
            get => _vAT;
            set => Set(ref _vAT, value);
        }

        public BitmapImage Logo
        {
            get => _logo;
            set => Set(ref _logo, value);
        }
        public PrimeInfo OriginalObject { get; set; }
        public string Error { get; }
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public PrimeInfoViewModel()
        {
            //OriginalObject = new PrimeInfo();
        }

        public PrimeInfoViewModel(PrimeInfo primeInfo) : this()
        {
            Update(primeInfo);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }
        

        private void Update(PrimeInfo primeInfo)
        {
            OriginalObject = primeInfo;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            Address = OriginalObject.Address;
            VAT = OriginalObject.VAT;
            TRNnumber = OriginalObject.TRNNumber;
            Logo = Extension.GetBitmapImage(OriginalObject.Logo);
            _logoBytes = OriginalObject.Logo;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.VAT = VAT;
            OriginalObject.TRNNumber = TRNnumber;
            OriginalObject.Address = Address;
            OriginalObject.Logo = _logoBytes;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(VAT, OriginalObject.VAT) ||
                                    Address != OriginalObject.Address ||
                                    TRNnumber != OriginalObject.TRNNumber ||
                                    Name != OriginalObject.Name ||
                                    !Equals(_logoBytes, OriginalObject.Logo);

        public string this[string columnName] => Validate(columnName);
        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(VAT))
            {
                if (!VAT.ValidateRequired(out error))
                    return error;
            }

            return null;
        }

        public void SetLogoBytes(byte[] logBytes)
        {
            _logoBytes = logBytes;
        }
    }
}
