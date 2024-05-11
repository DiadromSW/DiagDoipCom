using NLog;

namespace Logging
{
    public interface ILogHandler
    {
        void StartLogging(string vin, LogLevel logLevel);
        void StopLogging(string vin);
        void DeleteLogs(string vin);
        Task<string> GetLogs(string vin);
        void DeleteZipFile(string filename);
        Task<List<ActiveLogging>> GetLogStatus();
    }
}