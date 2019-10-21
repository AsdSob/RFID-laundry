using System.IO;
using Autofac;
using PALMS.LaundryKg.ViewModel.Windows;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Services;
using PALMS.ViewModels.Common;

namespace PALMS.LaundryKg.ViewModel
{
    public class LaundryKgModule: IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<LaundryKgSection>().SingleInstance();
            container.RegisterType<LaundryKgViewModel>().SingleInstance();

            var templateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Templates"); // TODO: use configuration
            container.Register(x => new EpplusReportService(templateDirectory)).As<IExcelReportService>();
            container.RegisterType<NoteReportWindowViewModel>().SingleInstance();
            container.RegisterType<ChangeDetailViewModel>().SingleInstance();

        }
    }
}
