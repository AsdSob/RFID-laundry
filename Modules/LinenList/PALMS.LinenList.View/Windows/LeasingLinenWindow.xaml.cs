using PALMS.LinenList.ViewModel;
using PALMS.ViewModels.Common;

namespace PALMS.LinenList.View
{
    /// <summary>
    /// Interaction logic for LeasingLinenWindow.xaml
    /// </summary>
    [HasViewModel(typeof(LeasingLinenViewModel))]
    public partial class LeasingLinenWindow
    {
        public LeasingLinenWindow()
        {
            InitializeComponent();
        }
    }
}
