using RPiLCDPiServer.Services.ConnectionService;
using RPiLCDClientConsole.Services;
using System.Collections.Generic;

namespace RPiLCDClientConsole.Update
{
    public interface IUpdateBuilder
    {
        public List<IUpdatableService> GetUpdatableServices();
        public List<Tag> GetTabTags();
        public void AddUpdatableService(IUpdatableService updatableService);
        public void AddStaticDataService(IStaticDataService staticDataService);
        public void StartAllServices();
        public string BuildFullUpdateString();
        public string BuildUpdateString();
        public string BuildStaticDataString();
    }
}
