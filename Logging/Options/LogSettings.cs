using System;

namespace Logging.Options
{
    public class LogSettings : ILogSettings
    {
        private string _basePath;
        private string _logFilePath;
        public const string Name = "LogSettings";
        public string BasePath { get => _basePath; set => _basePath = Environment.ExpandEnvironmentVariables(value); }
        public long MaxSizeForVinLog { get; set; }
        public string LogFilePath { get => BasePath + _logFilePath; set => _logFilePath = value; }
    }
}
