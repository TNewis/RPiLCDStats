namespace RPiLCDShared.Services
{
    public interface ILoggingService : IService
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogMessage(string message);
        void LogTrigger(LogTriggerTypes trigger);
    }
}
