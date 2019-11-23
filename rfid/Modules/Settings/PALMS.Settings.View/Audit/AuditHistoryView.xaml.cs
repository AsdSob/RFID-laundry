using PALMS.ViewModels.Common;

namespace PALMS.Settings.View.Audit
{
    /// <summary>
    /// Interaction logic for AuditHistoryView.xaml
    /// </summary>
    [HasViewModel(typeof(ViewModel.Audit.AuditHistoryViewModel))]
    public partial class AuditHistoryView
    {
        public AuditHistoryView()
        {
            InitializeComponent();
        }
    }
}
