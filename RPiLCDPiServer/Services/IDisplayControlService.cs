using RPiLCDShared.Services;

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
