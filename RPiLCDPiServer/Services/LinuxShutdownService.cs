using RPiLCDShared.Services;
using System.Diagnostics;

namespace RPiLCDPiServer.Services
{
    class LinuxShutdownService : IShutdownService
    {
        private ILoggingService _loggingService;
        public void SetLoggingService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }
        public void Shutdown()
        {
            Process process = new Process();
            process.StartInfo.FileName = "/usr/bin/sudo";
            process.StartInfo.Arguments = "/sbin/shutdown -h now";
            process.Start();
        }
    }
}
