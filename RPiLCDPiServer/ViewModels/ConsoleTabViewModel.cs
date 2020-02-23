using ReactiveUI;
using RPiLCDPiServer.Services;
using RPiLCDPiServer.Settings;
using RPiLCDShared.Services;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.ViewModels
{
    public class ConsoleTabViewModel : ViewModelBase
    {
        private readonly LogToEventService _loggingService;

        private StringBuilder _consoleStringBuilder;
        private Queue<ConsoleMessage> _messages = new Queue<ConsoleMessage>();
        private int _messageQueueLength = 25;

        private string _consoleString = "...";
        public string ConsoleString
        {
            get { return _consoleString; }
            set { this.RaiseAndSetIfChanged(ref _consoleString, value); }
        }

        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }

        public ConsoleTabViewModel()
        {
            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            _consoleStringBuilder = new StringBuilder();

            var serviceSelector = new ServiceSelector();

            _loggingService = (LogToEventService)serviceSelector.GetLoggingService();
            if (_loggingService != null)
            {
                _loggingService.MessageLoggedRecievedEvent += ShowLoggedMessageEvent;
            }
        }

        private void ShowLoggedMessageEvent(object sender, MessageLoggedEventArgs a)
        {
            if (a.MessageLevel == MessageLevel.Trigger)
            {
                return;
            }

            _messages.Enqueue(new ConsoleMessage() { message = a.LoggedMessage, level = a.MessageLevel });
            if (_messages.Count > _messageQueueLength)
            {
                _messages.Dequeue();
            }

            _consoleStringBuilder.Clear();
            foreach (ConsoleMessage message in _messages)
            {
                _consoleStringBuilder.AppendLine(message.message);
            }
            ConsoleString = _consoleStringBuilder.ToString();
        }
    }

    class ConsoleMessage
    {
        public string message;
        public MessageLevel level;
    } 
}
