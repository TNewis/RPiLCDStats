using RPiLCDPiServer.Services.ConnectionService;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public interface IUpdatableService
    {
        Tag GetTabTag();
        string GetUpdateString();

        void StartService();
        void EndService();
    }
}
