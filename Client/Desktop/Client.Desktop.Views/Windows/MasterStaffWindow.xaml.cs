using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Views.Windows
{
    [HasViewModel(typeof(MasterStaffWindowModel))]
    public partial class MasterStaffWindow : CustomWindow
    {
        public MasterStaffWindow()
        {
            InitializeComponent();
        }
    }
}
