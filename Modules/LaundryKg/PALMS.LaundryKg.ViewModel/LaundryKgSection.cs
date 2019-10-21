using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;


namespace PALMS.LaundryKg.ViewModel
{
    [AuthRoles(RoleEnum.Reception, RoleEnum.Account, RoleEnum.Supervisor)]
    public class LaundryKgSection : SectionViewModel<LaundryKgViewModel>
    {
        public override int Index => 9;

        public override string Name => "LaundryKg";

        public override string Image => "/PALMS.Settings.View;component/Icons/kg_64.png";
    }
}
