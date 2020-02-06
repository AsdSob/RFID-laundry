using System;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IMainDispatcher
    {
        void RunInMainThread(Action action);
    }
}
