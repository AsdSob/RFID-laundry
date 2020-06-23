using System.Windows;
using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Views.Windows
{

    [HasViewModel(typeof(RfidReaderWindowModel))]
    public partial class RfidReaderWindow : CustomWindow
    {
        public RfidReaderWindow()
        {
            InitializeComponent();
        }
    }
}
