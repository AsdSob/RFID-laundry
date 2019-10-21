using Autofac;
using PALMS.LinenList.ViewModel.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.LinenList.ViewModel
{
    public class LinenListModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<LinenListSection>().SingleInstance();

            container.RegisterType<LinensListViewModel>().SingleInstance();

            container.Register(x => new LinenListViewModel(x.Resolve<LinensListViewModel>(), x.Resolve<ICanExecuteMediator>()))
                .As<LinenListViewModel>().SingleInstance();

            container.RegisterType<LeasingLinenViewModel>().SingleInstance();
            container.RegisterType<SelectCopySourceViewModel>().SingleInstance();
            container.RegisterType<ChangeNoteLinenViewModel>().SingleInstance();
            container.RegisterType<UnUsedLinenViewModel>().SingleInstance();

        }
    }
}