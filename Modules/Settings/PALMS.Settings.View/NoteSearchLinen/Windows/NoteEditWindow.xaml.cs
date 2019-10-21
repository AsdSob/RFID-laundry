using PALMS.Settings.ViewModel.NoteSearchLinen.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.View.NoteSearchLinen.Windows
{
    /// <summary>
    /// Interaction logic for NoteEditWindow.xaml
    /// </summary>
    /// 
    [HasViewModel(typeof(NoteEditViewModel))]
    public partial class NoteEditWindow
    {
        public NoteEditWindow()
        {
            InitializeComponent();
        }
    }
}
