using System.Windows.Controls;
using PALMS.Settings.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.View.Windows
{
    /// <summary>
    /// Interaction logic for AddNewLinenWindow.xaml
    /// </summary>
    [HasViewModel(typeof(AddNewLinenViewModel))]
    public partial class AddNewLinenWindow
    {
        public AddNewLinenWindow()
        {
            InitializeComponent();
        }
    }
}
