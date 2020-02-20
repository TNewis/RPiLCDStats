using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.Services.DummyServices
{
    class DummyDisplayControlService : IDisplayControlService
    {
        public int GetMaxBrightness()
        {
            return 100;
        }

        public int GetMinBrightness()
        {
            return 0;
        }

        public int GetCurrentBrightness()
        {
            return 50;
        }

        public void SetBrightness(int brightness)
        {
            Console.WriteLine("Woosh screen brightness is now "+ brightness);
        }
    }
}
