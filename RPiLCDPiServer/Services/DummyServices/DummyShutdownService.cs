using RPiLCDShared.Services;

namespace RPiLCDPiServer.Services.DummyServices
{
    class DummyShutdownService : IShutdownService
    {
        private ILoggingService _loggingService;
        public void SetLoggingService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }
        public void Shutdown()
        {
            _loggingService.LogMessage("Woosh your computer is off now.");
        }

        public void Restart()
        {
            _loggingService.LogMessage("Woosh your computer done a restart now.");
        }
    }
}
