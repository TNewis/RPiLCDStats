using ReactiveUI;
using RPiLCDPiServer.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RPiLCDPiServer.ViewModels
{
    public class ConsoleTabViewModel : ViewModelBase
    {
        private Timer _timer;

        private string _consoleString = "No Entries";
        public string ConsoleString
        {
            get { return _consoleString; }
            set { this.RaiseAndSetIfChanged(ref _consoleString, value); }
        }

        private StringWriter _consoleStringWriter;

        public int ScreenWidth { get; set; }
        public int ScreenWidthWithoutSidebar { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenHeightWithoutButton { get; set; }

        public ConsoleTabViewModel()
        {
            //_consoleStringWriter = new StringWriter();
            //Console.SetOut(_consoleStringWriter);
            //Console.SetError(_consoleStringWriter);
            //ConsoleString = _consoleStringWriter.ToString();

            ScreenWidth = DisplaySettings.ScreenWidth;
            ScreenWidthWithoutSidebar = DisplaySettings.ScreenWidthWithoutSidebar;
            ScreenHeight = DisplaySettings.ScreenHeight;
            ScreenHeightWithoutButton = DisplaySettings.ScreenHeightWithoutButton;

            //_timer = new Timer(1000);
            //_timer.AutoReset = true;
            //_timer.Elapsed += async (sender, e) => await UpdateConsoleString();
            //_timer.Start();
        }

        //private Task UpdateConsoleString()
        //{
        //    return Task.Run(() =>
        //    {
        //        ConsoleString = _consoleStringWriter.ToString();
        //    });

        //}
    }
}
