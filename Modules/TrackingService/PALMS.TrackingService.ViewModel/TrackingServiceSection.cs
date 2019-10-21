using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.TrackingService.ViewModel
{
    [AuthRoles(RoleEnum.Account, RoleEnum.Reception)]
    public class TrackingServiceSection : SectionViewModel<TrackingServiceViewModel>
    {
        public override int Index => 8;

        public override string Name => "Tracking Service";

        public override string Image => "/PALMS.Settings.View;component/Icons/tracking_64.png";
    }
}
