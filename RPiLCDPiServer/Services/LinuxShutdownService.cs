using System.Diagnostics;

namespace RPiLCDPiServer.Services
{
    class LinuxShutdownService : IShutdownService
    {
        public void Shutdown()
        {
            Process process = new Process();
            process.StartInfo.FileName = "/usr/bin/sudo";
            process.StartInfo.Arguments = "/sbin/shutdown -h now";
            process.Start();
        }
    }
}
