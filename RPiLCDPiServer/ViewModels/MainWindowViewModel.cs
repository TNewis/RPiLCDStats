using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ReactiveUI;
using RPiLCDPiServer.Enums;
using RPiLCDPiServer.Services;
using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDPiServer.Settings;

namespace RPiLCDPiServer.ViewModels
{
    public class MainWindowViewModel : UpdateableViewModelBase
    {
        private readonly IConnectionService _connectionService;
        public IReactiveCommand SetTabSummaryCommand { get; set; }
        public IReactiveCommand SetTabCPUCommand { get; set; }
        public IReactiveCommand SetTabGPUCommand { get; set; }
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

        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }
        public MainWindowViewModel()
        {
            SetTabSummaryCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.Summary); });
            SetTabCPUCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.CPU); });
            SetTabGPUCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.GPU); });
            SetTabSettingsCommand = ReactiveCommand.Create(() => { SelectTabAndHideOthers(CurrentTab.Settings); });

            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            var serviceSelector = new ServiceSelector();
            _connectionService = serviceSelector.GetServiceInstance<IConnectionService>();
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
                    break;
                case CurrentTab.CPU:
                    ShowingSettingsTab = false;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = true;
                    ShowingGPUTab = false;
                    break;
                case CurrentTab.GPU:
                    ShowingSettingsTab = false;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = false;
                    ShowingGPUTab = true;
                    break;
                case CurrentTab.Settings:
                    ShowingSettingsTab = true;
                    ShowingSummaryTab = false;
                    ShowingCPUTab = false;
                    ShowingGPUTab = false;
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
