using System;

namespace Client.Desktop.ViewModels.Common.Windows
{
    public interface IWindowDialogViewModel
    {
        Action<bool> CloseAction { get; set; }
    }
}
