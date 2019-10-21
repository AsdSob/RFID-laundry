using PALMS.Settings.ViewModel.Users;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.View.Users
{
    [HasViewModel(typeof(UserEditViewModel))]
    public partial class UserEditView
    {
        public UserEditView()
        {
            InitializeComponent();
        }
    }
}
