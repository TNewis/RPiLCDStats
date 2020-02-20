using RPiLCDPiServer.Services.ConnectionService;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public interface IStaticDataService
    {
        string GetStaticData();
        void StartService();
        void EndService();
    }
}
