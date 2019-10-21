using Autofac;
using PALMS.Notes.ViewModel.Window;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    public class NotesModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<NotesSection>().SingleInstance();

            container.Register(x => new NotesViewModel(x.Resolve<DeliveryNoteViewModel>(), x.Resolve<ICanExecuteMediator>()))
                .As<NotesViewModel>()
                .SingleInstance();

            container.RegisterType<DeliveryNoteViewModel>().SingleInstance();

            container.RegisterType<NoteCommonMethods>().SingleInstance();
            container.RegisterType<PrintCopyNumberViewModel>().SingleInstance();
        }
    }
}
