using PALMS.Settings.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.View.Windows
{
    /// <summary>
    /// Interaction logic for ReadTagWindow.xaml
    /// </summary>
    
    [HasViewModel(typeof(ReadTagWindowViewModel))]
    public partial class ReadTagWindow
    {
        public ReadTagWindow()
        {
            InitializeComponent();
        }
    }
}
