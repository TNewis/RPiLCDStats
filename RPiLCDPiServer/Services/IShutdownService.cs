using RPiLCDShared.Services;

namespace RPiLCDPiServer.Services
{
    public interface IShutdownService: IService
    {
        void Shutdown();
    }
}
