using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.NoteHistory.ViewModel
{
    [AuthRoles(RoleEnum.Reception, RoleEnum.Account)]
    class NoteHistorySection : SectionViewModel<NoteHistoryViewModel>
    {
        public override int Index => 8;

        public override string Name => "Notes History";

        public override string Image => "/PALMS.Settings.View;component/Icons/history_64.png";
    }
}
