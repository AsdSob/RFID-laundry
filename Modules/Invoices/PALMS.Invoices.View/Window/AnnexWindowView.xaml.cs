using PALMS.Invoices.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.View.Window
{
    /// <summary>
    /// Interaction logic for AnnexWindowView.xaml
    /// </summary>
    [HasViewModel(typeof(AnnexWindowViewModel))]
    /// 
    public partial class AnnexWindowView
    {
        public AnnexWindowView()
        {
            InitializeComponent();
        }
    }
}
