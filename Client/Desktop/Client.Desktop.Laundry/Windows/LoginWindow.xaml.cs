using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Laundry.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    [HasViewModel(typeof(LoginWindowViewModel))]
    public partial class LoginWindow : CustomWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
    }
}
