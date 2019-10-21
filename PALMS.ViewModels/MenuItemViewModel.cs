using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.ViewModels
{
    [AuthRoles(RoleEnum.Account, RoleEnum.Supervisor)]
    public class ClientsSection : SectionViewModel<ClientsViewModel>
    {
        public override int Index => 1;

        public override string Name => "Clients";

        public override string Image => "/PALMS.WPFClient;component/Icons/clients.png";
    }

}
