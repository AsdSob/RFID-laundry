using PALMS.Settings.ViewModel.AppSettings;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.View.AppSettings
{
    /// <summary>
    /// Interaction logic for AppSettingsView.xaml
    /// </summary>
    [HasViewModel(typeof(AppSettingsViewModel))]
    public partial class AppSettingsView
    {
        public AppSettingsView()
        {
            InitializeComponent();
        }
    }
}
