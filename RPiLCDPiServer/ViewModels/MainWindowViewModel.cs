using System.Threading.Tasks;
using ReactiveUI;
using RPiLCDPiServer.Enums;
using RPiLCDPiServer.Services;
using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDPiServer.Settings;
using RPiLCDShared.Services;

namespace RPiLCDPiServer.ViewModels
{
    public class MainWindowViewModel : UpdateableViewModelBase
    {
        private readonly IConnectionService _connectionService;
        private readonly LogToEventService _loggingService;
        public IReactiveCommand SetTabSummaryCommand { get; set; }
        public IReactiveCommand SetTabCPUCommand { get; set; }
        public IReactiveCommand SetTabGPUCommand { get; set; }
        public IReactiveCommand SetTabConsoleCommand { get; set; }
        public IReactiveCommand SetTabSettingsCommand { get; set; }

        private bool _showingSummaryTab = true;
        public bool ShowingSummaryTab
        {
            get { return _showingSummaryTab; }
            set { this.RaiseAndSetIfChanged(ref _showingSummaryTab, value); }
        }

        private bool _showingCPUTab = false;
        public bool ShowingCPUTab
        {
            get { return _showingCPUTab; }
            set { this.RaiseAndSetIfChanged(ref _showingCPUTab, value); }
        }

        private bool _showingGPUTab = false;
        public bool ShowingGPUTab
        {
            get { return _showingGPUTab; }
            set { this.RaiseAndSetIfChanged(ref _showingGPUTab, value); }
        }

        private bool _showingSettingsTab = false;
        public bool ShowingSettingsTab
        {
            get { return _showingSettingsTab; }
            set { this.RaiseAndSetIfChanged(ref _showingSettingsTab, value); }
        }

        private bool _showingConsoleTab = false;
        public bool ShowingConsoleTab
        {
            get { return _showingConsoleTab; }
            set { this.RaiseAndSetIfChanged(ref _showingConsoleTab, value); }
        }

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

        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }
        public MainWindowViewModel()
        {
            SetTabSummaryCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.Summary); });
            SetTabCPUCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.CPU); });
            SetTabGPUCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.GPU); });
            SetTabConsoleCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.Console); });
            SetTabSettingsCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.Settings); });

            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            var serviceSelector = new ServiceSelector();
            _connectionService = serviceSelector.GetServiceInstance<IConnectionService>();
            _loggingService = (LogToEventService)serviceSelector.GetLoggingService();

            ConnectionService.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;
            OpenConnection();
        }

        public void OpenConnection()
        {
            Task.Run(() =>
            {
                _connectionService.OpenConnection();
            });
        }

        private void SelectTabAndHideOthers(CurrentTab currentTab)
        {
            switch (currentTab)
            {
                case CurrentTab.Summary:
                    ShowingSettingsTab = false;
                    ShowingSummaryTab = true;
                    ShowingCPUTab = false;
                    ShowingGPUTab = false;
                    ShowingConsoleTab = false;
                    break;
                case CurrentTab.CPU:
                    ShowingSettingsTab = false;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = true;
                    ShowingGPUTab = false;
                    ShowingConsoleTab = false;
                    break;
                case CurrentTab.GPU:
                    ShowingSettingsTab = false;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = false;
                    ShowingGPUTab = true;
                    ShowingConsoleTab = false;
                    break;
                case CurrentTab.Settings:
                    ShowingSettingsTab = true;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = false;
                    ShowingGPUTab = false;
                    ShowingConsoleTab = false;
                    break;
                case CurrentTab.Console:
                    ShowingSettingsTab = false;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = false;
                    ShowingGPUTab = false;
                    ShowingConsoleTab = true;
                    break;
            }
        }

        protected override void ProcessUpdate(string message)
        {
            var clockString = message.Substring(UpdateTags.ClockTabTag.TagOpen, UpdateTags.ClockTabTag.TagClose);
            if (clockString != null)
            {
                clockString = message.Substring(UpdateTags.ClockTag.TagOpen, UpdateTags.ClockTag.TagClose);
            }
        }
    }
}
