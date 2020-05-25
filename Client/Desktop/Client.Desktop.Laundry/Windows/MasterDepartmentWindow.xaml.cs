using System.Windows.Controls;
using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Laundry.Windows
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
