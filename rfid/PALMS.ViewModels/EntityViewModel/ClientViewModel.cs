using System;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;

namespace PALMS.ViewModels.EntityViewModel
{
    /// <summary>
    /// Класс обёртка для доступа к обёртке энтити
    /// </summary>
    public class ClientViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _name;
        private int _id;
        private string _colour;
        private string _shortName;
        private bool _active;
        private ClientInfoViewModel _clientInfo;
        private InvoiceDetailsViewModel _invoiceDetails;
        private byte[] _logoBytes;

        public string Error { get; set; }

        public Client OriginalObject { get; set; }

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
        public string Colour
        {
            get => _colour;
            set => Set(ref _colour, value);
        }
        public string ShortName
        {
            get => _shortName;
            set => Set(ref _shortName, value);
        }
        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }
        public ClientInfoViewModel ClientInfo
        {
            get => _clientInfo;
            set => Set(ref _clientInfo, value);
        }
        public InvoiceDetailsViewModel InvoiceDetail
        {
            get => _invoiceDetails;
            set => Set(ref _invoiceDetails, value);
        }

        public Func<ClientViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        /// <summary>
        /// IsNew Присваиваем значение true или false IsNew, OriginalObject равен или (||) IsNew true or false
        /// </summary>
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        /// <summary>
        /// Ctor.
        /// </summary>
        public ClientViewModel()
        {
            OriginalObject = new Client();
            ClientInfo = new ClientInfoViewModel();
            InvoiceDetail = new InvoiceDetailsViewModel();

            PropertyChanged += OnPropertyChanged;
        }

        public ClientViewModel(Client client) : this()
        {
            Update(client);
        }

        public ClientViewModel Clone()
        {
            return new ClientViewModel(OriginalObject);
        }

        public void Update(ClientViewModel client)
        {
            Update(client.OriginalObject);
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
            
                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(Name)))
                {
                    return "Name is already exist";
                }
            }
            else if (columnName == nameof(ShortName))
            {
                if (!ShortName.ValidateRequired(out error) ||
                    !ShortName.ValidateBySpaces(out error) ||
                    !ShortName.ValidateByShortNameLength(out error))

                {
                    return error;
                }

                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(ShortName)))
                {
                    return "Short Name is already exist";
                }
            }

            return null;
        }

        private void Update(Client client)
        {
            OriginalObject = client;

            Name = OriginalObject.Name;
            Active = OriginalObject.Active;
            Colour = OriginalObject.Color;
            ShortName = OriginalObject.ShortName;
            Id = OriginalObject.Id;

            if (client.ClientInfo != null)
                ClientInfo = new ClientInfoViewModel(client.ClientInfo);

            if (client.TaxAndFees?.Any() == true)
                InvoiceDetail = new InvoiceDetailsViewModel(client.TaxAndFees.Last());
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name.RemoveDoubleSpace();
            OriginalObject.ShortName = ShortName.RemoveDoubleSpace();
            OriginalObject.Active = Active;
            OriginalObject.Color = Colour;
            
        }

        /// <summary>
        /// Проверка есть ли изменения в классе
        /// </summary>
        /// <param name="client"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Name != OriginalObject.Name ||
                                    ShortName != OriginalObject.ShortName ||
                                    Active != OriginalObject.Active ||
                                    Colour != OriginalObject.Color;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name))
            {
                Name = Name?.ToUpper();
            }
            else if (e.PropertyName == nameof(ShortName))
            {
                ShortName = ShortName?.ToUpper();
            }
        }

    }
}