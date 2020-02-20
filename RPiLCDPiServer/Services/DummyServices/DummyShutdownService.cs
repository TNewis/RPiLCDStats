using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.Services.DummyServices
{
    class DummyShutdownService : IShutdownService
    {
        public void Shutdown()
        {
            Console.WriteLine("Woosh your computer is off now.");
        }
    }
}
