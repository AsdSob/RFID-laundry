using System;

namespace PALMS.ViewModels.Common.Services
{
    public interface ICanExecuteMediator
    {
        Func<bool> CanExecute { get; set; }
    }
}
