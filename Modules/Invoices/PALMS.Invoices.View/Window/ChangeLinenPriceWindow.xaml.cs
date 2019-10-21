using PALMS.Invoices.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.View.Window
{
    /// <summary>
    /// Interaction logic for ChangeLinenPriceWindow.xaml
    /// </summary>
    ///

    [HasViewModel(typeof(ChangeLinenPriceViewModel))]
    public partial class ChangeLinenPriceWindow
    {
        public ChangeLinenPriceWindow()
        {
            InitializeComponent();
        }
    }
}
