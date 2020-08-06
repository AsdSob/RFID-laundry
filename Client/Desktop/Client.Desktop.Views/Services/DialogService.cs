using System;
using System.Windows;
using Client.Desktop.ViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.Windows;
using Microsoft.Win32;

namespace Client.Desktop.Views.Services
{
    public class DialogService : IDialogService
    {
        public bool ShowDialog(IWindowDialogViewModel windowDialogViewModel)
        {
            var window = windowDialogViewModel?.GetType().GetControl<Window>();
            if (window == null)
                return false;

            window.DataContext = windowDialogViewModel;

            return window.ShowDialog() == true;
        }

        private readonly IResolver _resolver;
        private readonly IMainDispatcher _dispatcher;

        public DialogService(IResolver resolverService, IMainDispatcher dispatcher)
        {
            _resolver = resolverService ?? throw new ArgumentNullException(nameof(resolverService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        }

        public bool ShowQuestionDialog(string message)
        {
            return ShowMessageDialog(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public bool ShowErrorDialog(string message)
        {
            return ShowMessageDialog(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowWarnigDialog(string message)
        {
            return ShowMessageDialog(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowInfoDialog(string message)
        {
            return ShowMessageDialog(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowBusy()
        {
            SetIsBusy(true);
        }

        public void HideBusy()
        {
            SetIsBusy(false);
        }

        public string ShowSaveFileDialog(string filter, string fileName)
        {
            var saveFileDialog = new SaveFileDialog { Filter = filter, FileName = fileName };

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return null;
        }

        private void SetIsBusy(bool isBusy)
        {
            var viewModel = _resolver.Resolve<MainViewModel>();

            _dispatcher.RunInMainThread(() => viewModel.IsBusy = isBusy);

            //Helper.RunInMainThread(() => viewModel.IsBusy = isBusy);
        }

        private bool ShowMessageDialog(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            var dialogResult = MessageBox.Show(Application.Current.MainWindow, message, caption, buttons, image);

            return dialogResult == MessageBoxResult.OK || dialogResult == MessageBoxResult.Yes;
        }
    }
}
