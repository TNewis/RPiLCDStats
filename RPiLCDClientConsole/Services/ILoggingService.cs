using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public interface ILoggingService
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogMessage(string message);
    }
}
