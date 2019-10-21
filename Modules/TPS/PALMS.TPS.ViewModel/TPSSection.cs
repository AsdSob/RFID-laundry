using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.TPS.ViewModel
{
    [AuthRoles(RoleEnum.Operator, RoleEnum.Account, RoleEnum.Reception, RoleEnum.Supervisor)]
    public class TpsSection : SectionViewModel<TpsViewModel>
    {
        public override int Index => 10;

        public override string Name => "Labeling";

        public override string Image => "/PALMS.Settings.View;component/Icons/tps_64.png";
    }
}
