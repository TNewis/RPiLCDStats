using ReactiveUI;
using RPiLCDPiServer.Services;
using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDPiServer.Settings;
using RPiLCDShared.Services;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace RPiLCDPiServer.ViewModels
{
    public class SummaryTabViewModel : UpdateableViewModelBase
    {
        public IReactiveCommand SetSpeakerAudioCommand { get; set; }
        public IReactiveCommand SetHeadphoneAudioCommand { get; set; }
        public IReactiveCommand MuteAudioCommand { get; set; }
        public IReactiveCommand UnmuteAudioCommand { get; set; }

        private readonly IConnectionService _connectionService;
        public string FontSans { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }

        private int _volumeInt;
        private int _lastNonZeroVolumeInt;
        private string _volume;
        public string Volume
        {
            get { return _volume; }
            set { this.RaiseAndSetIfChanged(ref _volume, value); }
        }

        private bool _audioIsMuted;
        public bool AudioIsMuted
        {
            get { return _audioIsMuted; }
            set { this.RaiseAndSetIfChanged(ref _audioIsMuted, value); }
        }

        private string _temperatureUnitString;

        private bool _staticDataReceived=false;
        private readonly Timer _timer;

        private bool _audioSwapButtonDisabled;
        public bool AudioSwapButtonDisabled
        {
            get { return _audioSwapButtonDisabled; }
            set { this.RaiseAndSetIfChanged(ref _audioSwapButtonDisabled, value); }
        }

        private bool _currentAudioIsSpeaker;
        public bool CurrentAudioIsSpeaker
        {
            get { return _currentAudioIsSpeaker; }
            set { this.RaiseAndSetIfChanged(ref _currentAudioIsSpeaker, value); }
        }

        private string _timeString = DateTime.Now.ToLongTimeString();
        public string TimeString
        {
            get { return _timeString; }
            set { this.RaiseAndSetIfChanged(ref _timeString, value); }
        }

        private string _dateString = DateTime.Now.ToLongDateString();
        public string DateString
        {
            get { return _dateString; }
            set { this.RaiseAndSetIfChanged(ref _dateString, value); }
        }

        private string _gpuLoadString;
        public string GPULoadString
        {
            get { return _gpuLoadString; }
            set { this.RaiseAndSetIfChanged(ref _gpuLoadString, value); }
        }

        private float _gpuLoadFloat;
        public float GPULoadFloat
        {
            get { return _gpuLoadFloat; }
            set { this.RaiseAndSetIfChanged(ref _gpuLoadFloat, value); }
        }

        private string _gpuMemoryString;
        public string GPUMemoryString
        {
            get { return _gpuMemoryString; }
            set { this.RaiseAndSetIfChanged(ref _gpuMemoryString, value); }
        }

        private float _gpuMemoryBarWidth;
        public float GPUMemoryBarWidth
        {
            get { return _gpuMemoryBarWidth; }
            set { this.RaiseAndSetIfChanged(ref _gpuMemoryBarWidth, value); }
        }

        private string _systemMemoryTotalString;
        private string _systemMemoryUsageString;

        public string SystemMemoryUsageString
        {
            get { return _systemMemoryUsageString; }
            set { this.RaiseAndSetIfChanged(ref _systemMemoryUsageString, value); }
        }

        private float _systemMemoryBarWidth;
        public float SystemMemoryBarWidth
        {
            get { return _systemMemoryBarWidth; }
            set { this.RaiseAndSetIfChanged(ref _systemMemoryBarWidth, value); }
        }

        private float _cpu1LoadBarWidth;
        public float CPU1LoadBarWidth
        {
            get { return _cpu1LoadBarWidth; }
            set { this.RaiseAndSetIfChanged(ref _cpu1LoadBarWidth, value); }
        }

        private float _cpu2LoadBarWidth;
        public float CPU2LoadBarWidth
        {
            get { return _cpu2LoadBarWidth; }
            set { this.RaiseAndSetIfChanged(ref _cpu2LoadBarWidth, value); }
        }

        private float _cpu3LoadBarWidth;
        public float CPU3LoadBarWidth
        {
            get { return _cpu3LoadBarWidth; }
            set { this.RaiseAndSetIfChanged(ref _cpu3LoadBarWidth, value); }
        }

        private float _cpu4LoadBarWidth;
        public float CPU4LoadBarWidth
        {
            get { return _cpu4LoadBarWidth; }
            set { this.RaiseAndSetIfChanged(ref _cpu4LoadBarWidth, value); }
        }

        private string _cpuLoadString;
        public string CPULoadString
        {
            get { return _cpuLoadString; }
            set { this.RaiseAndSetIfChanged(ref _cpuLoadString, value); }
        }

        private string _cpuTempString;
        public string CPUTempString
        {
            get { return _cpuTempString; }
            set { this.RaiseAndSetIfChanged(ref _cpuTempString, value); }
        }

        private string _gpuTempString;
        public string GPUTempString
        {
            get { return _gpuTempString; }
            set { this.RaiseAndSetIfChanged(ref _gpuTempString, value); }
        }

        public SummaryTabViewModel()
        {
            SetSpeakerAudioCommand = ReactiveCommand.Create(() => { SetAudioOutputToSpeaker(); });
            SetHeadphoneAudioCommand = ReactiveCommand.Create(() => { SetAudioOutputToHeadphone(); });
            MuteAudioCommand = ReactiveCommand.Create(() => { MuteAudio(); });
            UnmuteAudioCommand = ReactiveCommand.Create(() => { UnmuteAudio(); });

            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            var serviceSelector = new ServiceSelector();

            _connectionService = serviceSelector.GetServiceInstance<IConnectionService>();
            ConnectionService.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;

            _timer = new Timer(1000){ AutoReset = true };
            _timer.Elapsed += async (sender, e) => await ClockUpdate();
            _timer.Start();
        }

        private void SetAudioOutputToSpeaker()
        {
            _connectionService.IncludeInResponse(UpdateTags.AudioDeviceTag.TagOpen + UpdateTags.AudioSpeakerTag.TagText + UpdateTags.AudioDeviceTag.TagClose);
            AudioSwapButtonDisabled = true;
        }

        private void SetAudioOutputToHeadphone()
        {
            _connectionService.IncludeInResponse(UpdateTags.AudioDeviceTag.TagOpen + UpdateTags.AudioHeadphoneTag.TagText + UpdateTags.AudioDeviceTag.TagClose);
            AudioSwapButtonDisabled = true;
        }

        private void MuteAudio()
        {
            _connectionService.IncludeInResponse(UpdateTags.AudioVolumeTag.TagOpen + "0" + UpdateTags.AudioVolumeTag.TagClose);
            AudioIsMuted = true;
        }

        private void UnmuteAudio()
        {
            _connectionService.IncludeInResponse(UpdateTags.AudioVolumeTag.TagOpen + _lastNonZeroVolumeInt + UpdateTags.AudioVolumeTag.TagClose);
            AudioIsMuted = false;
        }

        private Task ClockUpdate()
        {
            return Task.Run(() =>
            {
                TimeString = DateTime.Now.ToLongTimeString();
                DateString = DateTime.Now.ToLongDateString();
            });

        }

        protected override void ProcessUpdate(string message)
        {
            if (!_staticDataReceived)
            {
                var staticDataString = message.Substring(UpdateTags.StaticDataTag.TagOpen, UpdateTags.StaticDataTag.TagClose);
                if (staticDataString != null)
                {
                    ExtractSystemMemoryTotalString(staticDataString);
                    ExtractStartingAudioOutputType(staticDataString);
                    ExtractTemperatureUnits(staticDataString);
                    _staticDataReceived = true;
                }
                else
                {
                    _connectionService.IncludeInResponse(UpdateTags.StaticDataTag.TagOpen);
                }

            }

            var gpuString = message.Substring(UpdateTags.GPUTabTag.TagOpen, UpdateTags.GPUTabTag.TagClose);
            if (gpuString != null)
            {
                ExtractGPULoad(message);
                ExtractGPUMemoryUsage(message);
                ExtractPCMemoryUsage(message);
                ExtractCPULoad(message);
                ExtractCPUTemperature(message);
                ExtractGPUTemperature(message);
            }

            var audioString = message.Substring(UpdateTags.AudioDeviceTag.TagOpen, UpdateTags.AudioDeviceTag.TagClose);
            if(audioString != null)
            {
                if (audioString.Length > 0)
                {
                    AudioSwapButtonDisabled = false;
                    if (audioString.Contains(UpdateTags.AudioHeadphoneTag.TagText))
                    {
                        CurrentAudioIsSpeaker = false;
                    }
                    if (audioString.Contains(UpdateTags.AudioSpeakerTag.TagText))
                    {
                        CurrentAudioIsSpeaker = true;
                    }
                }

            }

            var volumeString = message.Substring(UpdateTags.AudioVolumeTag.TagOpen, UpdateTags.AudioVolumeTag.TagClose);
            if (volumeString != null)
            {
                if (int.Parse(volumeString) > 0)
                {
                    AudioIsMuted = false;
                }
                else
                {
                    AudioIsMuted = true;
                }
            }
        }

        private void ExtractTemperatureUnits(string message)
        {
            var cpuTempUnit = message.Substring(UpdateTags.TemperatureUnitsTag.TagOpen, UpdateTags.TemperatureUnitsTag.TagClose);
            if (cpuTempUnit != null)
            {
                _temperatureUnitString = cpuTempUnit;
            }
        }

        private void ExtractSystemMemoryTotalString(string message)
        {
            var memory = message.Substring(UpdateTags.PCMemoryTotalTag.TagOpen, UpdateTags.PCMemoryTotalTag.TagClose);
            if (memory != null)
            {
                _systemMemoryTotalString = memory;
            }
        }

        private void ExtractStartingAudioOutputType(string message)
        {
            var outputType = message.Substring(UpdateTags.AudioDeviceTag.TagOpen, UpdateTags.AudioDeviceTag.TagClose);
            if (outputType != null)
            {
                if (outputType.Contains(UpdateTags.AudioHeadphoneTag.TagText))
                {
                    CurrentAudioIsSpeaker = false;
                    return;
                }

                CurrentAudioIsSpeaker = true;
            }
        }

        private void ExtractGPUMemoryUsage(string message)
        {
            var gpuMemory = message.Substring(UpdateTags.GPUMemoryTag.TagOpen, UpdateTags.GPUMemoryTag.TagClose);
            if (gpuMemory != null)
            {
                var gpuMemoryTotal = message.Substring(UpdateTags.GPUMemoryTotalTag.TagOpen, UpdateTags.GPUMemoryTotalTag.TagClose);
                if (gpuMemoryTotal != null)
                {
                    GPUMemoryString = gpuMemory.Split(".", 2)[0] +"/"+ gpuMemoryTotal;
                    GPUMemoryBarWidth = (float.Parse(gpuMemory) / float.Parse(gpuMemoryTotal)) * 200;
                }

            }
        }

        private void ExtractGPULoad(string message)
        {
            var gpuLoad = message.Substring(UpdateTags.GPULoadTag.TagOpen, UpdateTags.GPULoadTag.TagClose);
            if (gpuLoad != null)
            {
                GPULoadString = gpuLoad + "%";
                GPULoadFloat = float.Parse(gpuLoad) * 2;
            }
        }
        private void ExtractPCMemoryUsage(string message)
        {
            var pcMemoryUsage = message.Substring(UpdateTags.PCMemoryTag.TagOpen, UpdateTags.PCMemoryTag.TagClose);
            if (pcMemoryUsage != null)
            {
                if (_systemMemoryTotalString != null)
                {
                    SystemMemoryUsageString = pcMemoryUsage.Split(".", 2)[0] + "/" + _systemMemoryTotalString.Split(".", 2)[0];
                    SystemMemoryBarWidth = float.Parse(pcMemoryUsage) / float.Parse(_systemMemoryTotalString) * 200;
                }

            }
        }

        private void ExtractCPULoad(string message)
        {
            var cpuLoad = message.Substring(UpdateTags.CPULoadTag.TagOpen, UpdateTags.CPULoadTag.TagClose);

            if (cpuLoad != null)
            {
                var loadArray= cpuLoad.Split(",");
                var cpu0Load = float.Parse(loadArray[0]);
                var cpu1Load = float.Parse(loadArray[1]);
                var cpu2Load = float.Parse(loadArray[2]);
                var cpu3Load = float.Parse(loadArray[3]);
                CPU1LoadBarWidth = cpu0Load * 2;
                CPU2LoadBarWidth = cpu1Load * 2;
                CPU3LoadBarWidth = cpu2Load * 2;
                CPU4LoadBarWidth = cpu3Load * 2;
                CPULoadString = ((cpu0Load + cpu1Load + cpu2Load + cpu3Load)/4).ToString() + "%";
            }
        }

        private void ExtractCPUTemperature(string message)
        {
            var cpuTemp = message.Substring(UpdateTags.CPUTemperatureTag.TagOpen, UpdateTags.CPUTemperatureTag.TagClose);

            if (cpuTemp != null)
            {
                CPUTempString = cpuTemp + _temperatureUnitString;
            }
        }

        private void ExtractGPUTemperature(string message)
        {
            var gpuTemp = message.Substring(UpdateTags.GPUTemperatureTag.TagOpen, UpdateTags.GPUTemperatureTag.TagClose);

            if (gpuTemp != null)
            {
                GPUTempString = gpuTemp + _temperatureUnitString;
            }
        }
    }
}
