using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class DeliveryNoteEntityViewModel: ViewModelBase
    {
        private int _id;
        private string _name;
        private string _comment;
        private int _departmentId;
        private int? _invoiceId;

        public int? InvoiceId
        {
            get => _invoiceId;
            set => Set(ref _invoiceId, value);
        }
        public int DepartmentId
        {
            get => _departmentId;
            set => Set(ref _departmentId, value);
        }
        public string Comment
        {
            get => _comment;
            set => Set(ref _comment, value);
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
    }
}
