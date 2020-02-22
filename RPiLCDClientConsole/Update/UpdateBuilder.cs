using RPiLCDClientConsole.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RPiLCDShared.Services;

namespace RPiLCDClientConsole.Update
{
    public class UpdateBuilder : IUpdateBuilder
    {
        private StringBuilder _updateBuilder = new StringBuilder();
        private StringBuilder _staticBuilder = new StringBuilder();
        private bool _staticsSent = false;
        private List<IUpdatableService> _updatableServices = new List<IUpdatableService>();
        private List<IStaticDataService> _staticDataServices = new List<IStaticDataService>();
        private List<Tag> _tabTags = new List<Tag>();

        public UpdateBuilder()
        {
            PCClient.RaiseUpdateRecievedEvent += HandleUpdateRecievedEvent;
        }

        protected void HandleUpdateRecievedEvent(object sender, UpdateRecievedEventArgs a)
        {
            if (a.UpdateString.Contains(UpdateTags.StaticDataTag.TagOpen))
            {
                _staticsSent = false;
            }

        }

        public List<IUpdatableService> GetUpdatableServices()
        {
            return _updatableServices;
        }

        public List<Tag> GetTabTags()
        {
            return _tabTags;
        }

        public void AddUpdatableService(IUpdatableService updatableService)
        {
            _updatableServices.Add(updatableService);
            if (!_tabTags.Contains(updatableService.GetTabTag()))
            {
                _tabTags.Add(updatableService.GetTabTag());
            }
        }
        public void AddStaticDataService(IStaticDataService staticDataService)
        {
            _staticDataServices.Add(staticDataService);
        }

        public string BuildFullUpdateString()
        {
            if (_staticsSent)
            {
                return BuildUpdateString();
            }

            return BuildStaticDataString() + BuildUpdateString();
        }
        public string BuildUpdateString()
        {
            foreach (Tag tabTag in _tabTags)
            {
                _updateBuilder.Append(tabTag.TagOpen);
                var updateablesInTab = GetUpdatableServices().Where(t => t.GetTabTag() == tabTag);
                foreach (IUpdatableService updatableService in updateablesInTab)
                {
                    _updateBuilder.Append(updatableService.GetUpdateString());
                }
                _updateBuilder.Append(tabTag.TagClose);
            }

            var updateString = _updateBuilder.ToString();
            _updateBuilder.Clear();
            return updateString;
        }

        public string BuildStaticDataString()
        {
            _staticBuilder.Append(UpdateTags.StaticDataTag.TagOpen);
            foreach (IStaticDataService staticDataService in _staticDataServices)
            {
                _staticBuilder.Append(staticDataService.GetStaticData());
            }
            _staticBuilder.Append(UpdateTags.StaticDataTag.TagClose);

            var staticDataString = _staticBuilder.ToString();
            _staticBuilder.Clear();
            _staticsSent = true;
            return staticDataString;
        }

        public void StartAllServices()
        {
            foreach(IUpdatableService updatableService in _updatableServices)
            {
                updatableService.StartService();
            }
        }

    }
}
