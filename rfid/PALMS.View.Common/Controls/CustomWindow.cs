using System;
using System.Windows;
using System.Windows.Input;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Window;

namespace PALMS.View.Common.Controls
{
    public class CustomWindow : Window
    {
        public CustomWindow()
        {
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow?.IsActive == true)
            {
                Owner = mainWindow;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ShowInTaskbar = false;
            }

            Loaded += CustomWindowOnLoaded;
            PreviewKeyDown += HandleEsc;
        }

        private async void CustomWindowOnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as IWindowDialogViewModel;
            if (viewModel != null)
                viewModel.CloseAction = CloseWindow;

            var initializeViewModel = DataContext as IInitializationAsync;
            if (initializeViewModel != null)
                await initializeViewModel.InitializeAsync();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                CloseWindow(false);
        }

        private void CloseWindow(bool dialogResult)
        {
            try
            {
                DialogResult = dialogResult;

                Close();
            }
            catch (Exception ex)
            {
                // TODO: log
            }
        }
    }
}
