using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.Services
{
    public interface IShutdownService: IService
    {
        void Shutdown();
    }
}
