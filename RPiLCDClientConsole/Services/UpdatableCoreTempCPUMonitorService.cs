using RPiLCDPiServer.Services.ConnectionService;
using System;
using GetCoreTempInfoNET;
using System.Linq;

namespace RPiLCDClientConsole.Services
{
    public class UpdatableCoreTempCPUMonitorService : UpdateableService, IUpdatableService
    {
        private Tag _tabTag = UpdateTags.CPUTabTag;
        private CoreTempInfo _coreTemp;
        private readonly ILoggingService _loggingService;

        public UpdatableCoreTempCPUMonitorService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _coreTemp = new CoreTempInfo();
            _coreTemp.ReportError += new ErrorOccured(CoreTemp_ReportError);
        }

        public Tag GetTabTag()
        {
            return _tabTag;
        }

        public string GetUpdateString()
        {
            try
            {
                _coreTemp.GetData();

                var coreLoad= string.Join(",", _coreTemp.GetCoreLoad.Take(4));

                var str = coreLoad.ToString();
                return TagifyString(coreLoad.ToString(), UpdateTags.CPULoadTag);

            }
            catch (Exception e)
            {
                _loggingService.LogError(e.Message);
            }

            return "";
        }

        void CoreTemp_ReportError(ErrorCodes ErrCode, string ErrMsg)
        {
            _loggingService.LogError(ErrMsg);
        }

        public void StartService()
        {
        }

        public void EndService()
        {
        }

    }
}
