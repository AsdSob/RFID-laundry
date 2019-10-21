using PALMS.ViewModels;
using PALMS.ViewModels.Common;

namespace PALMS.WPFClient.Windows
{
    [HasViewModel(typeof(DepartmentDetailsViewModel))]
    public partial class DepartmentDetailsWindow
    {
        public DepartmentDetailsWindow()
        {
            InitializeComponent();
        }
    }
}
