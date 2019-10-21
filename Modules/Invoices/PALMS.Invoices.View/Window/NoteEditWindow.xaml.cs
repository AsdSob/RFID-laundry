using PALMS.Invoices.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.View.Window
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
