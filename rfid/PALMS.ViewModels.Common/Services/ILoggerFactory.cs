namespace PALMS.ViewModels.Common.Services
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(string loggerName, string correlationId = null);
    }
}