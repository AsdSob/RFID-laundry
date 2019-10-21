using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.MasterLinens.ViewModel
{
    [AuthRoles(RoleEnum.Account, RoleEnum.Supervisor)]
    public class MasterLinensSection : SectionViewModel<MasterLinensViewModel>
    {
        public override int Index => 3;

        public override string Name => "Master Linens";

        public override string Image => "/PALMS.Settings.View;component/Icons/masterLinen_64.png";
    }
}
