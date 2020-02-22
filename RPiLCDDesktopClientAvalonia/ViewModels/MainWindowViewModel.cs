using ReactiveUI;
using RPiLCDClientConsole;
using RPiLCDClientConsole.Services;
using RPiLCDClientConsole.Update;
using System.Threading.Tasks;
using System.Timers;
using RPiLCDShared.Services;

namespace RPiLCDDesktopClientAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static LogToEventService _loggingService;

        private static PCClient _client;
        private static string _message;
        private static IUpdateBuilder _updateBuilder;


        private string _loggedMessage;
        public string LoggedMessage
        {
            get { return _loggedMessage; }
            set { this.RaiseAndSetIfChanged(ref _loggedMessage, value); }
        }

        private string _loggedWarning;
        public string LoggedWarning
        {
            get { return _loggedWarning; }
            set { this.RaiseAndSetIfChanged(ref _loggedWarning, value); }
        }

        private string _loggedError;
        public string LoggedError
        {
            get { return _loggedError; }
            set { this.RaiseAndSetIfChanged(ref _loggedError, value); }
        }
        public MainWindowViewModel()
        {
            _loggingService = new LogToEventService();
            PCClient.SetLoggingService(_loggingService);
            _loggingService.MessageLoggedRecievedEvent += ShowLoggedMessageEvent;

            _loggingService.LogMessage("Starting Client...");

            Timer _timer;

            _client = new PCClient();
            _updateBuilder = new UpdateBuilder();

            AddAllServices(_updateBuilder);

            _timer = new Timer(1000){ AutoReset = true };
            _timer.Elapsed += async (sender, e) => await Update();
            _timer.Start();
        }

        private static Task Update()
        {
            return Task.Run(() =>
            {
                _message = _updateBuilder.BuildFullUpdateString();
                _client.SendUpdate(_message);
            });
        }

        private void AddAllServices(IUpdateBuilder updateBuilder)
        {
            PCClient.SetLoggingService(_loggingService);

            updateBuilder.AddUpdatableService(new UpdatableClock());

            updateBuilder.AddUpdatableService(new UpdatableCoreTempCPUMonitorService(_loggingService));

            var gpuStatsService = new UpdatableGPUzMonitorService(_loggingService);
            updateBuilder.AddUpdatableService(gpuStatsService);
            updateBuilder.AddStaticDataService(gpuStatsService);

            var audioSwitcher = new AudioSwitcherService(_loggingService);
            updateBuilder.AddUpdatableService(audioSwitcher);
            updateBuilder.AddStaticDataService(audioSwitcher);

            updateBuilder.AddStaticDataService(new WindowsMemoryCheckService());

            updateBuilder.StartAllServices();
        }

        private void ShowLoggedMessageEvent(object sender, MessageLoggedEventArgs a)
        {
            if (a.MessageLevel== MessageLevel.Error)
            {
                LoggedError = a.LoggedMessage;
                return;
            }

            if (a.MessageLevel == MessageLevel.Warning)
            {
                LoggedWarning = a.LoggedMessage;
                return;
            }

            LoggedMessage = a.LoggedMessage;
        }

    }
}
