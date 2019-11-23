using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;

namespace PALMS.ViewModels.EntityViewModel
{
    /// <summary>
    /// Класс обёртка для доступа к обёртке энтити
    /// </summary>
    public class ClientInfoViewModel: ViewModelBase, IDataErrorInfo
    {
        private int _clientId;
        private int? _ticketId;
        private int? _noteId;
        private int? _invoiceId;
        private string _comment;
        private string _address;
        private double _express;
        private int _start;
        private int _end;
        private double _weightPrice;
        private string _tRNNumber;
        private BitmapImage _logo;
        private byte[] _logoBytes;

        public ClientInfo OriginalObject { get; set; }
        public string Error { get; set; }

        public string TRNNumber
        {
            get => _tRNNumber;
            set => Set(ref _tRNNumber, value);
        }
        public double WeightPrice
        {
            get => _weightPrice;
            set => Set(ref _weightPrice, value);
        }
        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int? TicketId
        {
            get => _ticketId;
            set => Set(ref _ticketId, value);
        }
        public int? NoteId
        {
            get => _noteId;
            set => Set(ref _noteId, value);
        }
        public int? InvoiceId
        {
            get => _invoiceId;
            set => Set(ref _invoiceId, value);
        }
        public string Comment
        {
            get => _comment;
            set => Set(ref _comment, value);
        }
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }
        public double Express
        {
            get => _express;
            set => Set(ref _express, value);
        }
        public int Start
        {
            get => _start;
            set => Set(ref _start, value);
        }
        public int End
        {
            get => _end;
            set => Set(ref _end, value);
        }
        public BitmapImage Logo
        {
            get => _logo;
            set => Set(ref _logo, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientInfoViewModel()
        {
            OriginalObject = new ClientInfo();
        }

        public ClientInfoViewModel(ClientInfo clientInfo)
        {
            OriginalObject = clientInfo;
            ClientId = OriginalObject.Id;
            TicketId = OriginalObject.TicketId;
            NoteId = OriginalObject.NoteId;
            InvoiceId = OriginalObject.InvoiceId;
            Comment = OriginalObject.Comment;
            Address = OriginalObject.Address;
            Start = OriginalObject.Start;
            Express = OriginalObject.Express;
            End = OriginalObject.End;
            WeightPrice = OriginalObject.WeighPrice;
            TRNNumber = OriginalObject.TRNNumber;
            Logo = Extension.GetBitmapImage(OriginalObject.Logo);
            _logoBytes = OriginalObject.Logo;
        }

        public bool HasChanges()
        {
            var originalObject = OriginalObject;

            return originalObject.IsNew ||
                   originalObject.Address != Address ||
                   originalObject.Comment != Comment ||
                   originalObject.TicketId != TicketId ||
                   originalObject.NoteId != NoteId ||
                   originalObject.InvoiceId != InvoiceId ||
                   !Equals(originalObject.Express, Express) ||
                   originalObject.Start != Start ||
                   originalObject.End != End ||
                   !Equals(originalObject.WeighPrice, WeightPrice) ||
                   originalObject.TRNNumber != TRNNumber ||
                   !Equals(_logoBytes, OriginalObject.Logo);
        }

        public void AcceptChanges(Client client)
        {
            OriginalObject.Id = client.Id;
            OriginalObject.Address = Address;
            OriginalObject.TicketId = TicketId ?? default(int);
            OriginalObject.NoteId = NoteId ?? default(int);
            OriginalObject.InvoiceId = InvoiceId ?? default(int);
            OriginalObject.Comment = Comment;
            OriginalObject.Start = Start;
            OriginalObject.Express = Express;
            OriginalObject.End = End;
            OriginalObject.WeighPrice = WeightPrice;
            OriginalObject.TRNNumber = TRNNumber;
            OriginalObject.Logo = _logoBytes;
        }

        public string this[string columnName] => Validate(columnName);
        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(Address))
            {
                if (!Address.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(TicketId))
            {
                if (!TicketId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(NoteId))
            {
                if (!NoteId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(InvoiceId))
            {
                if (!InvoiceId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(Express))
            {
                if (!Express.ValidateRequired(out error))
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
