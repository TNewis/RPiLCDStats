using System;
using System.Collections.Generic;
using System.Text;

namespace RPiLCDPiServer.Services
{
    interface IFontService : IService
    {
        string SansFont();
    }
}
