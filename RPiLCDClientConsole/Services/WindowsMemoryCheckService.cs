using RPiLCDPiServer.Services.ConnectionService;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public class WindowsMemoryCheckService: UpdateableService, IStaticDataService
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);



        public string GetStaticData()
        {
            long memKb;
            GetPhysicallyInstalledSystemMemory(out memKb);
            return TagifyString((memKb / 1024).ToString(), UpdateTags.PCMemoryTotalTag);
        }

        public void EndService()
        {
        }

        public void StartService()
        {
        }
    }
}
