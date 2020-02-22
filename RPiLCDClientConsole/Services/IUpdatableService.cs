using RPiLCDShared.Services;

namespace RPiLCDClientConsole.Services
{
    public interface IUpdatableService
    {
        Tag GetTabTag();
        string GetUpdateString();

        void StartService();
        void EndService();
    }
}
