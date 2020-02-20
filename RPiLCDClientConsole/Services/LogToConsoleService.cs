using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDClientConsole.Services
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

        public void LogWarning(string message)
        {
            Console.WriteLine(message);
        }
    }
}
