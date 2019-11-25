using System;

namespace PALMS.ViewModels.Common.Window
{
    public interface IWindowDialogViewModel
    {
        Action<bool> CloseAction { get; set; }
    }
}
