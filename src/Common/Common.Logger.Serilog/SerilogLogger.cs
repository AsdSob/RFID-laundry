using System;

namespace Common.Logger.Serilog
{
    public class SerilogLogger : ILogger
    {
        private readonly global::Serilog.ILogger _logger;

        public string Name { get; }

        public SerilogLogger(string name, global::Serilog.ILogger logger)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Trace(string message)
        {
            _logger.Verbose(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }

        public void Info(string message)
        {
            _logger.Information(message);
        }
    }
}