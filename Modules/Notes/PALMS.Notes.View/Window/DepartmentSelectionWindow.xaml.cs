using PALMS.Notes.ViewModel.Window;
using PALMS.ViewModels.Common;

namespace PALMS.Notes.View.Window
{
    /// <summary>
    /// Interaction logic for DepartmentSelectionWindow.xaml
    /// </summary>
    /// 
    [HasViewModel(typeof(DepartmentSelectionViewModel))]

    public partial class DepartmentSelectionWindow
    {
        public DepartmentSelectionWindow()
        {
            InitializeComponent();
        }
    }
}
