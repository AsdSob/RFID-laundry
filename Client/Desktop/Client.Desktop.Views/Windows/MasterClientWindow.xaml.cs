using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Views.Windows
{
    [HasViewModel(typeof(MasterClientWindowModel))]

    public partial class MasterClientWindow : CustomWindow

    {
        public MasterClientWindow()
        {
            InitializeComponent();
        }
    }
}
