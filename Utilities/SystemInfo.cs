using Microsoft.Extensions.Logging;

namespace Utilities
{
    public static class SystemInfo
    {
        private static string _diagComVersion = "Unknown";
        private static ILoggerFactory _loggerFactory;

        static SystemInfo()
        {
            _loggerFactory = new NLog.Extensions.Logging.NLogLoggerFactory();
        }

        public static void SaveDiagComVersion(string version)
        {
            _diagComVersion = version;
            LogDiagComVersion(CreateSystemLogger());
        }

        public static void LogSystemInfo(string vin)
        {
            LogDiagComVersion(CreateVinLogger(vin));
        }

        private static void LogDiagComVersion(ILogger logger)
        {
            logger.LogInformation($"DiagCom version: {_diagComVersion}.");
        }

        private static ILogger CreateSystemLogger()
        {
            return _loggerFactory.CreateLogger("DiagCom.SystemInfo");
        }

        private static ILogger CreateVinLogger(string vin)
        {
            return _loggerFactory.CreateLogger($"Vin.SystemInfo.{vin}");
        }
    }
}
