using System.Windows.Controls;
using PALMS.Notes.ViewModel.Window;
using PALMS.ViewModels.Common;

namespace PALMS.Notes.View.Window
{
    /// <summary>
    /// Interaction logic for NoteLinenReplacementWindow.xaml
    /// </summary>
    [HasViewModel(typeof(NoteLinenReplacementViewModel))]
    public partial class NoteLinenReplacementWindow
    {
        public NoteLinenReplacementWindow()
        {
            InitializeComponent();
        }
    }
}
