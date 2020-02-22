using RPiLCDShared.Services;

namespace RPiLCDPiServer.Services
{
    interface IFontService : IService
    {
        string SansFont();
    }
}
