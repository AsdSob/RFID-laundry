using Autofac;
using PALMS.NoteHistory.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.NoteHistory.ViewModel
{
    public class NoteHistoryModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<NoteHistorySection>().SingleInstance();
            container.RegisterType<NoteHistoryViewModel>().SingleInstance();
            container.RegisterType<AddLinenListViewModel>().SingleInstance();
            container.RegisterType<ClientDepSelectViewModel>().SingleInstance();


        }
    }
}
