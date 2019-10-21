using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Reports.ViewModel
{
    [AuthRoles(RoleEnum.Account)]
    public class ReportsSection : SectionViewModel<ReportsViewModel>
    {
        public override int Index => 7;

        public override string Name => "Reports";

        public override string Image => "/PALMS.Settings.View;component/Icons/reports_64.png";
    }
}
