using PALMS.Invoices.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.View.Window
{
    [HasViewModel(typeof(DepartmentDetailsViewModel))]
    public partial class DepartmentDetailsWindow
    {
        public DepartmentDetailsWindow()
        {
            InitializeComponent();
        }
    }
}
