using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.LinenList.ViewModel
{
    [AuthRoles(RoleEnum.Account, RoleEnum.Supervisor)]
    public class LinenListSection : SectionViewModel<LinenListViewModel>
    {
        public override int Index => 4;

        public override string Name => "Client Linens";

        public override string Image => "/PALMS.Settings.View;component/Icons/linenList_64.png";
    }
}
