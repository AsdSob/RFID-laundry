using System.ComponentModel;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class AccountDetailEntityViewModel : ViewModelBase, IDataErrorInfo
    {
        private AccountDetailsEntity _originalObject;
        private int _accountId;
        private int _readerId;
        private int _id;

        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public int ReaderId
        {
            get => _readerId;
            set => Set(() => ReaderId, ref _readerId, value);
        }
        public int AccountId
        {
            get => _accountId;
            set => Set(() => AccountId, ref _accountId, value);
        }
        public AccountDetailsEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;
        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);

        public AccountDetailEntityViewModel()
        {
            OriginalObject = new AccountDetailsEntity();

            PropertyChanged += OnPropertyChanged;
        }

        public AccountDetailEntityViewModel(AccountDetailsEntity originalObject) : this()
        {
            Update(originalObject);
        }

        private void Update(AccountDetailsEntity originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            AccountId = OriginalObject.AccountId;
            ReaderId = OriginalObject.ReaderId;
        }

        public void Refresh()
        {
            Update(OriginalObject);
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Id = Id;
            OriginalObject.AccountId = AccountId;
            OriginalObject.ReaderId = ReaderId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Id, OriginalObject.Id) ||
                                    !Equals(AccountId, OriginalObject.AccountId) ||
                                    !Equals(ReaderId, OriginalObject.ReaderId);

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private string Validate(string columnName)
        {
            return null;
        }
    }
}
