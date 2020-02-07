using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Laundry.ViewModels
{
    public class MainDispatcher : IMainDispatcher
    {
        public void RunInMainThread(Action action)
        {
            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Send, action);
        }

    }
}
