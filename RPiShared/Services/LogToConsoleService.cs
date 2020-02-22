using System;

namespace RPiLCDShared.Services
{
    public class LogToConsoleService : ILoggingService
    {
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }

        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogTrigger(LogTriggerTypes trigger)
        {
        }

        public void LogWarning(string message)
        {
            Console.WriteLine(message);
        }

        public void SetLoggingService(ILoggingService loggingService)
        {
        }
    }
}
