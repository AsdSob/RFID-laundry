using System.Windows;
using Client.Desktop.ViewModels.Common.Windows;

namespace Client.Desktop.Views.Windows
{
    public class CustomWindow : Window
    {
        public CustomWindow()
        {
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            Loaded += CustomWindowOnLoaded;
        }

        private void CustomWindowOnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as IWindowDialogViewModel;
            if (viewModel != null)
                viewModel.CloseAction = CloseWindow;
        }

        private void CloseWindow(bool dialogResult)
        {
            DialogResult = dialogResult;

            Close();
        }
    }
}
