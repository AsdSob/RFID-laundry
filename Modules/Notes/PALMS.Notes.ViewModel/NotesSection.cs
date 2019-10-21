using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    [AuthRoles(RoleEnum.Account, RoleEnum.Reception, RoleEnum.Supervisor)]
    class NotesSection : SectionViewModel<NotesViewModel>
    {
        public override int Index => 5;

        public override string Name => "Notes";

        public override string Image => "/PALMS.Settings.View;component/Icons/note_64.png";
    }
}
