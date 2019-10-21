using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using PALMS.ViewModels;
using PALMS.ViewModels.Services;
using PALMS.Reports.Common;
using PALMS.ViewModels.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PALMS.Notes.ViewModel.Window;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.WindowViewModel;
using PALMS.WPFClient.Services;
using PALMS.Reports.Xtra;
using PALMS.Reports.Epplus.Services;

namespace PALMS.WPFClient.ViewModel
{
    public class ViewModelLocator
    {
        private static IContainer Container { get; set; }

        public MainViewModel Main => Container.Resolve<MainViewModel>();

        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();

            RegisterServices(builder);
            RegisterViewModels(builder);

            Container = builder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(Container));
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceLocatorResolver>().As<IResolver>().SingleInstance();
            builder.RegisterType<CustomDispatcher>().As<IDispatcher>().SingleInstance();
            builder.RegisterType<ContextFactory>().As<IContextFactory>().SingleInstance();
            builder.RegisterType<DataService>().As<IDataService>().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.Register(x => x.Resolve<ILoggerFactory>().CreateLogger("main")).As<ILogger>().SingleInstance();

            builder.RegisterType<ReportsService>().As<IReportService>().SingleInstance();
            builder.RegisterType<EpplusReportService>().As<IExcelReportService>().SingleInstance();

            //builder.RegisterType<FakeReportsService>().As<IReportService>().SingleInstance();
            builder.RegisterType<ModuleCanChangeMediator>().As<ICanExecuteMediator>().SingleInstance();
            builder.RegisterType<AuthService>().As<IAuthService>().OnRelease(x => x.Dispose());
            builder.RegisterType<UserIdentity>().As<IUserIdentity>().SingleInstance();
           
        }

        private static void RegisterViewModels(ContainerBuilder builder)
        {
            RegisterModules(builder);

            // main
            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<MenuViewModel>().SingleInstance();
            builder.RegisterType<LoginViewModel>().SingleInstance();

            // sections
            builder.RegisterType<ClientsSection>().SingleInstance();

            builder.RegisterType<DataViewModel>();
            builder.RegisterType<ClientsViewModel>().SingleInstance();
            builder.RegisterType<ClientDetailsViewModel>();
            builder.RegisterType<DepartmentDetailsViewModel>();
            builder.RegisterType<ChargeDetailsViewModel>();
            builder.RegisterType<AddLinenViewModel>();
            builder.RegisterType<DepartmentSelectionViewModel>();
            builder.RegisterType<AddLinenListViewModel>();
            builder.RegisterType<NoteLinenReplacementViewModel>();
        }

        private static void RegisterModules(ContainerBuilder container)
        {
            var modules = GetAssemblies()
                .Where(x => x.FullName.Contains("PALMS"))
                .SelectMany(s => s.GetTypes()).Where(x => x.IsAbstract == false && x.GetInterfaces().Contains(typeof(IIocModule)))
                .Distinct(new TypeEqualityComparer());

            foreach (var module in modules)
            {
                var moduleInit = Expression.MemberInit(Expression.New(module));
                var callingMethod = Expression.Call(
                    moduleInit, module.GetMethod(nameof(IIocModule.Register)), Expression.Constant(container, typeof(ContainerBuilder)));

                Expression.Lambda<Action>(callingMethod).Compile().Invoke();
            }
        }

        private static ICollection<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        private class TypeEqualityComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

                return x.FullName == y.FullName;
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    public class ServiceLocatorResolver : IResolver
    {
        public T Resolve<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public object Resolve(Type type)
        {
            return ServiceLocator.Current.GetInstance(type);
        }
    }

    public class CustomDispatcher : IDispatcher
    {
        public void RunInMainThread(Action action)
        {
            Helper.RunInMainThread(action);
        }
    }

    public class FakeReportsService : IReportService
    {
        public void Print(IReport report)
        {
            throw new NotImplementedException();
        }

        public void Print(IReport report, string printer)
        {
            throw new NotImplementedException();
        }

        public void SaveAsAsync(IReport report, string path)
        {
            throw new NotImplementedException();
        }

        public void ShowReportPreview(IReport report)
        {
            throw new NotImplementedException();
        }

        public bool ShowReportPreviewBool(IReport report)
        {
            throw new NotImplementedException();
        }
    }
}