using System.Windows;
using PALMS.Invoices.ViewModel;
using PALMS.Invoices.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.View
{
    [HasViewModel(typeof(ChargeViewModel))]
    public partial class ChargeWindow
    {
        public ChargeWindow()
        {
            InitializeComponent();
        }
    }
}
