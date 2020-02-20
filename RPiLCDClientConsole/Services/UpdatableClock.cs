using RPiLCDPiServer.Services.ConnectionService;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public class UpdatableClock : UpdateableService, IUpdatableService
    {
        //Updateable Clock exists for testing update builder functionality.

        private Tag _tag = UpdateTags.ClockTag;
        private Tag _tabTag = UpdateTags.ClockTabTag;

        public Tag GetUpdateTag()
        {
            return _tag;
        }
        public string GetUpdateString()
        {
            return TagifyString(DateTime.Now.ToString(), _tag);
        }

        public Tag GetTabTag()
        {
            return _tabTag;
        }

        public void StartService()
        {
        }

        public void EndService()
        {
        }
    }
}
