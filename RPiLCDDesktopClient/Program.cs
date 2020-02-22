using RPiLCDClientConsole;
using RPiLCDClientConsole.Services;
using RPiLCDClientConsole.Update;
using RPiLCDShared.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPiLCDDesktopClient
{
    static class Program
    {
        private static PCClient _client;
        private static string _message;
        private static IUpdateBuilder _updateBuilder;
        private static ILoggingService _loggingService;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Client...");

            System.Timers.Timer _timer;

            _client = new PCClient();
            _updateBuilder = new UpdateBuilder();

            AddAllServices(_updateBuilder);

            _timer = new System.Timers.Timer(1000){ AutoReset = true };
            _timer.Elapsed += async (sender, e) => await Update();
            _timer.Start();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static Task Update()
        {
            return Task.Run(() =>
            {
                _message = _updateBuilder.BuildFullUpdateString();
                _client.SendUpdate(_message);
            });
        }

        private static void AddAllServices(IUpdateBuilder updateBuilder)
        {
            _loggingService = new LogToConsoleService();

            updateBuilder.AddUpdatableService(new UpdatableClock());
            updateBuilder.AddUpdatableService(new UpdatableGPUzMonitorService(_loggingService));

            updateBuilder.AddStaticDataService(new WindowsMemoryCheckService());
        }
    }
}
