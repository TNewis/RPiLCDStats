namespace RPiLCDClientConsole.Services
{
    public interface IStaticDataService
    {
        string GetStaticData();
        void StartService();
        void EndService();
    }
}
