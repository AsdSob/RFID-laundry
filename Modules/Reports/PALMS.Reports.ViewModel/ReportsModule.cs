using System.IO;
using Autofac;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Services;
using PALMS.Reports.ViewModel.Windows;
using PALMS.ViewModels.Common;

namespace PALMS.Reports.ViewModel
{
    public class ReportsModule : IIocModule
    {
        public void Register(ContainerBuilder container)
        {
            container.RegisterType<ReportsSection>().SingleInstance();
            container.RegisterType<ReportsViewModel>().SingleInstance();

            var templateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Templates"); // TODO: use configuration
            container.Register(x => new EpplusReportService(templateDirectory)).As<IExcelReportService>();
            container.RegisterType<CoordinateReportViewModel>().SingleInstance();
            container.RegisterType<LaundryKgWindowViewModel>().SingleInstance();
            container.RegisterType<RevenueWindowViewModel>().SingleInstance();

        }
    }
}