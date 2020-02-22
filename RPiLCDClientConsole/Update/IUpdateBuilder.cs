using RPiLCDClientConsole.Services;
using System.Collections.Generic;
using RPiLCDShared.Services;

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
