using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.Settings
{
    static class DisplaySettings
    {
        public static int ScreenWidth { get; set; } = 790;
        public static int ScreenHeight { get; set; } = 470;

        public static int ScreenWidthWithoutSidebar { get; set; } = ScreenWidth - 64;
        public static int ScreenHeightWithoutButton { get; set; } = ScreenHeight - 64;
    }
}
