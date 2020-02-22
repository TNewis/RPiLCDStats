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
            }

        }

        private void UpdateSavedDevices()
        {
            List<AudioDeviceOptions> savedDevices = GetSavedDevicesFromFile();

            bool savedDevicesAdded = false;

            foreach (CoreAudioDevice device in _controller.GetPlaybackDevices())
            {
                if (savedDevices.Any(d => d.Id == device.Id))
                {
                    var savedDevice = savedDevices.First(d => d.Id == device.Id);
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
            if (_audioUpdateNeeded)
            {
                if(_currentType== AudioDeviceType.Headphones)
                {
                    _audioUpdateNeeded = false;
                    return TagifyString(UpdateTags.AudioHeadphoneTag.TagText, _tabTag);
                }
                if (_currentType == AudioDeviceType.Speaker)
                {
                    _audioUpdateNeeded = false;
                    return TagifyString(UpdateTags.AudioSpeakerTag.TagText, _tabTag);
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
            if(_currentType== AudioDeviceType.Headphones)
            {
                return TagifyString(UpdateTags.AudioHeadphoneTag.TagText, UpdateTags.AudioDeviceTag);
            }
            return TagifyString(UpdateTags.AudioSpeakerTag.TagText, UpdateTags.AudioDeviceTag);
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
