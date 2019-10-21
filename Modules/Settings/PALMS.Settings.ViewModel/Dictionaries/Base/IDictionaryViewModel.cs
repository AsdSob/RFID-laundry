using System;

namespace PALMS.Settings.ViewModel.Dictionaries.Base
{
    public interface IDictionaryViewModel
    {
        Action CancelEditAction { get; set; }
    }
}