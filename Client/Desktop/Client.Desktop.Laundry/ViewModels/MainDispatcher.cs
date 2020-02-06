using System;
using System.Windows;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Laundry.ViewModels
{
    public class MainDispatcher : IMainDispatcher
    {

        public void RunInMainThread(Action action)
        {
            Application.Current.MainWindow.Dispatcher.Invoke(action);
        }
    }
}
