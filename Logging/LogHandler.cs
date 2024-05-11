using Logging.Exceptions;
using Logging.Options;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using System.Diagnostics;
using Utilities;
using LogLevel = NLog.LogLevel;

namespace Logging
{
    public class LogHandler : ILogHandler
    {
        private readonly IFileWrapper _fileWrapper;
        private readonly IFileRepository _fileRepository;
        private readonly string _pathToLogs;
        private readonly long _maxSizeForVinLog;
        public LogHandler(IFileWrapper fileWrapper, IFileRepository fileRepository, IOptions<LogSettings> pathToLogs)
        {
            _fileWrapper = fileWrapper;
            _fileRepository = fileRepository;
            _pathToLogs = pathToLogs.Value.LogFilePath;
            _maxSizeForVinLog = pathToLogs.Value.MaxSizeForVinLog;
        }

        // Used from tests to avoid polluting real logs.
        public static void SetSystemLogFolderPath(string folderPath)
        {
            var systemLogger = LogManager.Configuration.FindTargetByName("SystemLogger") as FileTarget;
            Debug.Assert(systemLogger != null, "Could not find NLog file target \"SystemLogger\".");
            var filename = Path.GetFileName(systemLogger.FileName.ToString()!);
            var logFilePath = Path.Combine(folderPath, filename);
            systemLogger.FileName = logFilePath;
            LogManager.ReconfigExistingLoggers();
        }

        public static string GetSystemLogFilePath()
        {
            var systemLogger = LogManager.Configuration.FindTargetByName("SystemLogger") as FileTarget;
            Debug.Assert(systemLogger != null, "Could not find NLog file target \"SystemLogger\".");
            return systemLogger.FileName.ToString()!;
        }

        /// <summary>
        /// Returns a stream from a zip file containing logs for a specific VIN
        /// if successful, otherwise throws NoLogsFoundException
        /// </summary>
        /// <param name="vin">VIN of logs to be fetched</param>
        /// <returns>A stream</returns>
        public async Task<string> GetLogs(string vin)
        {
            var pathToZipFile = await _fileWrapper.CreateZipFileFromDirectory(vin, _pathToLogs);
            return pathToZipFile;
        }

        /// <summary>
        /// Deletes log files belonging to a specific VIN if no VIN is requested remove all logs
        /// and Error30s logs if there only exists log files for one VIN
        /// </summary>
        /// <param name="vin">Name of the log directory to be deleted</param>
        public void DeleteLogs(string vin)
        {
            try
            {
                _fileRepository.DeleteDirectory($"{_pathToLogs}/{vin}");
            }
            catch (IOException e)
            {
                throw new LogFileOpenException(e.Message, e);
            }
        }

        /// <summary>
        /// Starts logging for a specific VIN with a given severity level,
        /// </summary>
        /// <param name="vin">VIN to be logged for</param>
        /// <param name="logLevel">Severity of logs</param>
        public void StartLogging(string vin, LogLevel logLevel)
        {
            ApplyLoggingRules(vin, logLevel);
            LogManager.ReconfigExistingLoggers();
            LogManager.GetLogger("Logging").Log(logLevel, $"Started Logging for {vin} on LogLevel {logLevel}");
            SystemInfo.LogSystemInfo(vin);
        }

        public const string VinLogFileName = "VinLog.log";
        public const string VinRuleEnding = "_VinLog";

        public static string GetVinRuleName(string vin)
        {
            return $"{vin}{VinRuleEnding}";
        }

        /// <summary>
        /// Updates the new logging rules and filetargets to the Nlog config file dynamically
        /// </summary>
        /// <param name="vin">VIN to be logged for</param>
        /// <param name="logLevel">Level of severity to be set</param>
        private void ApplyLoggingRules(string vin, LogLevel logLevel)
        {
            var loggingRule = LogManager.Configuration.LoggingRules.FirstOrDefault(l => l.RuleName == GetVinRuleName(vin));
            if (loggingRule == null)
            {
                CreateLoggingRule(vin, logLevel);
            }
            else
            {
                loggingRule.SetLoggingLevels(logLevel, LogLevel.Fatal);
                loggingRule.EnableLoggingForLevels(logLevel, LogLevel.Fatal);
            }
        }

        /// <summary>
        /// Stops logging for the specific VIN
        /// </summary>
        /// <param name="vin">VIN to be stopped logging</param>
        public void StopLogging(string vin)
        {
            var rule = LogManager.Configuration.LoggingRules.FirstOrDefault(r => r.RuleName == GetVinRuleName(vin));

            if (rule == null)
                throw new NoLogRuleException($"There is no ongoing logging for VIN: {vin}");

            LogManager.GetLogger("Logging").Log(LogLevel.Info, $"Stopped logging for {vin}");

            rule.DisableLoggingForLevels(LogLevel.Trace, LogLevel.Fatal);
        }

        public void DeleteZipFile(string filename)
        {
            _fileRepository.DeleteZipFile($"{_pathToLogs}/{filename}");
        }

        /// <summary>
        /// Creates new a new Filetarget and loggingrule and applies them to the Nlog config dynamically.
        /// </summary>
        /// <param name="vin">Vin number</param>
        /// <param name="logLevel">What loglevel</param>
        /// <returns>An updated Nlog config with a new filetarget and loggingrule </returns>
        private void CreateLoggingRule(string vin, LogLevel logLevel)
        {
            LoggingRule loggingrule;
            string fileName = $"{_pathToLogs}/{vin}/{VinLogFileName}";
            FileTarget logTarget = new(vin)
            {
                FileName = fileName,
                Layout = "${longdate} ${message} ${exception:format=Message}",
                ArchiveAboveSize = _maxSizeForVinLog,
                MaxArchiveFiles = 1,
                ArchiveFileName = fileName  // VinLog.log will be archived as VinLog.1.log.
            };
            LogManager.Configuration.AddTarget(fileName, logTarget);

            loggingrule = new LoggingRule("*", logLevel, logTarget) { RuleName = GetVinRuleName(vin) };
            loggingrule.FilterDefaultAction = FilterResult.Ignore;
            SetFilter(loggingrule, vin);
            LogManager.Configuration.LoggingRules.Insert(0, loggingrule);
        }

        private static void SetFilter(LoggingRule loggingrule, string vin)
        {
            // Ignore Microsoft loggers.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "starts-with('${logger}', 'Microsoft.')",
                Action = FilterResult.Ignore
            });

            // Ignore functional TesterPresent.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "contains('${message}', 'Functional Request: 3E 80')",
                Action = FilterResult.Ignore
            });

            // Log SystemInfo VIN logger matching this VIN.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = $"logger == 'Vin.SystemInfo.{vin}'",
                Action = FilterResult.Log
            });

            // Log ISO 14229 VIN logger matching this VIN.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = $"logger == 'Vin.ISO 14229.{vin}'",
                Action = FilterResult.Log
            });

            // Log EthernetConnectionManager VIN logger matching this VIN.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = $"logger == 'Vin.EthernetConnectionManager.{vin}'",
                Action = FilterResult.Log
            });

            // Log API logger matching this VIN.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = $"logger == 'Vin.API.{vin}'",
                Action = FilterResult.Log
            });

            // Log SwdlSummary logger matching this VIN.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = $"logger == 'Vin.SwdlSummary.{vin}'",
                Action = FilterResult.Log
            });

            // Ignore VIN logger that do not match this VIN.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "starts-with('${logger}', 'Vin.') and !ends-with('${logger}', '." + vin + "')",
                Action = FilterResult.Ignore
            });

            // Log non-VIN logger that include VIN in message.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "contains('${message}', '" + vin + "')",
                Action = FilterResult.Log
            });

            // Log non-VIN logger that include VIN in message.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "contains('${exception}', '" + vin + "')",
                Action = FilterResult.Log
            });

            // Log fatal errors from any logger.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "level >= LogLevel.Fatal",
                Action = FilterResult.Log
            });

            // Ignore info and lower level from any logger.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "level <= LogLevel.Info",
                Action = FilterResult.Ignore
            });

            // Log matching VIN logger.
            loggingrule.Filters.Add(new ConditionBasedFilter
            {
                Condition = "starts-with('${logger}', 'Vin.') and ends-with('${logger}', '." + vin + "')",
                Action = FilterResult.Log
            });
        }

        /// <summary>
        /// Calculates which files are currently being logged to,
        /// - i.e, which VINs are logging.
        /// </summary>
        /// <returns>
        /// A List of all VINs that are currently logging
        /// or have been logging combined with a status for each vin
        /// </returns>
        public Task<List<ActiveLogging>> GetLogStatus()
        {
            var loggingrules = LogManager.Configuration.LoggingRules.ToList();
            var nonActive = _fileRepository.GetDirectories(_pathToLogs).ToList();
            nonActive.RemoveAll(c => c.Contains("SystemLogger"));
            var activeRules = new HashSet<ActiveLogging>();

            foreach (var rule in loggingrules)
            {
                var rulename = new string(rule.RuleName.Take(17).ToArray());
                if (NonVinLoggingRule(rule) || !rule.IsLoggingEnabledForLevel(LogLevel.Error))
                {
                    continue;
                }                
                activeRules.Add(new ActiveLogging(rulename, true));
                nonActive.Remove($"{_pathToLogs}\\{rulename}");
            }
            //Adds all Vins currently not logging, where logs exists
            nonActive.ForEach(x => activeRules.Add(new ActiveLogging(x[^17..], false)));
            return Task.FromResult(activeRules.ToList());
        }
        private static bool NonVinLoggingRule(LoggingRule rule)
        {
            return rule.RuleName.Equals("SystemLogger") || rule.RuleName.Equals("Microsoft") || rule.RuleName.Equals("HttpLogger") || rule.RuleName.Equals("ErrorLogger");
        }
    }
}
