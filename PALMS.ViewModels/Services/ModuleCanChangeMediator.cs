using System;
using PALMS.ViewModels.Common.Services;

namespace PALMS.ViewModels.Services
{
    public class ModuleCanChangeMediator : ICanExecuteMediator
    {
        public Func<bool> CanExecute { get; set; }
    }
}
