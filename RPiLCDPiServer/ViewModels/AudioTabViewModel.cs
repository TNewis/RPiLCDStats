using ReactiveUI;
using RPiLCDPiServer.Services;
using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDPiServer.Settings;
using RPiLCDShared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.ViewModels
{
    public class AudioTabViewModel : UpdateableViewModelBase
    {
        public IReactiveCommand SetSpeakerAudioCommand { get; set; }
        public IReactiveCommand SetHeadphoneAudioCommand { get; set; }
        //public IReactiveCommand MuteAudioCommand { get; set; }
        //public IReactiveCommand UnmuteAudioCommand { get; set; }

        private readonly IConnectionService _connectionService;

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

        public AudioTabViewModel()
        {
            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            SetSpeakerAudioCommand = ReactiveCommand.Create(() => { SetAudioOutputToSpeaker(); });
            SetHeadphoneAudioCommand = ReactiveCommand.Create(() => { SetAudioOutputToHeadphone(); });
            //MuteAudioCommand = ReactiveCommand.Create(() => { MuteAudio(); });
            //UnmuteAudioCommand = ReactiveCommand.Create(() => { UnmuteAudio(); });

            var serviceSelector = new ServiceSelector();
            _connectionService = serviceSelector.GetServiceInstance<IConnectionService>();
            ConnectionService.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;
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
            //_connectionService.IncludeInResponse(UpdateTags.AudioVolumeTag.TagOpen + "0" + UpdateTags.AudioVolumeTag.TagClose);
            //AudioIsMuted = true;
        }

        private void UnmuteAudio()
        {
            //_connectionService.IncludeInResponse(UpdateTags.AudioVolumeTag.TagOpen + _lastNonZeroVolumeInt + UpdateTags.AudioVolumeTag.TagClose);
            //AudioIsMuted = false;
        }

        protected override void ProcessUpdate(string message)
        {
            var audioString = message.Substring(UpdateTags.AudioDeviceTag.TagOpen, UpdateTags.AudioDeviceTag.TagClose);
            if (audioString != null)
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
                    if (audioString.Contains(UpdateTags.AudioVolumeTag.TagText))
                    {
                        var volumeString = audioString.Substring(UpdateTags.AudioVolumeTag.TagOpen, UpdateTags.AudioVolumeTag.TagClose);

                        _volumeInt = int.Parse(volumeString);
                        Volume = _volumeInt.ToString();

                        if (_volumeInt > 0)
                        {
                            _lastNonZeroVolumeInt = _volumeInt;
                        }
                    }
                }

            }
        }
    }
}
