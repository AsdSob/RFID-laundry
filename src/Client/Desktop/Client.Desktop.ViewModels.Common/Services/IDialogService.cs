using Client.Desktop.ViewModels.Common.Windows;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// Show dialog.
        /// </summary>
        /// <param name="windowDialogViewModel">The view model of the window.</param>
        /// <returns>The dialog result.</returns>
        bool ShowDialog(IWindowDialogViewModel windowDialogViewModel);

        /// <summary>
        /// Show question dialog.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ShowQuestionDialog(string message);

        /// <summary>
        /// Show error dialog.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ShowErrorDialog(string message);

        /// <summary>
        /// Show warning dialog.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ShowWarnigDialog(string message);

        /// <summary>
        /// Show info dialog.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ShowInfoDialog(string message);

        void ShowBusy();

        void HideBusy();

        string ShowSaveFileDialog(string filter, string fileName);
    }
}
