using System.Windows.Controls;
using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Laundry.Windows
{

    [HasViewModel(typeof(StaffChangeWindowModel))]

    public partial class StaffChangeWindow : CustomWindow
    {
        public StaffChangeWindow()
        {
            InitializeComponent();
        }
    }
}
