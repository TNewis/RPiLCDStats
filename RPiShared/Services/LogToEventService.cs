using System;

namespace RPiLCDShared.Services
{
    public class LogToEventService : ILoggingService
    {
        private readonly MessageLoggedEventArgs _args = new MessageLoggedEventArgs();
        public event EventHandler<MessageLoggedEventArgs> MessageLoggedRecievedEvent;
        protected void OnMessageLogged(MessageLoggedEventArgs e)
        {
            MessageLoggedRecievedEvent?.Invoke(null, e);
        }

        public void LogError(string message)
        {
            _args.LoggedMessage = DateTime.Now.ToLongTimeString() + ": " + message;
            _args.MessageLevel = MessageLevel.Error;
            _args.TriggerType = LogTriggerTypes.None;
            OnMessageLogged(_args);
        }

        public void LogMessage(string message)
        {
            _args.LoggedMessage = DateTime.Now.ToLongTimeString() + ": " + message;
            _args.MessageLevel = MessageLevel.Message;
            _args.TriggerType = LogTriggerTypes.None;
            OnMessageLogged(_args);
        }

        public void LogWarning(string message)
        {
            _args.LoggedMessage = DateTime.Now.ToLongTimeString() + ": " + message;
            _args.MessageLevel = MessageLevel.Warning;
            _args.TriggerType = LogTriggerTypes.None;
            OnMessageLogged(_args);
        }

        public void LogTrigger(LogTriggerTypes trigger)
        {
            _args.TriggerType = trigger;
            _args.MessageLevel = MessageLevel.Trigger;
            _args.LoggedMessage = "";
            OnMessageLogged(_args);
        }

        public void SetLoggingService(ILoggingService loggingService)
        {
        }


    }

    public class MessageLoggedEventArgs : EventArgs
    {
        public string LoggedMessage { get; set; }
        public MessageLevel MessageLevel { get; set; }
        public LogTriggerTypes TriggerType { get; set; } = LogTriggerTypes.None;
    }

    public enum MessageLevel
    {
        Trigger,
        Message,
        Warning,
        Error
    }
}
