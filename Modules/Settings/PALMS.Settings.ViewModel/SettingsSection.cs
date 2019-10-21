using PALMS.ViewModels.Common;

namespace PALMS.Settings.ViewModel
{
    public class SettingsSection : SectionViewModel<SettingsViewModel>
    {
        public override int Index => 2;

        public override string Name => "Settings";

        public override string Image => "/PALMS.Settings.View;component/Icons/settings_64.png";
    }
}
