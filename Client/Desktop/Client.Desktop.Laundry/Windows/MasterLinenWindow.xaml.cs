using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Laundry.Windows
{
    [HasViewModel(typeof(MasterLinenWindowModel))]
    public partial class MasterLinenWindow : CustomWindow
    {
        public MasterLinenWindow()
        {
            InitializeComponent();
        }
    }
}
