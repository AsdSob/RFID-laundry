using NLog;
using PALMS.ViewModels.Common.Services;
using ILogger = PALMS.ViewModels.Common.Services.ILogger;

namespace PALMS.ViewModels.Services
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string loggerName, string correlationId = null)
        {
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("nlog.config");

            var logger = LogManager.GetLogger(loggerName);

            return new NLogger(logger);
        }
    }
}