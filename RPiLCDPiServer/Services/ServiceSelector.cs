using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDShared.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPiLCDPiServer.Services
{
    class ServiceSelector
    {
        //For a little program like this it just doesn't seem worth setting up DI containers, so we'll just have a little class that we can ask for services.
        //What the fuck is the point of this man. How is this any better than manually instantiating all your static services and getting the one you need by name?

        private static ILoggingService _loggingService=null;
        private static readonly IShutdownService _shutdownService;
        private static readonly IDisplayControlService _displayControlService;
        private static readonly IConnectionService _connectionService;

        readonly List<IService> _serviceList= new List<IService>();

        public ServiceSelector()
        {
            if (_loggingService == null)
            {
                _loggingService = new LogToEventService();
            }

            if (Environment.OSVersion.Platform== PlatformID.Win32NT)
            {

                InstantiateService<DummyServices.DummyShutdownService>(_shutdownService);
                InstantiateService<DummyServices.DummyDisplayControlService>(_displayControlService);
            }
            else
            {
                InstantiateService<LinuxShutdownService>(_shutdownService);
                InstantiateService<LinuxOfficialRPiDisplayService>(_displayControlService);
            }

            InstantiateService<LANStatsServer>(_connectionService);

        }

        private void InstantiateService<T>(IService service) where T : IService
        {
            if (service == null)
            {
                service = (T)Activator.CreateInstance(typeof(T));
            }
            _serviceList.Add(service);
            service.SetLoggingService(_loggingService);
        }

        public T GetServiceInstance<T>()
        {
            T service=(T)_serviceList.First(s => typeof(T).IsAssignableFrom(s.GetType()));
            return service;
        }

        public ILoggingService GetLoggingService()
        {
            return _loggingService;
        }

        public void SetLoggingService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }
    }
}
