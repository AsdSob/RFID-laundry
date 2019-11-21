using Autofac;
using Common.Logger.Serilog;

namespace Common.Logger.Module
{
    public class LoggerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SerilogLoggerFactory>()
                .As<ILoggerFactory>()
                .AutoActivate()
                .SingleInstance();
            builder.Register(x =>
            {
                var loggerFactory = x.Resolve<ILoggerFactory>();

                return loggerFactory.CreateLogger("main");
            }).As<ILogger>();
        }
    }

    public static class LoggerModuleExtension
    {
        public static ContainerBuilder RegisterLoggerModule(this ContainerBuilder builder)
        {
            builder.RegisterModule<LoggerModule>();

            return builder;
        }
    }
}
