namespace Logging.Options
{
    public interface ILogSettings
    {
        long MaxSizeForVinLog { get; set; }
        string LogFilePath { get; set; }
    }
}
