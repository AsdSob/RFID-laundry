using System.Windows;
using System.Windows.Controls;
using Client.Desktop.ViewModels.Common.Attributes;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.Laundry.Windows
{
    /// <summary>
    /// Interaction logic for RfidReaderWindow.xaml
    /// </summary>
    [HasViewModel(typeof(RfidReaderWindowModel))]
    public partial class RfidReaderWindow : Window
    {
        public RfidReaderWindow()
        {
            InitializeComponent();
        }
    }
}
