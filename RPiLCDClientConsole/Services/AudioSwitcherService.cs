using AudioSwitcher.AudioApi.CoreAudio;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using RPiLCDShared.Services;

namespace RPiLCDClientConsole.Services
{
    public class AudioSwitcherService : UpdateableService, IAudioSwitcherService, IUpdatableService, IStaticDataService
    {
        private readonly ILoggingService _loggingService;

        private readonly string _savedDevicesFilePath = "SavedAudioDevices.xml";
        private readonly CoreAudioController _controller;
        private readonly SortedList<AudioDeviceOptions, CoreAudioDevice> _devices;
        private readonly Tag _tabTag = UpdateTags.AudioDeviceTag;
        private bool _audioUpdateNeeded = true;
        private AudioDeviceType _currentType = AudioDeviceType.Other;

        private int _previousVolume=0;
        private bool _previouslyMuted = false;

        public AudioSwitcherService(ILoggingService loggingService)
        {
            _loggingService = loggingService;

            PCClient.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;

            _controller = new CoreAudioController();
            _devices = new SortedList<AudioDeviceOptions, CoreAudioDevice>();

            UpdateSavedDevices();
        }

        private void HandleUpdateRecievedEvent(object sender, UpdateRecievedEventArgs a)
        {
            var audioUpdate = a.UpdateString.Substring(UpdateTags.AudioDeviceTag.TagOpen, UpdateTags.AudioDeviceTag.TagClose);
            if (audioUpdate != null)
            {
                if (audioUpdate.Contains(UpdateTags.AudioHeadphoneTag.TagText))
                {
                    SelectDefaultHeadphones();
                    _audioUpdateNeeded = true;
                }

                if (audioUpdate.Contains(UpdateTags.AudioSpeakerTag.TagText))
                {
                    SelectDefaultSpeaker();
                    _audioUpdateNeeded = true;
                }

                if (audioUpdate.Contains(UpdateTags.AudioVolumeTag.TagOpen))
                {
                    var newVolume = audioUpdate.Substring(UpdateTags.AudioVolumeTag.TagOpen, UpdateTags.AudioVolumeTag.TagClose);
                    SetDefaultDeviceVolume(int.Parse(newVolume));
                    _audioUpdateNeeded = true;
                }
            }

            var volumeUpdate = a.UpdateString.Substring(UpdateTags.AudioVolumeTag.TagOpen, UpdateTags.AudioVolumeTag.TagClose);
            if (volumeUpdate != null)
            {
                SetDefaultDeviceVolume(int.Parse(volumeUpdate));
                _audioUpdateNeeded = true;
            }

            var muteUpdate = a.UpdateString.Substring(UpdateTags.AudioVolumeMuteTag.TagOpen, UpdateTags.AudioVolumeMuteTag.TagClose);
            if (muteUpdate != null)
            {
                ToggleMute(Boolean.Parse(muteUpdate));
                _audioUpdateNeeded = true;
            }
        }

        private void UpdateSavedDevices()
        {
            List<AudioDeviceOptions> savedDevices = GetSavedDevicesFromFile();

            bool savedDevicesAdded = false;

            foreach (CoreAudioDevice device in _controller.GetPlaybackDevices())
            {
                if (savedDevices.Any(d => d.Name == device.Name))
                {
                    var savedDevice = savedDevices.First(d => d.Name == device.Name);
                    AddActiveAudioDevice(device, savedDevice);

                    continue;
                }

                var newSavedDevice = new AudioDeviceOptions(device.Id, device.Name) { Type = SetTypeOfAudioDeviceByIcon(device), IsDefault= device.IsDefaultDevice };
                AddActiveAudioDevice(device, newSavedDevice);

                savedDevices.Add(newSavedDevice);
                savedDevicesAdded = true;
            }

            if (savedDevicesAdded)
            {
                XMLSerialiser.WriteToXmlFile(_savedDevicesFilePath, savedDevices, false);
            }
        }

        private void AddActiveAudioDevice(CoreAudioDevice device, AudioDeviceOptions deviceOptions)
        {
            if (!(device.State == AudioSwitcher.AudioApi.DeviceState.Unplugged) && !(device.State == AudioSwitcher.AudioApi.DeviceState.NotPresent))
            {
                _devices.Add(deviceOptions, device);
                if (device.IsDefaultDevice)
                {
                    _currentType = deviceOptions.Type;
                }
            }
        }

        private AudioDeviceType SetTypeOfAudioDeviceByIcon(CoreAudioDevice device)
        {
            switch (device.Icon)
            {
                case AudioSwitcher.AudioApi.DeviceIcon.Headphones:
                case AudioSwitcher.AudioApi.DeviceIcon.Headset:
                    return AudioDeviceType.Headphones;
                case AudioSwitcher.AudioApi.DeviceIcon.Speakers:
                case AudioSwitcher.AudioApi.DeviceIcon.StereoMix:
                case AudioSwitcher.AudioApi.DeviceIcon.Monitor:
                    return AudioDeviceType.Speaker;
                default:
                    return AudioDeviceType.Other;
            }
        }

        private List<AudioDeviceOptions> GetSavedDevicesFromFile()
        {
            List<AudioDeviceOptions> savedDevices;
            if (File.Exists(_savedDevicesFilePath))
            {
                savedDevices = XMLSerialiser.ReadFromXmlFile<List<AudioDeviceOptions>>(_savedDevicesFilePath);
            }
            else
            {
                savedDevices = new List<AudioDeviceOptions>();
            }

            return savedDevices;
        }

        public void SelectDefaultSpeaker()
        {
            SelectDefaultAudioDeviceForType(AudioDeviceType.Speaker);
            _currentType = AudioDeviceType.Speaker;
        }

        public void SelectDefaultHeadphones()
        {
            SelectDefaultAudioDeviceForType(AudioDeviceType.Headphones);
            _currentType = AudioDeviceType.Headphones;
        }

        private void SelectDefaultAudioDeviceForType(AudioDeviceType audioDeviceType)
        {
            var device = _devices.FirstOrDefault(d => d.Key.IsDefault == true && d.Key.Type == audioDeviceType).Value;
            if (device==null)
            {
                _loggingService.LogWarning("No "+audioDeviceType.ToString()+ " audio device found to switch to. Is an enabled audio device set as default in SavedAudioDevices.xml?");
            }
            device.SetAsDefaultAsync();
        }

        private void SetDefaultDeviceVolume(int volume)
        {     
            if(volume == 0)
            {
                _controller.DefaultPlaybackDevice.Mute(true);
            }
            else
            {
                if (_controller.DefaultPlaybackDevice.IsMuted)
                {
                    _controller.DefaultPlaybackDevice.Mute(false);
                }
                _controller.DefaultPlaybackDevice.Volume = volume;
            }
        }

        private void ToggleMute(bool mute)
        {
            _controller.DefaultPlaybackDevice.Mute(mute);
        }

        private void SetDeviceOptionDefault(Guid deviceId, bool isDefault)
        {
            _devices.First(d => d.Key.Id == deviceId).Key.IsDefault = isDefault;
            List<AudioDeviceOptions> savedDevices = XMLSerialiser.ReadFromXmlFile<List<AudioDeviceOptions>>(_savedDevicesFilePath);
            savedDevices.First(d => d.Id == deviceId).IsDefault = isDefault;
            XMLSerialiser.WriteToXmlFile(_savedDevicesFilePath, savedDevices, false);
        }

        private void SetDeviceType(Guid deviceId, AudioDeviceType type)
        {
            _devices.First(d => d.Key.Id == deviceId).Key.Type = type;
            List<AudioDeviceOptions> savedDevices = XMLSerialiser.ReadFromXmlFile<List<AudioDeviceOptions>>(_savedDevicesFilePath);
            savedDevices.First(d => d.Id == deviceId).Type = type;
            XMLSerialiser.WriteToXmlFile(_savedDevicesFilePath, savedDevices, false);
        }

        public Tag GetTabTag()
        {
            return _tabTag;
        }

        public string GetUpdateString()
        {
            var audioVolume = (int)_controller.DefaultPlaybackDevice.Volume;
            if (audioVolume!= _previousVolume)
            {
                _previousVolume = audioVolume;
                _audioUpdateNeeded=true;
            }

            var isMuted = _controller.DefaultPlaybackDevice.IsMuted;
            if (isMuted!= _previouslyMuted)
            {
                _previouslyMuted = isMuted;
                _audioUpdateNeeded = true;
            }

            if (_audioUpdateNeeded)
            {
                var volumeString = TagifyString(_controller.DefaultPlaybackDevice.Volume.ToString(), UpdateTags.AudioVolumeTag);
                var muteString = TagifyString(isMuted.ToString(), UpdateTags.AudioVolumeMuteTag);

                if (_currentType== AudioDeviceType.Headphones)
                {
                    _audioUpdateNeeded = false;
                    return String.Format("{0}{1}{2}", volumeString, TagifyString(UpdateTags.AudioHeadphoneTag.TagText, _tabTag), muteString);
                }
                if (_currentType == AudioDeviceType.Speaker)
                {
                    _audioUpdateNeeded = false;
                    return String.Format("{0}{1}{2}", volumeString, TagifyString(UpdateTags.AudioSpeakerTag.TagText, _tabTag), muteString);
                }
            }

            _audioUpdateNeeded = false;
            return "";
        }

        public void StartService()
        {
        }

        public void EndService()
        {
        }

        public string GetStaticData()
        {
            var volumeString= TagifyString(_controller.DefaultPlaybackDevice.Volume.ToString(), UpdateTags.AudioVolumeTag);
            string deviceString;

            if(_currentType== AudioDeviceType.Headphones)
            {
                deviceString= TagifyString(UpdateTags.AudioHeadphoneTag.TagText, UpdateTags.AudioDeviceTag);
            }
            else
            {
                deviceString= TagifyString(UpdateTags.AudioSpeakerTag.TagText, UpdateTags.AudioDeviceTag);
            }

            return String.Format("{0}{1}", volumeString, deviceString);
        }
    }

    [Serializable]
    public class AudioDeviceOptions : IComparable<AudioDeviceOptions>
    {
        public string Name;
        public Guid Id;
        public bool IsDefault;
        public AudioDeviceType Type;

        public AudioDeviceOptions(Guid id, string name)
        {
            Name = name;
            Id = id;
            IsDefault = false;
            Type = AudioDeviceType.Speaker;
        }

        public AudioDeviceOptions()
        {
        }

        public int CompareTo(AudioDeviceOptions obj)
        {
            return Id.CompareTo(obj.Id);
        }
    }

    public enum AudioDeviceType
    {
        Headphones,
        Speaker,
        Other
    }

    public static class XMLSerialiser
    {
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
