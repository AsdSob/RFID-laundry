using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using Microsoft.Practices.ServiceLocation;
using PALMS.ViewModels.Common.Services;

namespace PALMS.WPFClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplicationThemeHelper.ApplicationThemeName = Theme.Office2016WhiteName;

            ThemeManager.ThemeChanged += (sender, args) => { System.Diagnostics.Debug.WriteLine(args.ThemeName); };

            CheckDb();
        }

        private static void CheckDb()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["PrimeConnection"].ConnectionString;

                ServiceLocator.Current.GetInstance<IAppSettings>().ConnectionString = connectionString;

                var builder = new SqlConnectionStringBuilder(connectionString);

                string database = builder.InitialCatalog;

                builder.InitialCatalog = "";
                var serverConnectionString = builder.ConnectionString;

                using (var connection = new SqlConnection(serverConnectionString))
                {
                    using (var command = new SqlCommand($"SELECT db_id('{database}')", connection))
                    {
                        connection.Open();
                        var dbIsExist = command.ExecuteScalar() != DBNull.Value;
                        if (!dbIsExist)
                        {
                            // will be created by LoadAsync
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceLocator.Current.GetInstance<ILogger>().Error("Database not available", ex);
                ServiceLocator.Current.GetInstance<IDataService>().SetState(DatabaseState.Notavailable);
                return;
            }

            //ServiceLocator.Current.GetInstance<IDataService>().LoadAsync();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var message = e.Exception.Message;
            if (e.Exception.InnerException != null)
                message = $"{message}{Environment.NewLine}{e.Exception.InnerException.Message}";

            ServiceLocator.Current.GetInstance<ILogger>().Error(message, e.Exception);
            ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorDialog(message);

            e.Handled = true;
        }
    }
}
