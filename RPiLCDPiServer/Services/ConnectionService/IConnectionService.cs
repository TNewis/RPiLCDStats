using RPiLCDShared.Services;

namespace RPiLCDPiServer.Services.ConnectionService
{
    public interface IConnectionService: IService
    {
        public void OpenConnection();
        public void CloseConnection();
        public void IncludeInResponse(string response);
    }
}