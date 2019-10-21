using Autofac;
using PALMS.Invoices.ViewModel.Service;
using PALMS.Invoices.ViewModel.Tabs;
using PALMS.Invoices.ViewModel.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Invoices.ViewModel
{
    class InvoiceModul : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<InvoicesSection>().SingleInstance();

            container.Register(x => new InvoicesViewModel(x.Resolve<TabsViewModel>(), x.Resolve<ICanExecuteMediator>()))
                .As<InvoicesViewModel>().SingleInstance();

            container.RegisterType<TabsViewModel>().SingleInstance();
            container.RegisterType<InvoicingViewModel>().SingleInstance();
            container.RegisterType<PriceCounting>().SingleInstance();
            container.RegisterType<InvoiceEditViewModel>().SingleInstance();

            container.RegisterType<ChargeViewModel>().SingleInstance();
            container.RegisterType<NoteEditViewModel>().SingleInstance();
            container.RegisterType<ChangeLinenPriceViewModel>().SingleInstance();
            container.RegisterType<ChangeNotesLinenViewModel>().SingleInstance();
            container.RegisterType<DepartmentDetailsViewModel>().SingleInstance();
            container.RegisterType<AnnexWindowViewModel>().SingleInstance();


        }
    }
}
