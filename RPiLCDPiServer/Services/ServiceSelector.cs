using RPiLCDPiServer.Services.ConnectionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiLCDPiServer.Services
{
    class ServiceSelector
    {
        //For a little program like this it just doesn't seem worth setting up DI containers, so we'll just have a little class that we can ask for services.
        //What the fuck is the point of this man. How is this any better than manually instantiating all your static services and getting the one you need by name?

        private static readonly IShutdownService _shutdownService;
        private static readonly IDisplayControlService _displayControlService;
        private static readonly IConnectionService _connectionService;

        readonly List<IService> _serviceList= new List<IService>();

        public ServiceSelector()
        {
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
        }

        public T GetServiceInstance<T>()
        {
            T service=(T)_serviceList.First(s => typeof(T).IsAssignableFrom(s.GetType()));
            return service;
        }
    }
}
