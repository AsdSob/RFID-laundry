using System;

namespace PALMS.ViewModels.Common.Services
{
    public interface IDispatcher
    {
        void RunInMainThread(Action action);
    }
}
