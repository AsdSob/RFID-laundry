using System;
using NLog;
using ILogger = PALMS.ViewModels.Common.Services.ILogger;

namespace PALMS.ViewModels.Services
{
    public class NLogger : ILogger
    {
        private readonly Logger _logger;

        public string Name { get; }

        public NLogger(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Name = _logger.Name;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }
    }
}