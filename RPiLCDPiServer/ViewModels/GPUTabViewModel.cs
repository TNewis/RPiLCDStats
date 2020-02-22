using ReactiveUI;
using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDPiServer.Settings;
using RPiLCDShared.Services;

namespace RPiLCDPiServer.ViewModels
{
    class GPUTabViewModel: UpdateableViewModelBase
    {
        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }

        private float _gpuLoad;
        public float GPULoad
        {
            get { return _gpuLoad; }
            set { this.RaiseAndSetIfChanged(ref _gpuLoad, value); }
        }

        private float _gpuMemory;
        public float GPUMemory
        {
            get { return _gpuMemory; }
            set { this.RaiseAndSetIfChanged(ref _gpuMemory, value); }
        }

        private string _gpuName;
        public string GPUName
        {
            get { return _gpuName; }
            set { this.RaiseAndSetIfChanged(ref _gpuName, value); }
        }

        public GPUTabViewModel()
        {
            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            ConnectionService.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;
        }

        protected override void ProcessUpdate(string message)
        {
            if (message.StartsWith(UpdateTags.GPUTabTag.TagOpen))
            {
                var gpuNameUpdate = message.Substring(UpdateTags.GPUNameTag.TagOpen, UpdateTags.GPUNameTag.TagClose);
                if (gpuNameUpdate != null)
                {
                    GPUName = gpuNameUpdate;
                }


                var gpuLoadUpdate = message.Substring(UpdateTags.GPULoadTag.TagOpen, UpdateTags.GPULoadTag.TagClose);
                if (gpuLoadUpdate != null)
                {
                    GPULoad = float.Parse(gpuLoadUpdate);
                }

                var gpuMemoryUpdate = message.Substring(UpdateTags.GPUMemoryTag.TagOpen, UpdateTags.GPUMemoryTag.TagClose);
                if (gpuMemoryUpdate != null)
                {
                    GPUMemory = float.Parse(gpuMemoryUpdate);
                }
            }
        }

    }
}
