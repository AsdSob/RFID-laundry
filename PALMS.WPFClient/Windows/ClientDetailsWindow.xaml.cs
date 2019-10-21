using PALMS.ViewModels;
using PALMS.ViewModels.Common;

namespace PALMS.WPFClient.Windows
{
    /// <summary>
    /// Interaction logic for ClientDetailsWindow.xaml
    /// </summary>
    [HasViewModel(typeof(ClientDetailsViewModel))]
    public partial class ClientDetailsWindow
    {
        public ClientDetailsWindow()
        {
            InitializeComponent();
        }
    }
}
