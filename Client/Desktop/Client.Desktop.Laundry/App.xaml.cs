using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Client.Desktop.Laundry.ViewModels;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Services;
using Common.Logger;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;

namespace Client.Desktop.Laundry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // create a custom principal with an anonymous identity at startup
            var customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            // database migration
            using (var context = ViewModelLocator.Container.Resolve<IDbContextFactory>().Create())
                context.Database.Migrate();

            base.OnStartup(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var message = e.Exception.Message;
            if (e.Exception.InnerException != null)
                message = $"{message}{Environment.NewLine}{e.Exception.InnerException.Message}";

            var container = ViewModelLocator.Container;

            container.Resolve<ILogger>().Error(message, e.Exception);
            container.Resolve<IDialogService>().ShowErrorDialog(message);

            e.Handled = true;
        }
    }
}
