using RPiLCDShared.Services;

namespace RPiLCDPiServer.Services.DummyServices
{
    class DummyDisplayControlService : IDisplayControlService
    {
        private ILoggingService _loggingService;
        public void SetLoggingService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

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
            _loggingService.LogMessage("Woosh screen brightness is now "+ brightness);
        }
    }
}
