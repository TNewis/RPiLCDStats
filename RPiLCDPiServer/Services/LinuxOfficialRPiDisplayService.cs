using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace RPiLCDPiServer.Services
{
    class LinuxOfficialRPiDisplayService : IDisplayControlService
    {
        public int GetMaxBrightness()
        {
            return 255;
        }

        public int GetMinBrightness()
        {
            return 6;
        }

        public int GetCurrentBrightness()
        {
            var brightnessString = File.ReadAllText("/sys/class/backlight/rpi_backlight/brightness");
            int.TryParse(brightnessString, out int brightnessInt);
            return brightnessInt;
        }

        public void SetBrightness(int brightness)
        {
            //Normally I'd clean this up, but I'm leaving it because why the fuck didn't this work
            //in the end we just run the entire program with sudo, which we should fix, but fuck right off.

            //Process process = new Process();
            //process.StartInfo.FileName = "/usr/bin/sudo";
            ////var command = "/bin/sh -c \"shutdown -r now\"";//nope
            ////var command = "sh -c \"shutdown -r now\"";//nope
            ////var command = "bash -c \"shutdown -r now\"";//nope
            ////var command = "/bin/bash -c \"shutdown -r now\"";//nope
            ////var command = "/bin/echo 'c# echo' | tee /out.txt";//nope
            ////var command = "/bin/echo 'c# echo' | /usr/bin/tee /out.txt";//none

            ////var command = "/bin/echo \"test\"> /out.txt";//this will be denied

            ////var command = "/usr/bin/bash -c 'echo \"" + brightness + "\"> /sys/class/backlight/rpi_backlight/brightness'";
            //command = command.Replace("\"", "\\\"");
            //process.StartInfo.Arguments = command;
            //process.Start();
            //process.WaitForExit();

            //return process.StandardOutput.ReadToEnd();

            string[] lines = { brightness.ToString() };
            File.WriteAllLines(@"/sys/class/backlight/rpi_backlight/brightness", lines);
        }
    }
}
