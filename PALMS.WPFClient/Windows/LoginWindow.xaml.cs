using PALMS.ViewModels.Common;
using PALMS.ViewModels.WindowViewModel;

namespace PALMS.WPFClient.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    [HasViewModel(typeof(LoginViewModel))]
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
    }
}
