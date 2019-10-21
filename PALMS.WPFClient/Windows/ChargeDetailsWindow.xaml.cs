using PALMS.ViewModels;
using PALMS.ViewModels.Common;

namespace PALMS.WPFClient.Windows
{
    [HasViewModel(typeof(ChargeDetailsViewModel))]
    public partial class ChargeDetailsWindow
    {
        public ChargeDetailsWindow()
        {
            InitializeComponent();
        }
    }
}
