using System;

namespace PALMS.ViewModels.Common.Services
{
    public interface ILogger
    {
        void Debug(string message);
        void Trace(string message);
        void Error(string message);
        void Error(string message, Exception exception);
        void Warning(string message);
        void Info(string message);
    }
}
