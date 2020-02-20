using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDClientConsole.Libraries;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public class UpdatableGPUzMonitorService : UpdateableService, IUpdatableService, IStaticDataService
    {
        private readonly ILoggingService _loggingService;
        private Tag _tabTag = UpdateTags.GPUTabTag;

        private GPUzWrapper _gpuz;
        private bool _gpuzMemoryOpen;

        public UpdatableGPUzMonitorService(ILoggingService loggingService)
        {
            _gpuz = new GPUzWrapper();
        }


        public Tag GetTabTag()
        {
            return _tabTag;
        }

        public string GetUpdateString()
        {
            try
            {
                var load = _gpuz.SensorValue(6);
                var gpuMemory = _gpuz.SensorValue(5);
                var gpuMemoryTotal = _gpuz.DataValue(35);
                var pcMemory = _gpuz.SensorValue(15);
                var gpuTemperature = _gpuz.SensorValue(2);
                var cpuTemperature = _gpuz.SensorValue(14);

                var updateMessage = TagifyString(load.ToString(), UpdateTags.GPULoadTag)
                    + TagifyString(gpuMemory.ToString(), UpdateTags.GPUMemoryTag)
                    + TagifyString(gpuMemoryTotal.ToString(), UpdateTags.GPUMemoryTotalTag)
                    + TagifyString(pcMemory.ToString(), UpdateTags.PCMemoryTag)
                    + TagifyString(gpuTemperature.ToString(), UpdateTags.GPUTemperatureTag)
                    + TagifyString(cpuTemperature.ToString(), UpdateTags.CPUTemperatureTag);


                return updateMessage;
            }
            catch (Exception e)
            {
                _loggingService.LogError("Error opening GPUz shared memory.");
            }

            return "";
        }

        public void StartService()
        {
            if (_gpuzMemoryOpen == false)
            {
                _gpuz.Open();
                _gpuzMemoryOpen = true;
            }
        }

        public void EndService()
        {
            if(_gpuzMemoryOpen == true) 
            {
                _gpuz.Close();
                _gpuzMemoryOpen = false;
            }
        }

        public string GetStaticData()
        {
            var gpuTemperatureUnits = _gpuz.SensorUnit(2);
            if (gpuTemperatureUnits.Contains("F"))
            {
                return TagifyString(" F", UpdateTags.TemperatureUnitsTag);
            }

            return TagifyString(" C", UpdateTags.TemperatureUnitsTag);
        }
    }
}
