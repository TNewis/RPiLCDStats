using ReactiveUI;
using RPiLCDPiServer.Services;
using RPiLCDPiServer.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.ViewModels
{
    public class SettingsTabViewModel: ViewModelBase
    {
        private readonly IShutdownService _shutdownService;
        private readonly IDisplayControlService _displayControlService;

        public IReactiveCommand ShutdownCommand { get; set; }
        public IReactiveCommand ExitToOSCommand { get; set; }
        public IReactiveCommand RestartCommand { get; set; }
        public IReactiveCommand SetBrightnessCommand { get; set; }

        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }

        public int MinBrightness { get; set; }
        public int MaxBrightness { get; set; }

        private int _currentBrightness;
        public int CurrentBrightness
        {
            get { return _currentBrightness; }
            set { this.RaiseAndSetIfChanged(ref _currentBrightness, value); _displayControlService.SetBrightness(CurrentBrightness); }
        }
        public SettingsTabViewModel()
        {
            var serviceSelector = new ServiceSelector();
            _shutdownService = serviceSelector.GetServiceInstance<IShutdownService>();
            _displayControlService = serviceSelector.GetServiceInstance<IDisplayControlService>();

            ShutdownCommand = ReactiveCommand.Create(() => { _shutdownService.Shutdown(); });
            RestartCommand = ReactiveCommand.Create(() => { _shutdownService.Restart(); });
            ExitToOSCommand = ReactiveCommand.Create(() => { Environment.Exit(0); });

            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            MinBrightness = _displayControlService.GetMinBrightness();
            MaxBrightness = _displayControlService.GetMaxBrightness();

            _currentBrightness = _displayControlService.GetCurrentBrightness();
        }
    }
}
