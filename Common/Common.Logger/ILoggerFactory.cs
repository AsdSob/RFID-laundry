namespace Common.Logger
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(string loggerName, string correlationId = null);
    }
}
