using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Invoices.ViewModel
{
    [AuthRoles(RoleEnum.Account)]
    public class InvoicesSection : SectionViewModel<InvoicesViewModel>
    {
        public override int Index => 6;

        public override string Name => "Invoices";

        public override string Image => "/PALMS.Settings.View;component/Icons/invoice_64.png";
    }
}
