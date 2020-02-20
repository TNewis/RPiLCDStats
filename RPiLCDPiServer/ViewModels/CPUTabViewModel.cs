using ReactiveUI;
using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDPiServer.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.ViewModels
{

    public class CPUTabViewModel : UpdateableViewModelBase
    {
        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }

        private string _cpuName;
        public string CPUName
        {
            get { return _cpuName; }
            set { this.RaiseAndSetIfChanged(ref _cpuName, value); }
        }

        public CPUTabViewModel()
        {
            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            ConnectionService.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;
        }

        protected override void ProcessUpdate(string message)
        {
            //if (message.StartsWith(tabTag.TagOpen))
            //{
            //    CPUName = ExtractStringData(message, UpdateTags.CPUNameTag);
            //}
        }
    }
}
