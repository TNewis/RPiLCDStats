using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.Services
{
    interface IDisplayControlService: IService
    {
        int GetMaxBrightness();
        int GetMinBrightness();
        int GetCurrentBrightness();
        void SetBrightness(int brightness);
    }
}
