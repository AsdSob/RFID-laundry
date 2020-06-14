using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Views.Windows
{
    [HasViewModel(typeof(ClientLinenWindowModel))]
    public partial class ClientLinenWindow : CustomWindow
    {
        public ClientLinenWindow()
        {
            InitializeComponent();
        }
    }
}
