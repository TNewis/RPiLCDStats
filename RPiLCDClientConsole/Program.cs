using RPiLCDClientConsole.Services;
using RPiLCDClientConsole.Update;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace RPiLCDClientConsole
{
    class Program
    {
        private static ILoggingService _loggingService;
        private static PCClient _client;
        private static string _message;
        private static string _staticData;
        private static IUpdateBuilder _updateBuilder;

        private static bool _staticDataSent;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Client...");

            Timer _timer;

            _loggingService = new LogToConsoleService();
            _client = new PCClient();
            _updateBuilder = new UpdateBuilder();

            AddAllServices(_updateBuilder);

            _timer = new Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += async (sender, e) => await Update();
            _timer.Start();

            while (true)
            {

            }
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
            updateBuilder.AddUpdatableService(new UpdatableClock());

            updateBuilder.AddUpdatableService(new UpdatableCoreTempCPUMonitorService(_loggingService));

            var gpuStatsService = new UpdatableGPUzMonitorService(_loggingService);
            updateBuilder.AddUpdatableService(gpuStatsService);
            updateBuilder.AddStaticDataService(gpuStatsService);

            var audioSwitcher = new AudioSwitcherService(_loggingService);
            updateBuilder.AddUpdatableService(audioSwitcher);
            updateBuilder.AddStaticDataService(audioSwitcher);

            updateBuilder.AddStaticDataService(new WindowsMemoryCheckService());

            updateBuilder.StartAllServices();
        }
    }
}
