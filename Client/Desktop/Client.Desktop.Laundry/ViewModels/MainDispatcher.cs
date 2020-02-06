using System;
using System.Windows;
using System.Windows.Threading;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Laundry.ViewModels
{
    public class MainDispatcher : IMainDispatcher
    {

        public void RunInMainThread(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
        {
            Application.Current.MainWindow.Dispatcher.Invoke(action, dispatcherPriority );
        }
    }
}
