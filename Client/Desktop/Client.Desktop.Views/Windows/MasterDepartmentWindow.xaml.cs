using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Views.Windows
{
    [HasViewModel(typeof(MasterDepartmentWindowModel))]

    public partial class MasterDepartmentWindow : CustomWindow
    {
        public MasterDepartmentWindow()
        {
            InitializeComponent();
        }
    }
}
