using System;
using System.Windows;
using System.Windows.Forms;
using DevExpress.Xpf.Core;
using PALMS.ViewModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using Application = System.Windows.Application;

namespace PALMS.WPFClient.Services
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

        public DialogService(IResolver resolverService)
        {
            _resolver = resolverService ?? throw new ArgumentNullException(nameof(resolverService));

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
            var saveFileDialog = new SaveFileDialog {Filter = filter, FileName = fileName};

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }

            return null;
        }

        private void SetIsBusy(bool isBusy)
        {
            var viewModel = _resolver.Resolve<MainViewModel>();

            Helper.RunInMainThread(() => viewModel.IsBusy = isBusy);
        }

        private bool ShowMessageDialog(string questionMessage, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            var dialogResult = DXMessageBox.Show(Application.Current.MainWindow, questionMessage, caption, buttons, image);

            return dialogResult == MessageBoxResult.OK || dialogResult == MessageBoxResult.Yes;
        }
    }
}
